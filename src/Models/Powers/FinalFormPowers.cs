using IreneMod.Models.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IreneMod.Models.Powers;

public sealed class FinalFormPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public sealed class FinalFormProgressPower : PowerModel
{
    private bool _airCondition;
    private bool _shootCondition;
    private bool _lanternCondition;
    private int _comboGained;
    private int _shots;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount =>
        3 - (_airCondition ? 1 : 0) - (_shootCondition ? 1 : 0) - (_lanternCondition ? 1 : 0);

    public Task RecordFloating(PlayerChoiceContext context)
    {
        _airCondition = true;
        return CheckCompletion(context);
    }

    public Task RecordCombo(PlayerChoiceContext context, int amount)
    {
        if (!_airCondition)
        {
            _comboGained += amount;
            if (_comboGained >= 3) _airCondition = true;
        }
        return CheckCompletion(context);
    }

    public Task RecordShoot(PlayerChoiceContext context)
    {
        if (!_shootCondition)
        {
            _shots++;
            if (_shots >= 2) _shootCondition = true;
        }
        return CheckCompletion(context);
    }

    public Task RecordLanternLit(PlayerChoiceContext context)
    {
        _lanternCondition = true;
        return CheckCompletion(context);
    }

    private async Task CheckCompletion(PlayerChoiceContext context)
    {
        InvokeDisplayAmountChanged();
        if (!_airCondition || !_shootCondition || !_lanternCondition) return;
        var form = Owner.GetPower<FinalFormPower>();
        if (form is not null)
        {
            await PowerCmd.Apply<StrengthPower>(context, Owner, form.Amount, Owner, null);
            await PowerCmd.Apply<PrecisionPower>(context, Owner, form.Amount, Owner, null);
            await CardPileCmd.Draw(context, form.Amount, Owner.Player!);
        }
        _airCondition = _shootCondition = _lanternCondition = false;
        _comboGained = _shots = 0;
        InvokeDisplayAmountChanged();
    }
}
