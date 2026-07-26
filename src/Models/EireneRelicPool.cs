using EireneMod.Models.Relics;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace EireneMod.Models;

public sealed class EireneRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "ironclad";
    public override Color LabOutlineColor => new("C8A6FF");

    protected override IEnumerable<RelicModel> GenerateAllRelics() =>
        [ModelDb.Relic<ChurchRapier>()];
}
