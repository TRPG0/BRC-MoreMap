using HarmonyLib;
using Reptile;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(PhotoObjectiveProgressable), "MadePhotograph")]
    public class PhotoObjectiveProgressable_MadePhotograph_Patch
    {
        public static void Postfix(PhotoObjectiveProgressable __instance)
        {
            if (PinManager.Instance != null)
            {
                if (PinManager.Instance.poloLinks.ContainsKey(__instance))
                {
                    Traverse.Create(PinManager.Instance.poloLinks[__instance]).Method("DisableMapPinGameObject").GetValue();
                }
            }
        }
    }
}
