using EireneMod.Models.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace EireneMod.Commands;

public static class LanternCmd
{
    public const int DefaultMaximum = 2;

    public static async Task Light(
        PlayerChoiceContext choiceContext,
        CardModel source)
    {
        var owner = source.Owner.Creature;
        if (owner.HasPower<LanternLockPower>())
        {
            return;
        }

        int maximum = DefaultMaximum + (owner.GetPower<LanternCapacityPower>()?.Amount ?? 0);
        var existing = owner.GetPower<LanternPower>();
        if (existing is not null)
        {
            await PowerCmd.Remove(existing);
        }

        await PowerCmd.Apply<LanternPower>(
            choiceContext,
            owner,
            maximum,
            owner,
            source);

        var draw = owner.GetPower<GuideDrawPower>();
        if (draw is not null)
            await CardPileCmd.Draw(choiceContext, draw.Amount, source.Owner);

        var block = owner.GetPower<GuideBlockPower>();
        if (block is not null)
            await CreatureCmd.GainBlock(owner, block.Amount,
                MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, null);

        var impact = owner.GetPower<FlameImpactPower>();
        if (impact is not null && owner.CombatState is not null)
            foreach (var enemy in owner.CombatState.HittableEnemies)
                await EirenePowerCmd.ApplyImbalance(
                    choiceContext, enemy, impact.Amount, owner, source);

        var progress = owner.GetPower<FinalFormProgressPower>();
        if (progress is not null)
            await progress.RecordLanternLit(choiceContext);
    }

    public static async Task Extinguish(CardModel source)
    {
        var lantern = source.Owner.Creature.GetPower<LanternPower>();
        if (lantern is not null)
            await PowerCmd.Remove(lantern);
    }
}
