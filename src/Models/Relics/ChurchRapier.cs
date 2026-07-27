using IreneMod.Commands;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IreneMod.Models.Relics;

public sealed class ChurchRapier : RelicModel
{
    private readonly HashSet<Creature> _targetsHitByCurrentAttack = [];
    private CardModel? _currentAttack;
    public int ComboGainedThisCombat { get; private set; }

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStart()
    {
        ComboGainedThisCombat = 0;
        await PowerCmd.Apply<NightWatchCostPower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature, 1, Owner.Creature, null);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            _currentAttack = cardPlay.Card;
            _targetsHitByCurrentAttack.Clear();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (dealer != Owner.Creature || cardSource != _currentAttack)
        {
            return;
        }

        _targetsHitByCurrentAttack.Add(target);

        if (target.HasPower<FloatingPower>())
        {
            ComboGainedThisCombat++;
            Flash();
            await PowerCmd.Apply<ComboPower>(
                choiceContext,
                Owner.Creature,
                1,
                Owner.Creature,
                cardSource);
            var progress = Owner.Creature.GetPower<FinalFormProgressPower>();
            if (progress is not null)
                await progress.RecordCombo(choiceContext, 1);
        }
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card != _currentAttack)
        {
            return;
        }

        foreach (var target in _targetsHitByCurrentAttack.Where(target => target.IsAlive))
        {
            await IrenePowerCmd.ApplyImbalance(
                choiceContext,
                target,
                1,
                Owner.Creature,
                this);
        }

        _targetsHitByCurrentAttack.Clear();
        _currentAttack = null;
    }
}
