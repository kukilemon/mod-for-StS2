param(
    [string]$MegaDotPath = "D:\megadot_4.5.1\MegaDot_v4.5.1-stable_mono_win64_console.exe"
)

$projectRoot = Split-Path -Parent $PSScriptRoot
& $MegaDotPath `
    --headless `
    --path $projectRoot `
    --script "res://tools/pack_pck.gd"

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Created $(Join-Path $projectRoot 'build\IreneMod.pck')"
