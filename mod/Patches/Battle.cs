using HarmonyLib;
using Reptile;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(Battle), "StartBattle")]
    public class Battle_StartBattle_Patch
    {
        public static void Prefix()
        {
            WorldHandler.instance.RemoveTempRacers();
        }
    }
}
