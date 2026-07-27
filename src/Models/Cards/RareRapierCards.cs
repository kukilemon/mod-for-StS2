using IreneMod.Commands;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IreneMod.Models.Cards;

public sealed class DuelStance : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public DuelStance() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int combo = Owner.Creature.GetPower<ComboPower>()?.Amount ?? 0;
        if (combo > 0)
            await PowerCmd.Apply<StrengthPower>(context, Owner.Creature, combo, Owner.Creature, this);
    }
    protected override void OnUpgrade() => RemoveKeyword(CardKeyword.Exhaust);
}

public sealed class Judgment : CardModel, IRapierCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];
    public Judgment() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        int hits = 1;
        if (play.Target.HasPower<FloatingPower>()) hits++;
        if ((Owner.Creature.GetPower<ComboPower>()?.Amount ?? 0) >= 3) hits++;
        if (Owner.Creature.HasPower<LanternPower>()) hits++;
        await RapierCmd.Single(context, this, play.Target, DynamicVars.Damage.BaseValue, hits);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class SwordGunConcerto : CardModel
{
    public SwordGunConcerto() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<SwordGunConcertoPower>(context, Owner.Creature, 1, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class MeteorSword : CardModel
{
    public MeteorSword() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<MeteorSwordPower>(context, Owner.Creature, 1, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class GaleFinalThrust : CardModel, IRapierCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move)];
    public GaleFinalThrust() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        var targets = CombatState.HittableEnemies.ToArray();
        await RapierCmd.All(context, this, DynamicVars.Damage.BaseValue);
        foreach (var target in targets.Where(t => t.HasPower<FloatingPower>()))
        {
            var floating = target.GetPower<FloatingPower>();
            if (floating is not null) await PowerCmd.Remove(floating);
            await PowerCmd.Apply<VulnerablePower>(context, target, 99, Owner.Creature, this);
            await PowerCmd.Apply<WeakPower>(context, target, 99, Owner.Creature, this);
        }
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
