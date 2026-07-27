using IreneMod.Commands;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IreneMod.Models.Cards;

public sealed class LightTheWay : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1), new BlockVar(3m, ValueProp.Move)];
    public LightTheWay() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        if (Owner.Creature.HasPower<LanternPower>())
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}

public sealed class PrepareUnderLantern : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("PutBack", 1m)];
    public PrepareUnderLantern() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        var selected = await CardSelectCmd.FromHand(context, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this);
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class BorrowLight : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    public BorrowLight() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        if (!Owner.Creature.HasPower<LanternPower>())
        {
            var card = (await CardSelectCmd.FromHand(context, Owner,
                new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, this)).FirstOrDefault();
            if (card is not null) await CardCmd.Exhaust(context, card);
        }
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class LightMatch : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public LightMatch() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay play) => LanternCmd.Light(context, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class LanternStrike : CardModel
{
    protected override bool IsPlayable => Owner.Creature.HasPower<LanternPower>();
    protected override bool ShouldGlowGoldInternal => IsPlayable;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20m, ValueProp.Move)];
    public LanternStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(context);
        await LanternCmd.Extinguish(this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}

public sealed class ThrowSpark : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(2m)];
    public ThrowSpark() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        if (Owner.Creature.HasPower<LanternPower>() && CombatState is not null)
            await PowerCmd.Apply<VulnerablePower>(context, CombatState.HittableEnemies,
                DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        else
            await PowerCmd.Apply<VulnerablePower>(context, play.Target,
                DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Vulnerable.UpgradeValueBy(1m);
}

public sealed class Disarm : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move), new PowerVar<WeakPower>(2m)];
    public Disarm() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target).Execute(context);
        if (Owner.Creature.HasPower<LanternPower>())
            await PowerCmd.Apply<WeakPower>(context, play.Target, 2, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class NightWatch : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(13m, ValueProp.Move)];
    public NightWatch() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class LanternBearer : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    public LanternBearer() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await LanternCmd.Light(context, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
