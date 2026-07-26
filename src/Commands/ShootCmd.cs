using EireneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands.Builders;

namespace EireneMod.Commands;

public static class ShootCmd
{
    public static decimal GetDamage(CardModel card, decimal baseDamage)
    {
        decimal precision = card.Owner.Creature.GetPower<PrecisionPower>()?.Amount ?? 0m;
        decimal damage = baseDamage + precision;
        decimal explosive = card.Owner.Creature.GetPower<ExplosiveRoundPower>()?.Amount ?? 0m;
        return damage * (1m + explosive * 0.5m);
    }

    public static async Task<AttackCommand> Single(
        PlayerChoiceContext choiceContext,
        CardModel card,
        Creature target,
        decimal baseDamage,
        int hitCount = 1)
    {
        AttackCommand command = await DamageCmd.Attack(GetDamage(card, baseDamage))
            .FromCard(card)
            .Targeting(target)
            .WithHitCount(hitCount)
            .Unpowered()
            .Execute(choiceContext);
        await AfterShoot(choiceContext, card, [target], baseDamage, hitCount);
        return command;
    }

    public static async Task<AttackCommand> All(
        PlayerChoiceContext choiceContext,
        CardModel card,
        decimal baseDamage,
        int hitCount = 1)
    {
        ArgumentNullException.ThrowIfNull(card.Owner.Creature.CombatState);
        Creature[] targets = card.Owner.Creature.CombatState.HittableEnemies.ToArray();
        AttackCommand command = await DamageCmd.Attack(GetDamage(card, baseDamage))
            .FromCard(card)
            .TargetingAllOpponents(card.Owner.Creature.CombatState)
            .WithHitCount(hitCount)
            .Unpowered()
            .Execute(choiceContext);
        await AfterShoot(choiceContext, card, targets, baseDamage, hitCount);
        return command;
    }

    private static async Task AfterShoot(
        PlayerChoiceContext context,
        CardModel card,
        IEnumerable<Creature> targets,
        decimal baseDamage,
        int hitCount)
    {
        Creature[] livingTargets = targets.Where(target => target.IsAlive).Distinct().ToArray();
        var finalProgress = card.Owner.Creature.GetPower<FinalFormProgressPower>();
        if (finalProgress is not null)
            await finalProgress.RecordShoot(context);
        int dualWieldStacks = card.Owner.Creature
            .GetPower<DualWieldFormPower>()?.Amount ?? 0;

        // Each stack of Dual Wield Form repeats the original shot once.
        for (int repeat = 0; repeat < dualWieldStacks; repeat++)
        {
            foreach (Creature target in livingTargets)
            {
                await RawShot(context, card, target, baseDamage, hitCount);
            }
        }

        var doubleShotKit = card.Owner.Creature.GetPower<DoubleShotKitPower>();
        if (doubleShotKit is not null)
        {
            foreach (Creature target in livingTargets)
            {
                // The kit grants a genuine shot, so Dual Wield Form repeats it too.
                await RawShot(context, card, target, doubleShotKit.Amount, 1);
                for (int repeat = 0; repeat < dualWieldStacks; repeat++)
                {
                    await RawShot(context, card, target, doubleShotKit.Amount, 1);
                }
            }
        }

        var reinforcedCover = card.Owner.Creature.GetPower<ReinforcedCoverPower>();
        if (reinforcedCover is not null)
        {
            await CreatureCmd.GainBlock(
                card.Owner.Creature,
                (decimal)reinforcedCover.Amount,
                MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered,
                null);
        }

        var quickReload = card.Owner.Creature.GetPower<QuickReloadPower>();
        if (quickReload is not null)
        {
            await CardPileCmd.Draw(context, 2, card.Owner);
            await PowerCmd.Decrement(quickReload);
        }
    }

    private static async Task RawShot(
        PlayerChoiceContext context,
        CardModel card,
        Creature target,
        decimal baseDamage,
        int hitCount)
    {
        var finalProgress = card.Owner.Creature.GetPower<FinalFormProgressPower>();
        if (finalProgress is not null)
            await finalProgress.RecordShoot(context);
        await DamageCmd.Attack(GetDamage(card, baseDamage))
            .FromCard(card)
            .Targeting(target)
            .WithHitCount(hitCount)
            .Unpowered()
            .Execute(context);
    }
}
