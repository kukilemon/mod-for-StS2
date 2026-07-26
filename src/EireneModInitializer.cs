using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace EireneMod;

[ModInitializer(nameof(Initialize))]
public static class EireneModInitializer
{
    public static void Initialize()
    {
        new Harmony("airlemon.EireneMod").PatchAll();
        Log.Info("EireneMod loaded.");
    }
}
