using IreneMod.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models.Cards;

public sealed class StandardLantern : CardModel
{
    public StandardLantern()
        : base(2, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay) =>
        await LanternCmd.Light(choiceContext, this);

    protected override void OnUpgrade() =>
        EnergyCost.UpgradeBy(-1);
}
