using HarmonyLib;
using Reptile.Phone;
using TMPro;
using ModLocalizer;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(HomescreenButton), "SetContent")]
    internal class HomescreenButton_SetContent_Patch
    {
        public static void Postfix(HomescreenButton __instance)
        {
            if (__instance.AssignedApp.AppName == "AppMap")
            {
                Traverse.Create(__instance).Field<TextMeshProUGUI>("m_TitleLabel").Value.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, __instance.AssignedApp.DisplayName);
            }
        }
    }
}
