using IreneMod.Models;
using IreneMod.Models.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Patches;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCardPools), MethodType.Getter)]
public static class ModelDbAllCardPoolsPatch
{
    private static void Postfix(ref IEnumerable<CardPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.CardPool<IreneCardPool>())
            .Distinct();
    }
}

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCharacters), MethodType.Getter)]
public static class ModelDbAllCharactersPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        __result = __result
            .Append(ModelDb.Character<Irene>())
            .Distinct();
    }
}
