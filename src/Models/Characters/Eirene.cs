using EireneMod.Models.Cards;
using EireneMod.Models.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace EireneMod.Models.Characters;

public sealed class Eirene : CharacterModel
{
    public override CharacterGender Gender => CharacterGender.Feminine;
    protected override CharacterModel? UnlocksAfterRunAs => null;

    public override Color NameColor => new("C8A6FF");
    public override int StartingHp => 70;
    public override int StartingGold => 99;

    public override CardPoolModel CardPool => ModelDb.CardPool<EireneCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<EireneRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<IroncladPotionPool>();

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<EireneStrike>(),
        ModelDb.Card<EireneStrike>(),
        ModelDb.Card<EireneStrike>(),
        ModelDb.Card<EireneStrike>(),
        ModelDb.Card<EireneDefend>(),
        ModelDb.Card<EireneDefend>(),
        ModelDb.Card<EireneDefend>(),
        ModelDb.Card<EireneDefend>(),
        ModelDb.Card<StandardLantern>(),
        ModelDb.Card<OpenFire>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
        [ModelDb.Relic<ChurchRapier>()];

    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    public override Color EnergyLabelOutlineColor => new("493568");
    public override Color DialogueColor => new("493568");
    public override VfxColor SpeechBubbleColor => VfxColor.Purple;
    public override Color MapDrawingColor => new("9B72CF");
    public override Color RemoteTargetingLineColor => new("C8A6FF");
    public override Color RemoteTargetingLineOutline => new("493568");

    public override string CharacterSelectSfx =>
        ModelDb.Character<MegaCrit.Sts2.Core.Models.Characters.Ironclad>().CharacterSelectSfx;

    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_slash",
        "vfx/vfx_dramatic_stab"
    ];
}
