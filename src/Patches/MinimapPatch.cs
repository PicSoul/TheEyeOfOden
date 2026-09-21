using HarmonyLib;
using TheEyeofOden.Radar;

namespace TheEyeofOden.Patches
{
    [HarmonyPatch(typeof(Minimap))]
    public static class MinimapPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Postfix_Awake(Minimap __instance)
        {
            RadarOverlay.Initialize(__instance);
        }

        [HarmonyPatch("UpdatePins")]
        [HarmonyPostfix]
        private static void Postfix_UpdatePins(Minimap __instance)
        {
            RadarOverlay.UpdateRadar(__instance);
        }

        [HarmonyPatch("OnDestroy")]
        [HarmonyPrefix]
        private static void Prefix_OnDestroy()
        {
            RadarOverlay.Destroy();
        }
    }
}
