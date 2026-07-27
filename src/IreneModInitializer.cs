using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace IreneMod;

[ModInitializer(nameof(Initialize))]
public static class IreneModInitializer
{
    public static void Initialize()
    {
        new Harmony("airlemon.IreneMod").PatchAll();
        Log.Info("IreneMod loaded.");
    }
}
