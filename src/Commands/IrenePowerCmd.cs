using IreneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Commands;

public static class IrenePowerCmd
{
    public static async Task ApplyFloating(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature applier,
        CardModel source)
    {
        if (applier.HasPower<FloatingLockPower>())
        {
            return;
        }
        int bonus = applier.GetPower<GravityReversalPower>()?.Amount ?? 0;
        await PowerCmd.Apply<FloatingPower>(
            choiceContext, target, amount + bonus, applier, source);
        if (target.HasPower<FloatingPower>())
        {
            var progress = applier.GetPower<FinalFormProgressPower>();
            if (progress is not null)
                await progress.RecordFloating(choiceContext);
        }
    }

    public static async Task ApplyImbalance(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature applier,
        AbstractModel source)
    {
        if (target.HasPower<FloatingPower>() || amount <= 0)
        {
            return;
        }

        await PowerCmd.Apply<ImbalancePower>(
            choiceContext,
            target,
            amount,
            applier,
            source as CardModel);
    }
}
