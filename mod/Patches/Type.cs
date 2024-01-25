using HarmonyLib;
using System;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(Type))]
    public class Type_GetType_Patch
    {
        [HarmonyPatch("GetType", (typeof(string)))]
        public static void Postfix(string typeName, ref Type __result)
        {
            if (typeName == "Reptile.Phone.AppMap") __result = typeof(AppMap);
        }
    }
}
