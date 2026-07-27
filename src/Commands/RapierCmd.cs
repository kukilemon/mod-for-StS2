using IreneMod.Models.Cards;
using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using HarmonyLib;

namespace IreneMod.Commands;

public static class RapierCmd
{
    public const int ImbalanceThreshold = 5;

    public static async Task<AttackCommand> Single(
        PlayerChoiceContext choiceContext,
        CardModel card,
        Creature target,
        decimal damage,
        int hitCount = 1)
    {
        if (card is not IRapierCard)
        {
            throw new ArgumentException("RapierCmd requires an IRapierCard.", nameof(card));
        }

        await TryLaunch(choiceContext, card, target);
        decimal adjustedDamage = await ConsumeDamageBonus(card, damage);

        AttackCommand command = DamageCmd.Attack(adjustedDamage)
            .FromCard(card)
            .Targeting(target)
            .WithHitCount(hitCount);
        MakeUnblockableIfNeeded(command, card);
        await command.Execute(choiceContext);
        await ConsumeImbalanceBonus(choiceContext, card, [target]);
        return command;
    }

    public static async Task<AttackCommand> All(
        PlayerChoiceContext choiceContext,
        CardModel card,
        decimal damage,
        int hitCount = 1)
    {
        if (card is not IRapierCard)
        {
            throw new ArgumentException("RapierCmd requires an IRapierCard.", nameof(card));
        }

        ArgumentNullException.ThrowIfNull(card.Owner.Creature.CombatState);
        Creature[] targets = card.Owner.Creature.CombatState.HittableEnemies.ToArray();
        foreach (Creature target in targets)
        {
            await TryLaunch(choiceContext, card, target);
        }

        decimal adjustedDamage = await ConsumeDamageBonus(card, damage);
        AttackCommand command = DamageCmd.Attack(adjustedDamage)
            .FromCard(card)
            .TargetingAllOpponents(card.Owner.Creature.CombatState)
            .WithHitCount(hitCount);
        MakeUnblockableIfNeeded(command, card);
        await command.Execute(choiceContext);
        await ConsumeImbalanceBonus(choiceContext, card, targets);
        return command;
    }

    public static bool WillLaunch(Creature target) =>
        !target.HasPower<FloatingPower>()
        && (target.GetPower<ImbalancePower>()?.Amount ?? 0) >= ImbalanceThreshold;

    private static async Task TryLaunch(
        PlayerChoiceContext choiceContext,
        CardModel card,
        Creature target)
    {
        var imbalance = target.GetPower<ImbalancePower>();
        if (target.HasPower<FloatingPower>()
            || imbalance is null
            || imbalance.Amount < ImbalanceThreshold)
        {
            return;
        }

        // Floating Lock will be checked here once that debuff is implemented.
        await PowerCmd.Remove(imbalance);
        await IrenePowerCmd.ApplyFloating(
            choiceContext, target, 2, card.Owner.Creature, card);
    }

    private static void MakeUnblockableIfNeeded(AttackCommand command, CardModel card)
    {
        if (!card.Owner.Creature.HasPower<MoonlightSwordPower>())
        {
            return;
        }

        var setter = AccessTools.PropertySetter(typeof(AttackCommand), nameof(AttackCommand.DamageProps));
        setter.Invoke(command, [command.DamageProps | ValueProp.Unblockable]);
    }

    private static async Task<decimal> ConsumeDamageBonus(CardModel card, decimal damage)
    {
        var power = card.Owner.Creature.GetPower<AdjustStancePower>();
        if (power is null)
        {
            return damage;
        }

        decimal result = damage + power.Amount;
        await PowerCmd.Remove(power);
        return result;
    }

    private static async Task ConsumeImbalanceBonus(
        PlayerChoiceContext context,
        CardModel card,
        IEnumerable<Creature> targets)
    {
        var power = card.Owner.Creature.GetPower<FencingEtiquettePower>();
        if (power is null)
        {
            return;
        }

        foreach (Creature target in targets.Where(target => target.IsAlive))
        {
            await IrenePowerCmd.ApplyImbalance(
                context, target, power.Amount, card.Owner.Creature, card);
        }
        await PowerCmd.Remove(power);
    }
}
