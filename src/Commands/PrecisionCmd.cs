using EireneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace EireneMod.Commands;

public static class PrecisionCmd
{
    public static async Task GainTemporary(
        PlayerChoiceContext choiceContext,
        CardModel source,
        decimal amount)
    {
        await PowerCmd.Apply<PrecisionPower>(
            choiceContext,
            source.Owner.Creature,
            amount,
            source.Owner.Creature,
            source);

        await PowerCmd.Apply<LosePrecisionPower>(
            choiceContext,
            source.Owner.Creature,
            amount,
            source.Owner.Creature,
            source);
    }
}
