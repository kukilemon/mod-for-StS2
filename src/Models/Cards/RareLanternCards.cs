using EireneMod.Commands;
using EireneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EireneMod.Models.Cards;

public sealed class BrilliantLantern : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2), new CardsVar(2)];
    public BrilliantLantern() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        await LanternCmd.Light(context, this);
    }
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}

public sealed class ThrowFlame : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];
    public ThrowFlame() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
    protected override PileType GetResultPileTypeForCardPlay() =>
        Owner.Creature.HasPower<LanternPower>() ? PileType.Discard : base.GetResultPileTypeForCardPlay();
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(context);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}

public sealed class ReturnHome : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public ReturnHome() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int count = CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count;
        if (count > 0) await CardPileCmd.Draw(context, count, Owner);
        if (Owner.Creature.HasPower<LanternPower>())
            await PowerCmd.Apply<RetainHandPower>(context, Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade() { }
}

public sealed class ExpandedLantern : CardModel
{
    public ExpandedLantern() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<LanternCapacityPower>(context, Owner.Creature, 1, Owner.Creature, this);
        await LanternCmd.Light(context, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class FlameImpact : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FlameImpactPower>(2m)];
    public FlameImpact() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<FlameImpactPower>(context, Owner.Creature, 2, Owner.Creature, this);
        if (CurrentUpgradeLevel > 0) await LanternCmd.Light(context, this);
    }
    protected override void OnUpgrade() { }
}

public sealed class FinalForm : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FinalFormPower>(1m)];
    public FinalForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<FinalFormPower>(context, Owner.Creature,
            DynamicVars["FinalFormPower"].BaseValue, Owner.Creature, this);
        if (!Owner.Creature.HasPower<FinalFormProgressPower>())
            await PowerCmd.Apply<FinalFormProgressPower>(context, Owner.Creature, 3, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["FinalFormPower"].UpgradeValueBy(1m);
}

public sealed class EternalLantern : CardModel
{
    public EternalLantern() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<EternalLanternPower>(context, Owner.Creature, 1, Owner.Creature, this);
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}

public sealed class SeaBornTransformation : CardModel
{
    public SeaBornTransformation() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<SeaBornPower>(context, Owner.Creature, 3, Owner.Creature, this);
        await PowerCmd.Apply<LanternLockPower>(context, Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<FloatingLockPower>(context, Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
