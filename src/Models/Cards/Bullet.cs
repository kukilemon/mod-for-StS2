using IreneMod.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models.Cards;

public sealed class Bullet : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Shoot", 4m)];

    public Bullet()
        : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, shouldShowInCardLibrary: false)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await ShootCmd.Single(context, this, play.Target, DynamicVars["Shoot"].BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars["Shoot"].UpgradeValueBy(2m);
}
