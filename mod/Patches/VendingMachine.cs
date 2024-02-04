using HarmonyLib;
using Reptile;
using UnityEngine;

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
                        MapPin linkedPin = PinManager.Instance.vendingMachineLinks[__instance];
                        if (PinManager.Instance.musicPins.Contains(linkedPin)) PinManager.Instance.musicPins.Remove(linkedPin);
                        if (PinManager.Instance.graffitiCollectiblePins.Contains(linkedPin)) PinManager.Instance.graffitiCollectiblePins.Remove(linkedPin);
                        if (PinManager.Instance.outfitPins.Contains(linkedPin)) PinManager.Instance.outfitPins.Remove(linkedPin);
                        if (PinManager.Instance.movestylePins.Contains(linkedPin)) PinManager.Instance.movestylePins.Remove(linkedPin);
                        PinManager.Instance.vendingMachineLinks.Remove(__instance);
                        PinManager.Instance.RemovePin(linkedPin);
                        if (linkedPin.gameObject != null) GameObject.Destroy(linkedPin.gameObject);
                    }
                }
            }
        }
    }
}
