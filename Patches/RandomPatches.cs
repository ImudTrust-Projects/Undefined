using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using UnityEngine;
using static Undefined.Utilities.Variables;

namespace Undefined.Patches;

[HarmonyPatch(typeof(NewMapsDisplay), nameof(NewMapsDisplay.UpdateSlideshow))]
public static class NewMapsDisplay_UpdateSlideshow_Patch
{
    private static bool Prefix(NewMapsDisplay __instance)
    {
        if (__instance == null)
            return true;

        return __instance.mapImage != null && __instance.mapImage.gameObject != null;
    }
}

// thx to iidk for this.
[HarmonyPatch(typeof(VRRig), nameof(VRRig.IsItemAllowed))]
public class CosmeticPatch
{
    public static bool enabled;

    public static void Postfix(VRRig __instance, ref bool __result)
    {
        if (enabled)
            __result = true;
    }
}

// thx to iidk for this.
[HarmonyPatch(typeof(BundleManager), nameof(BundleManager.CheckIfBundlesOwned))]
public class PostGetData
{
    public static bool CosmeticsInitialized;
    private static void Postfix()
    {
        CosmeticsInitialized = true;
        CosmeticsOwned = CosmeticsController.instance.concatStringCosmeticsAllowed;
    }
}