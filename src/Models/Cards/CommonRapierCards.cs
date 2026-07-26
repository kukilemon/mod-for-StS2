using EireneMod.Commands;
using EireneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EireneMod.Models.Cards;

public sealed class AdjustStance : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<AdjustStancePower>(4m)];
    public AdjustStance() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<AdjustStancePower>(
            context, Owner.Creature, DynamicVars["AdjustStancePower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["AdjustStancePower"].UpgradeValueBy(2m);
}

public sealed class LungeStep : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    public LungeStep() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue, 2);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class RisingThrust : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new PowerVar<ImbalancePower>(1m)];
    public RisingThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue);
        await EirenePowerCmd.ApplyImbalance(
            context, play.Target, DynamicVars["ImbalancePower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["ImbalancePower"].UpgradeValueBy(1m);
    }
}

public sealed class PursuingThrust : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    public PursuingThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        int hits = play.Target.HasPower<FloatingPower>() ? 2 : 1;
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue, hits);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class ArmorPiercingThrust : CardModel, IRapierCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new PowerVar<VulnerablePower>(1m)];
    public ArmorPiercingThrust() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue);
        await PowerCmd.Apply<VulnerablePower>(
            context, play.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}

public sealed class AerialPursuit : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new CardsVar(2)];
    public AerialPursuit() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        bool floating = play.Target.HasPower<FloatingPower>();
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue);
        if (floating)
        {
            await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class FencingEtiquette : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new PowerVar<FencingEtiquettePower>(2m)];
    public FencingEtiquette() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<FencingEtiquettePower>(
            context, Owner.Creature, DynamicVars["FencingEtiquettePower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class AdvancingSlash : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new EnergyVar(2)];
    public AdvancingSlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        bool launches = RapierCmd.WillLaunch(play.Target);
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue);
        if (launches)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class SweepingSlash : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];
    public SweepingSlash() : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await RapierCmd.All(context, this, DynamicVars.Damage.BaseValue);
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
