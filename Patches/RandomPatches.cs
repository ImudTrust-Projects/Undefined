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