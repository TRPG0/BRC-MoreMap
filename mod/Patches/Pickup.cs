﻿using HarmonyLib;
using Reptile;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(Pickup), "ApplyPickupType")]
    public class Pickup_ApplyPickupType
    {
        public static void Postfix(Pickup.PickUpType pickupType)
        {
            if (PinManager.Instance == null) Core.Instance.DelayMapSetup(0.25f);
        }
    }
}
