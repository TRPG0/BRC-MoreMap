using HarmonyLib;
using Reptile;
using System.Linq;
using UnityEngine;

namespace MoreMap.Patches
{
    [HarmonyPatch(typeof(WorldHandler), "CreateTempRacer")]
    public class WorldHandler_CreateTempRacer_Patch
    {
        public static void Postfix(Player __result, NPC linkedNPC, NPC.TempRacer tempRacerSetup, NPC.Dialogue hookedUpToNPCDialogue = null)
        {
            if (PinManager.Instance != null && linkedNPC != null)
            {
                if (PinManager.Instance.characterNPCs.Contains(linkedNPC.gameObject))
                {
                    Core.Logger.LogInfo("Creating temp racer for linked NPC!");

                    /*
                    PinManager.Instance.characterPins.Remove(PinManager.Instance.npcLinks[linkedNPC.gameObject]);
                    PinManager.Instance.RemovePin(PinManager.Instance.npcLinks[linkedNPC.gameObject]);
                    GameObject.Destroy(PinManager.Instance.npcLinks[linkedNPC.gameObject].gameObject);
                    PinManager.Instance.npcLinks.Remove(linkedNPC.gameObject);
                    */

                    MapPin pin = PinManager.Instance.MapcontrollerT.Method("CreatePin", new object[] { MapPin.PinType.GraffitiPin }).GetValue<MapPin>();
                    pin.AssignGameplayEvent(__result.gameObject);
                    __result.gameObject.AddComponent<TempRacerLink>().SetPin(pin);
                    pin.InitMapPin(MapPin.PinType.StoryObjectivePin);
                    pin.OnPinEnable();

                    foreach (MapPin cPin in PinManager.Instance.characterPins)
                    {
                        if (Traverse.Create(cPin).Field<GameObject>("m_ObjectiveObject").Value == linkedNPC.gameObject) PinManager.Instance.tempRacerLinks.Add(pin, cPin);
                    }

                    PinManager.Instance.characterPins.Add(pin);
                    PinManager.Instance.SetPinColor(pin, PinManager.Instance.GetPinColor(pin));
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorldHandler), "RemoveTempRacers")]
    public class WorldHandler_RemoveTempRacers_Patch
    {
        public static void Postfix()
        {
            if (PinManager.Instance != null)
            {
                for (int i = 0; i < PinManager.Instance.characterPins.Count; i++)
                {
                    if (Traverse.Create(PinManager.Instance.characterPins[i]).Field<GameObject>("m_ObjectiveObject").Value == null) PinManager.Instance.characterPins.RemoveAt(i);
                }
                PinManager.Instance.tempRacerLinks.Clear();
            }
        }
    }
}
