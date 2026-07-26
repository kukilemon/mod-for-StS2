using EireneMod.Commands;
using EireneMod.Models.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace EireneMod.Models.Powers;

public sealed class SwordGunConcertoPower : PowerModel
{
    private int _rapierCards;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay play)
    {
        if (play.Card.Owner != Owner.Player || play.Card is not IRapierCard)
        {
            return;
        }

        _rapierCards++;
        if (_rapierCards < 2)
        {
            return;
        }

        _rapierCards = 0;
        var combat = Owner.CombatState;
        if (combat is null) return;
        var targets = combat.HittableEnemies.ToArray();
        if (targets.Length == 0) return;
        var target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(targets);
        if (target is not null)
            await ShootCmd.Single(context, play.Card, target, 5m);
    }
}

public sealed class MeteorSwordPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
