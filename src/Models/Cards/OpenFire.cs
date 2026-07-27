using IreneMod.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models.Cards;

public sealed class OpenFire : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Shoot", 9m)];

    public OpenFire()
        : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await ShootCmd.Single(
            choiceContext,
            this,
            cardPlay.Target,
            DynamicVars["Shoot"].BaseValue);
    }

    protected override void OnUpgrade() =>
        DynamicVars["Shoot"].UpgradeValueBy(3m);
}
