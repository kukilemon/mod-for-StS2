using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models.Powers;

public sealed class LosePrecisionPower : PowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(Owner))
        {
            return;
        }

        var precision = Owner.GetPower<PrecisionPower>();
        if (precision is not null)
        {
            var stacksToLose = Math.Min(Amount, precision.Amount);
            for (var i = 0; i < stacksToLose; i++)
            {
                await PowerCmd.Decrement(precision);
            }
        }

        await PowerCmd.Remove(this);
    }
}
