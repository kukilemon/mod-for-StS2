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

public sealed class RapidFire : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Shoot", 5m)];
    public RapidFire() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
    }
    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(2m);
}

public sealed class CruisingMissile : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Shoot", 3m)];
    public CruisingMissile() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override bool ShouldGlowGoldInternal =>
        CombatState?.HittableEnemies.Any(enemy => enemy.HasPower<FloatingPower>()) ?? false;
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        if (play.Target.HasPower<FloatingPower>())
        {
            await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue, 2);
        }
    }
    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(1m);
}

public sealed class HoldBreath : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("TemporaryPrecision", 3m)];
    public HoldBreath() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PrecisionCmd.GainTemporary(context, this, DynamicVars["TemporaryPrecision"].BaseValue);
    protected override void OnUpgrade() => DynamicVars["TemporaryPrecision"].UpgradeValueBy(2m);
}

public sealed class RollingShot : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 6m), new CardsVar(1)];
    public RollingShot() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Shoot"].UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}

public sealed class SuppressiveFire : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Shoot", 8m)];
    public SuppressiveFire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await ShootCmd.All(context, this, DynamicVars["Shoot"].BaseValue);
    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(2m);
}

public sealed class AimForTheVitals : CardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 4m), new PowerVar<VulnerablePower>(2m)];
    public AimForTheVitals() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
        await PowerCmd.Apply<VulnerablePower>(
            context, play.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Shoot"].UpgradeValueBy(2m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}

public sealed class Cover : CardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BlockNextTurnPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 7m), new BlockVar(7m, ValueProp.Unpowered)];
    public Cover() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
        await PowerCmd.Apply<BlockNextTurnPower>(
            context, Owner.Creature, DynamicVars.Block.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Shoot"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}

public sealed class TripleShot : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Shoot", 5m)];
    public TripleShot() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue, 3);
    }
    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(1m);
}
