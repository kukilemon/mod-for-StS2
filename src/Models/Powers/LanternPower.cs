using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models.Powers;

public sealed class LanternPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner.Player || Amount <= 0)
        {
            return;
        }

        Flash();
        await PlayerCmd.GainEnergy(1, player);
        if (!Owner.HasPower<EternalLanternPower>())
            await PowerCmd.Decrement(this);
    }
}
