using IreneMod.Commands;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IreneMod.Models.Cards;

public sealed class RecycleEmbers : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3)];
    public RecycleEmbers() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await LanternCmd.Extinguish(this);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class Guide : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<GuideDrawPower>(1m), new PowerVar<GuideBlockPower>(5m)];
    public Guide() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<GuideDrawPower>(context, Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<GuideBlockPower>(context, Owner.Creature,
            DynamicVars["GuideBlockPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["GuideBlockPower"].UpgradeValueBy(2m);
}

public sealed class RecallInLight : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public RecallInLight() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var card = (await CardSelectCmd.FromCombatPile(context, PileType.Discard.GetPile(Owner),
            Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        if (card is null) return;
        await CardPileCmd.Add(card, PileType.Hand);
        if (Owner.Creature.HasPower<LanternPower>()) card.SetToFreeThisTurn();
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class LightingRitual : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2), new DynamicVar("Exhaust", 3m)];
    public LightingRitual() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CardPileCmd.Draw(context, DynamicVars.Cards.BaseValue, Owner);
        int count = Math.Min(DynamicVars["Exhaust"].IntValue, PileType.Hand.GetPile(Owner).Cards.Count);
        if (count > 0)
        {
            var cards = await CardSelectCmd.FromHand(context, Owner,
                new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, count), null, this);
            foreach (var card in cards) await CardCmd.Exhaust(context, card);
        }
        await LanternCmd.Light(context, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVars["Exhaust"].UpgradeValueBy(-1m);
    }
}

public sealed class EmberShield : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];
    public EmberShield() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        int lantern = Owner.Creature.GetPower<LanternPower>()?.Amount ?? 0;
        if (lantern > 0)
            await CreatureCmd.GainBlock(Owner.Creature,
                DynamicVars.Block.BaseValue * lantern, ValueProp.Move, play);
        await LanternCmd.Extinguish(this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}

public sealed class GuardTheFlame : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];
    public GuardTheFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (Owner.Creature.HasPower<LanternPower>())
            await PowerCmd.Apply<RetainHandPower>(context, Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class AddLampOil : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<LampOilPower>(1m)];
    public AddLampOil() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<LampOilPower>(context, Owner.Creature,
            DynamicVars["LampOilPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["LampOilPower"].UpgradeValueBy(1m);
}

public sealed class EvolutionRitual : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move)];
    public EvolutionRitual() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        foreach (var power in Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff).ToArray())
            await PowerCmd.Remove(power);
        if (Owner.Creature.HasPower<LanternPower>())
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class IreneMetamorphosis : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>(1m)];
    public IreneMetamorphosis() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<IntangiblePower>(context, Owner.Creature,
            DynamicVars["IntangiblePower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<LanternLockPower>(context, Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["IntangiblePower"].UpgradeValueBy(1m);
}

public sealed class HolyLightBaptism : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Growth", 4m)];
    public HolyLightBaptism() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this).TargetingAllOpponents(CombatState).Execute(context);
    }
    public override Task AfterSideTurnEnd(
        PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player
            && participants.Contains(Owner.Creature)
            && Owner.Creature.HasPower<LanternPower>())
        {
            DynamicVars.Damage.BaseValue += DynamicVars["Growth"].BaseValue;
        }
        return Task.CompletedTask;
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Growth"].UpgradeValueBy(2m);
    }
}
