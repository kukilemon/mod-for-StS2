using EireneMod.Commands;
using EireneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EireneMod.Models.Cards;

public sealed class AerialIntercept : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("ComboDamage", 2m)];
    public AerialIntercept() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        decimal combo = Owner.Creature.GetPower<ComboPower>()?.Amount ?? 0;
        await RapierCmd.Single(context, this, play.Target,
            DynamicVars.Damage.BaseValue + combo * DynamicVars["ComboDamage"].BaseValue);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["ComboDamage"].UpgradeValueBy(1m);
    }
}

public sealed class PerfectParry : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PerfectParryPower>(1m)];
    public PerfectParry() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<PerfectParryPower>(context, Owner.Creature,
            DynamicVars["PerfectParryPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["PerfectParryPower"].UpgradeValueBy(1m);
}

public sealed class Launch : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FloatingPower>(2m)];
    public Launch() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await EirenePowerCmd.ApplyFloating(context, play.Target,
            DynamicVars["FloatingPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ChainThrust : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];
    public ChainThrust() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue, 4);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class DefensiveCounter : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(12m, ValueProp.Move), new PowerVar<DefensiveCounterPower>(6m)];
    public DefensiveCounter() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<DefensiveCounterPower>(context, Owner.Creature,
            DynamicVars["DefensiveCounterPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class MoonlightSword : CardModel
{
    public MoonlightSword() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<MoonlightSwordPower>(context, Owner.Creature, 1, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class GravityReversal : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GravityReversalPower>(1m)];
    public GravityReversal() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<GravityReversalPower>(context, Owner.Creature,
            DynamicVars["GravityReversalPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["GravityReversalPower"].UpgradeValueBy(1m);
}

public sealed class SunSword : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    public SunSword() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var command = await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue);
        int dealt = command.Results.SelectMany(results => results).Sum(result => result.UnblockedDamage);
        if (dealt > 0) await CreatureCmd.Heal(Owner.Creature, dealt);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class SwordWindSuppression : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new PowerVar<WeakPower>(1m)];
    public SwordWindSuppression() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        Creature[] targets = CombatState.HittableEnemies.ToArray();
        await RapierCmd.All(context, this, DynamicVars.Damage.BaseValue);
        await PowerCmd.Apply<WeakPower>(context, targets,
            DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}
