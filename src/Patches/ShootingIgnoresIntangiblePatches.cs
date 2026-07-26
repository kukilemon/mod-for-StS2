using EireneMod.Models.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace EireneMod.Patches;

[HarmonyPatch(typeof(IntangiblePower), nameof(IntangiblePower.ModifyDamageCap))]
public static class ShootingIgnoresIntangibleDamageCapPatch
{
    private static bool Prefix(
        CardModel? cardSource,
        ref decimal __result)
    {
        if (cardSource is null || !ShootingCardRegistry.Contains(cardSource))
        {
            return true;
        }

        __result = decimal.MaxValue;
        return false;
    }
}

[HarmonyPatch(typeof(IntangiblePower), nameof(IntangiblePower.ModifyHpLostAfterOsty))]
public static class ShootingIgnoresIntangibleHpLossPatch
{
    private static bool Prefix(
        decimal amount,
        CardModel? cardSource,
        ref decimal __result)
    {
        if (cardSource is null || !ShootingCardRegistry.Contains(cardSource))
        {
            return true;
        }

        __result = amount;
        return false;
    }
}
