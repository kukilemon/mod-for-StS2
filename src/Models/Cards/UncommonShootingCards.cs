using IreneMod.Commands;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.CardSelection;
using System.Linq;

namespace IreneMod.Models.Cards;

public sealed class LoadAmmunition : CardModel
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new PowerVar<PrecisionPower>(2m)];
    public LoadAmmunition() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        await PowerCmd.Apply<PrecisionPower>(
            context, Owner.Creature, DynamicVars["PrecisionPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["PrecisionPower"].UpgradeValueBy(1m);
    }
}

public sealed class BlindingRound : CardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 12m), new PowerVar<WeakPower>(2m)];
    public BlindingRound() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
        await PowerCmd.Apply<WeakPower>(
            context, play.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(4m);
}

public sealed class ConcussionRound : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 10m), new PowerVar<ImbalancePower>(3m)];
    public ConcussionRound() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
        await IrenePowerCmd.ApplyImbalance(
            context, play.Target, DynamicVars["ImbalancePower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(4m);
}

public sealed class ReinforcedCover : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ReinforcedCoverPower>(2m)];
    public ReinforcedCover() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<ReinforcedCoverPower>(
            context, Owner.Creature, DynamicVars["ReinforcedCoverPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() =>
        DynamicVars["ReinforcedCoverPower"].UpgradeValueBy(1m);
}

public sealed class HomemadeAmmunition : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Bullet>()];
    protected override bool IsPlayable => PileType.Hand.GetPile(Owner).Cards.Count >= 2;
    public HomemadeAmmunition() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var selected = await CardSelectCmd.FromHand(
            context,
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 2),
            null,
            this);
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(context, card);
        }

        for (var i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            ArgumentNullException.ThrowIfNull(CombatState);
            var bullet = CombatState.CreateCard<Bullet>(Owner);
            if (IsUpgraded)
            {
                bullet.UpgradeInternal();
            }
            await CardPileCmd.AddGeneratedCardToCombat(bullet, PileType.Hand, Owner);
        }
    }
}

public sealed class QuickReload : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<QuickReloadPower>(2m), new CardsVar(2)];
    public QuickReload() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play) =>
        await PowerCmd.Apply<QuickReloadPower>(
            context, Owner.Creature, DynamicVars["QuickReloadPower"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() =>
        DynamicVars["QuickReloadPower"].UpgradeValueBy(1m);
}

public sealed class TracerRound : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("StrengthLoss", 2m), new PowerVar<PrecisionPower>(4m)];
    public TracerRound() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(
            context, Owner.Creature, -DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<PrecisionPower>(
            context, Owner.Creature, DynamicVars["PrecisionPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() =>
        DynamicVars["StrengthLoss"].UpgradeValueBy(-1m);
}

public sealed class ExplosiveRound : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 6m), new PowerVar<ExplosiveRoundPower>(1m)];
    public ExplosiveRound() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
        await PowerCmd.Apply<ExplosiveRoundPower>(
            context, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
