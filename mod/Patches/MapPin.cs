using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(MapPin), "RefreshGraffitiMapPin")]
    public class MapPin_RefreshGraffitiMapPin_Patch
    {
        public static bool Prefix(MapPin __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            if (!traverse.Field<GameObject>("graffitiSpotGameObject").Value.activeSelf
                || (traverse.Field<GameObject>("containingChapterObject").Value != null && !traverse.Field<GameObject>("containingChapterObject").Value.activeSelf)
                || (traverse.Field<GameObject>("containingProgressObject").Value != null && !traverse.Field<GameObject>("containingProgressObject").Value.activeSelf))
            {
                if (traverse.Field<bool>("isMapPinActive").Value) traverse.Method("DisableMapPinGameObject").GetValue();
                return false;
            }
            if (!Core.showGraffitiSpot.Value || traverse.Field("m_GraffitiSpot").Method("ClaimedByPlayableCrew").GetValue<bool>())
            {
                if (traverse.Field<bool>("isMapPinActive").Value) traverse.Method("DisableMapPinGameObject").GetValue();
                return false;
            }
            if (!Core.requireMap.Value || Reptile.Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().mapFound)
            {
                __instance.UpdateLocation();
                if (!traverse.Field<bool>("isMapPinActive").Value) traverse.Method("EnableMapPinGameObject").GetValue();
                return false;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MapPin), "RefreshTaxiPin")]
    public class MapPin_RefreshTaxiPin_Patch
    {
        public static bool Prefix(MapPin __instance)
        {
            if (!Core.showTaxi.Value)
            {
                Traverse.Create(__instance).Method("DisableMapPinGameObject").GetValue();
                return false;
            }
            if (!Core.requireMap.Value)
            {
                Traverse.Create(__instance).Method("EnableMapPinGameObject").GetValue();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MapPin), "RefreshToiletPin")]
    public class MapPin_RefreshToiletPin_Patch
    {
        public static bool Prefix(MapPin __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            if (!Core.showToilet.Value || !traverse.Field<PublicToilet>("toilet").Value.CanSwap)
            {
                traverse.Method("DisableMapPinGameObject").GetValue();
                return false;
            }
            traverse.Method("EnableMapPinGameObject").GetValue();
            return false;
        }
    }

    [HarmonyPatch(typeof(MapPin), "RefreshCypherPin")]
    public class MapPin_RefreshCypherPin_Patch
    {
        public static bool Prefix(MapPin __instance)
        {
            if (!Core.showCypher.Value)
            {
                Traverse.Create(__instance).Method("DisableMapPinGameObject").GetValue();
                return false;
            }
            if (!Core.requireMap.Value)
            {
                Traverse.Create(__instance).Method("EnableMapPinGameObject").GetValue();
                return false;
            }
            return true;
        }
    }
}
