"""Split Irene's flattened combat art into reusable cutout-rig layers.

The source image must be a 1024x1536 transparent PNG using the current v7 pose.
Layer masks intentionally overlap by a few pixels so tiny idle rotations do not
expose bright seams between adjacent pieces.
"""

from __future__ import annotations

import json
from pathlib import Path

from PIL import Image, ImageChops, ImageDraw, ImageFilter


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "images/creature_visuals/irene/combat/irene_v7_clean.png"
OUTPUT = ROOT / "images/creature_visuals/irene/combat/rig"

LAYER_SPECS = [
    ("rear_hair", [(225, 90), (490, 90), (520, 655), (225, 655)], (452, 345)),
    ("ribbon_left", [(135, 590), (405, 565), (365, 1285), (125, 1285)], (335, 650)),
    ("ribbon_right", [(610, 575), (865, 650), (860, 1280), (720, 1280)], (690, 650)),
    ("head", [(425, 95), (645, 95), (650, 405), (420, 405)], (510, 355)),
    ("arm_left", [(260, 335), (450, 335), (445, 865), (235, 895)], (382, 390)),
    ("arm_right", [(585, 330), (720, 340), (900, 1040), (620, 870)], (625, 390)),
    ("lantern", [(535, 620), (680, 620), (690, 1005), (530, 1005)], (585, 675)),
    ("torso", [(340, 285), (680, 285), (715, 765), (325, 765)], (510, 645)),
]


def make_mask(size: tuple[int, int], points: list[tuple[int, int]]) -> Image.Image:
    mask = Image.new("L", size, 0)
    ImageDraw.Draw(mask).polygon(points, fill=255)
    return mask.filter(ImageFilter.MaxFilter(7))


def save_cropped_layer(
    source: Image.Image,
    name: str,
    mask: Image.Image,
    pivot: tuple[int, int],
) -> dict[str, object]:
    alpha = ImageChops.multiply(source.getchannel("A"), mask)
    bbox = alpha.getbbox()
    if bbox is None:
        raise RuntimeError(f"Layer {name!r} did not contain any visible pixels")

    layer = source.copy()
    layer.putalpha(alpha)
    layer = layer.crop(bbox)
    path = OUTPUT / f"{name}.png"
    layer.save(path, optimize=True)

    left, top, right, bottom = bbox
    return {
        "name": name,
        "texture": path.relative_to(ROOT).as_posix(),
        "pivot": list(pivot),
        "center": [(left + right) / 2.0, (top + bottom) / 2.0],
        "bbox": [left, top, right, bottom],
    }


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    source = Image.open(SOURCE).convert("RGBA")
    if source.size != (1024, 1536):
        raise ValueError(f"Expected a 1024x1536 source, got {source.size}")

    claimed = Image.new("L", source.size, 0)
    layers: list[dict[str, object]] = []

    # Front-most pieces claim their pixels first. Reversing at save time keeps
    # the scene's intended rear-to-front draw order while assigning every source
    # pixel to exactly one texture.
    masks: list[tuple[str, Image.Image, tuple[int, int]]] = []
    for name, points, pivot in LAYER_SPECS:
        masks.append((name, make_mask(source.size, points), pivot))

    exclusive_masks: dict[str, Image.Image] = {}
    for name, mask, _pivot in reversed(masks):
        exclusive = ImageChops.subtract(mask, claimed)
        exclusive_masks[name] = exclusive
        claimed = ImageChops.lighter(claimed, mask)

    base_mask = ImageChops.invert(claimed)
    layers.append(save_cropped_layer(source, "base", base_mask, (512, 1400)))
    for name, _mask, pivot in masks:
        layers.append(save_cropped_layer(source, name, exclusive_masks[name], pivot))

    manifest = {
        "source": SOURCE.relative_to(ROOT).as_posix(),
        "canvas_size": list(source.size),
        "layers": layers,
    }
    (OUTPUT / "rig_manifest.json").write_text(
        json.dumps(manifest, ensure_ascii=False, indent=2),
        encoding="utf-8",
    )
    print(json.dumps(manifest, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
