using IreneMod.Commands;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using IreneMod.Models.Relics;

namespace IreneMod.Models.Cards;

public sealed class WeaponModification : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PrecisionPower>(5m)];

    public WeaponModification()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<PrecisionPower>(
            context, Owner.Creature, DynamicVars["PrecisionPower"].BaseValue, Owner.Creature, this);

    protected override void OnUpgrade() =>
        DynamicVars["PrecisionPower"].UpgradeValueBy(2m);
}

public sealed class SolemnMourning : CardModel
{
    protected override bool HasEnergyCostX => true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 6m), new DynamicVar("Multiplier", 2m)];

    public SolemnMourning()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        int hitCount = ResolveEnergyXValue() * DynamicVars["Multiplier"].IntValue;
        if (hitCount > 0)
        {
            await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue, hitCount);
        }
    }

    protected override void OnUpgrade() =>
        DynamicVars["Shoot"].UpgradeValueBy(2m);
}

public sealed class DoubleShotKit : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DoubleShotKitPower>(4m)];

    public DoubleShotKit()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<DoubleShotKitPower>(
            context, Owner.Creature, DynamicVars["DoubleShotKitPower"].BaseValue, Owner.Creature, this);

    protected override void OnUpgrade() =>
        DynamicVars["DoubleShotKitPower"].UpgradeValueBy(1m);
}

public sealed class DualWieldForm : CardModel
{
    public DualWieldForm()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<DualWieldFormPower>(
            context, Owner.Creature, 1m, Owner.Creature, this);

    protected override void OnUpgrade() =>
        EnergyCost.UpgradeBy(-1);
}

public sealed class DeathDeathDeath : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 4m)];

    public DeathDeathDeath()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        int hitCount = Owner.GetRelic<ChurchRapier>()?.ComboGainedThisCombat ?? 0;
        if (hitCount > 0)
        {
            await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue, hitCount);
        }
    }

    protected override void OnUpgrade() =>
        DynamicVars["Shoot"].UpgradeValueBy(2m);
}
