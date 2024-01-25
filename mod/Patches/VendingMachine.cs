using HarmonyLib;
using Reptile;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(VendingMachine), "SetState")]
    public class VendingMachine_SetState_Patch
    {
        public static void Postfix(VendingMachine __instance, object __0)
        {
            //Core.Logger.LogInfo($"{(int)__0} {__0}");
            if ((int)__0 == 3 && __instance.rewards.Length > 0)
            {
                if (__instance.rewards[Traverse.Create(__instance).Field<int>("rewardCount").Value] == VendingMachine.Reward.UNLOCKABLE_DROP)
                {
                    if (PinManager.Instance != null && PinManager.Instance.vendingMachineLinks.ContainsKey(__instance)) 
                    {
                        Traverse.Create(PinManager.Instance.vendingMachineLinks[__instance]).Method("DisableMapPinGameObject").GetValue();
                    }
                }
            }
        }
    }
}
