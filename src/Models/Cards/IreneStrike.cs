using MegaCrit.Sts2.Core.Commands;
using IreneMod.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IreneMod.Models.Cards;

public sealed class IreneStrike : CardModel, IRapierCard
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move)];

    public IreneStrike()
        : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await RapierCmd.Single(
            choiceContext,
            this,
            cardPlay.Target,
            DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade() =>
        DynamicVars.Damage.UpgradeValueBy(3m);
}
