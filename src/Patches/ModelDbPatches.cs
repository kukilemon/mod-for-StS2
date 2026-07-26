using EireneMod.Models;
using EireneMod.Models.Characters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace EireneMod.Patches;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCardPools), MethodType.Getter)]
public static class ModelDbAllCardPoolsPatch
{
    private static void Postfix(ref IEnumerable<CardPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.CardPool<EireneCardPool>())
            .Distinct();
    }
}

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCharacters), MethodType.Getter)]
public static class ModelDbAllCharactersPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        __result = __result
            .Append(ModelDb.Character<Eirene>())
            .Distinct();
    }
}
