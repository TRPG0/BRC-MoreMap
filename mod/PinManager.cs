using System.Collections.Generic;
using UnityEngine;
using Reptile;
using HarmonyLib;
using System.Linq;
using Reptile.Phone;

namespace MoreMap
{
    public class PinManager : MonoBehaviour
    {
        public static PinManager Instance { get; private set; }
        public Mapcontroller Mapcontroller;
        public Traverse MapcontrollerT;

        public List<MapPin> objectivePins = new List<MapPin>();
        public List<MapPin> graffitiSpotPins = new List<MapPin>();
        public List<MapPin> taxiPins = new List<MapPin>();
        public List<MapPin> toiletPins = new List<MapPin>();
        public List<MapPin> cypherPins = new List<MapPin>();
        public List<MapPin> graffitiCollectiblePins = new List<MapPin>();
        public List<MapPin> musicPins = new List<MapPin>();
        public List<MapPin> outfitPins = new List<MapPin>();
        public List<MapPin> movestylePins = new List<MapPin>();
        public List<MapPin> characterPins = new List<MapPin>();
        public List<MapPin> poloPins = new List<MapPin>();

        public Dictionary<VendingMachine, MapPin> vendingMachineLinks = new Dictionary<VendingMachine, MapPin>();
        public List<GameObject> characterNPCs = new List<GameObject>();
        public Dictionary<MapPin, MapPin> tempRacerLinks = new Dictionary<MapPin, MapPin>();
        public Dictionary<PhotoObjectiveProgressable, MapPin> poloLinks = new Dictionary<PhotoObjectiveProgressable, MapPin>();

        public void Start()
        {
            if (Instance != null && Instance != this) return;
            Instance = this;
            Mapcontroller = GetComponent<Mapcontroller>();
            MapcontrollerT = Traverse.Create(Mapcontroller);

            FindAllPins();
        }

        public void Update()
        {
            if (Core.showGraffitiCollectible.Value) UpdatePins(graffitiCollectiblePins);
            if (Core.showCharacter.Value)
            {
                UpdateCharacterPins();
                UpdateTempRacerPins();
            }
        }

        public void FindAllPins()
        {
            GetExistingPins();
            CreatePinsFromProgressables();
            CreateCharacterPins();
            CreateVendingMachinePins();
            DisableAllPins(graffitiCollectiblePins);
            if (Core.showGraffitiCollectible.Value) EnableAllPins(graffitiCollectiblePins);
            DisableAllPins(musicPins);
            if (Core.showMusic.Value) EnableAllPins(musicPins);
            DisableAllPins(outfitPins);
            if (Core.showOutfit.Value) EnableAllPins(outfitPins);
            DisableAllPins(movestylePins);
            if (Core.showMovestyle.Value) EnableAllPins(movestylePins);
            DisableAllPins(poloPins);
            if (Core.showPolo.Value) EnablePhotoPins();
            Core.Logger.LogInfo($"Finished pin setup.");
        }

        public void GetExistingPins()
        {
            foreach (MapPin pin in MapcontrollerT.Field<List<MapPin>>("m_MapPins").Value)
            {
                Traverse pinT = Traverse.Create(pin);
                switch (pinT.Field<MapPin.PinType>("m_pinType").Value)
                {
                    case MapPin.PinType.GraffitiPin:
                        graffitiSpotPins.Add(pin);
                        break;
                    case MapPin.PinType.StoryObjectivePin:
                        objectivePins.Add(pin);
                        break;
                    case MapPin.PinType.TaxiPin:
                        taxiPins.Add(pin);
                        break;
                    case MapPin.PinType.ToiletPin:
                        toiletPins.Add(pin);
                        break;
                    case MapPin.PinType.CypherPin:
                        cypherPins.Add(pin);
                        break;
                    default:
                        break;
                }
                SetPinColor(pin, GetPinColor(pin));
            }
        }

        public void CreatePinsFromProgressables()
        {
            foreach (AProgressable progressable in WorldHandler.instance.SceneObjectsRegister.progressables)
            {
                if (progressable is Collectable || progressable is PhotoObjectiveProgressable)
                {
                    Traverse progressableT = Traverse.Create(progressable);
                    MapPin pin = MapcontrollerT.Method("CreatePin", new object[] { MapPin.PinType.GraffitiPin }).GetValue<MapPin>();
                    pin.AssignGameplayEvent(progressable.gameObject);

                    if (progressable is PhotoObjectiveProgressable) pin.InitMapPin(MapPin.PinType.Pin);
                    else pin.InitMapPin(MapPin.PinType.StoryObjectivePin);

                    pin.OnPinEnable();

                    //if (Traverse.Create(collectable).Field<bool>("pickedUp").Value) pin
                    if (progressable is Collectable)
                    {
                        switch (progressableT.Field<Pickup.PickUpType>("pickUpType").Value)
                        {
                            case Pickup.PickUpType.MUSIC_UNLOCKABLE:
                                musicPins.Add(pin);
                                break;
                            case Pickup.PickUpType.GRAFFITI_UNLOCKABLE:
                                graffitiCollectiblePins.Add(pin);
                                break;
                            case Pickup.PickUpType.OUTFIT_UNLOCKABLE:
                                outfitPins.Add(pin);
                                break;
                            case Pickup.PickUpType.MOVESTYLE_SKIN_UNLOCKABLE:
                                movestylePins.Add(pin);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (progressable is PhotoObjectiveProgressable photo)
                    {
                        poloPins.Add(pin);
                        poloLinks.Add(photo, pin);
                    }
                    SetPinColor(pin, GetPinColor(pin));
                }
            }
        }

        public void CreateCharacterPins()
        {
            Dictionary<Stage, List<string>> stageNPCs = new Dictionary<Stage, List<string>>()
            {
                { Stage.downhill, new List<string>() { "NPC_Rave1", "NPC_Rave2", "NPC_Rave3", "NPC_Frank_UnlockChallenge", "NPC_Irene" } },
                { Stage.square, new List<string>() { "NPC_DJ_UnlockChallenge" } },
                { Stage.tower, new List<string>() { "Graffiti_Challenge/NPC_rogue", "Score_Challenge /NPC_crew", "WaterChallenge/NPC_rogue", "WaterChallenge/NPC_rogue (1)", "Chapter6/NPC_Eclipe_UnlockChallenge" } },
                { Stage.Mall, new List<string>() { "ShineNPC1", "ShineNPC2", "ShineNPC3", "ShineNPC4", "NPC_DOTEXE_UnlockChallenge" } },
                { Stage.pyramid, new List<string>() { "NPC_Rogue_PufferGirl (1)", "NPC_Rogue_PufferGirl_Start", "NPC_DevilTheory_UnlockChallenge" } },
                { Stage.osaka, new List<string>() { "NPC_coil1", "NPC_coil2", "NPC_coil3", "NPC_coil4", "NPC_Futurism_UnlockChallenge", "NPC_FlesPrince_UnlockDance" } }
            };

            List<NPC> NPCs = new List<NPC>();

            Stage currentStage = Reptile.Core.Instance.BaseModule.CurrentStage;
            if (!stageNPCs.ContainsKey(currentStage)) return;
            foreach (NPC npc in WorldHandler.instance.SceneObjectsRegister.NPCs)
            {
                if (currentStage == Stage.tower)
                {
                    if (npc.transform.parent == null) continue;
                    if (stageNPCs[currentStage].Contains($"{npc.transform.parent.name}/{npc.name}")) NPCs.Add(npc);
                }
                else
                {
                    if (stageNPCs[currentStage].Contains(npc.name)) NPCs.Add(npc);
                }
            }

            foreach (NPC npc in NPCs)
            {
                characterNPCs.Add(npc.gameObject);
                MapPin pin = MapcontrollerT.Method("CreatePin", new object[] { MapPin.PinType.GraffitiPin }).GetValue<MapPin>();
                MapPin pin2 = null;

                if (Traverse.Create(npc).Field<bool>("activeTempRacerWithDialogue").Value)
                {
                    foreach (Player player in WorldHandler.instance.SceneObjectsRegister.players)
                    {
                        Traverse playerT = Traverse.Create(player);
                        if (playerT.Field<PlayerType>("playerType").Value == PlayerType.TEMP_RACER_AI && playerT.Field<PlayerAI>("AI").Value.linkedNPC == npc)
                        {
                            Core.Logger.LogInfo("Creating temp racer for linked NPC!");
                            pin2 = MapcontrollerT.Method("CreatePin", new object[] { MapPin.PinType.GraffitiPin }).GetValue<MapPin>();
                            pin2.AssignGameplayEvent(player.gameObject);
                            player.gameObject.AddComponent<TempRacerLink>().SetPin(pin2);
                            pin2.InitMapPin(MapPin.PinType.StoryObjectivePin);
                            pin2.OnPinEnable();
                            characterPins.Add(pin2);
                            SetPinColor(pin2, GetPinColor(pin2));
                            break;
                        }
                    }
                }
                
                pin.AssignGameplayEvent(npc.gameObject);
                pin.InitMapPin(MapPin.PinType.StoryObjectivePin);
                pin.OnPinEnable();
                characterPins.Add(pin);
                SetPinColor(pin, GetPinColor(pin));
                if (pin2 != null) tempRacerLinks.Add(pin2, pin);
            }
        }

        public void CreateVendingMachinePins()
        {
            foreach (VendingMachine vm in Object.FindObjectsOfType<VendingMachine>(true))
            {
                if (vm.unlockableDrop != null)
                {
                    MapPin pin = MapcontrollerT.Method("CreatePin", new object[] { MapPin.PinType.GraffitiPin }).GetValue<MapPin>();
                    pin.AssignGameplayEvent(vm.gameObject);
                    pin.InitMapPin(MapPin.PinType.Pin);
                    pin.OnPinEnable();

                    AUnlockable unlockable = vm.unlockableDrop.GetComponent<DynamicPickup>().unlock;
                    if (unlockable is MusicTrack) musicPins.Add(pin);
                    else if (unlockable is GraffitiAppEntry) graffitiCollectiblePins.Add(pin);
                    else if (unlockable is OutfitUnlockable) outfitPins.Add(pin);
                    else if (unlockable is MoveStyleSkin) movestylePins.Add(pin);

                    SetPinColor(pin, GetPinColor(pin));
                    vendingMachineLinks.Add(vm, pin);
                }
            }
        }

        public void RemovePin(MapPin pin)
        {
            List<MapPin> pins = MapcontrollerT.Field<List<MapPin>>("m_MapPins").Value;
            if (pins.Contains(pin)) pins.Remove(pin);
            MapcontrollerT.Field<List<MapPin>>("m_MapPins").Value = pins;
        }

        public void UpdatePins(List<MapPin> list)
        {
            foreach (MapPin pin in list)
            {
                Traverse traverse = Traverse.Create(pin);

                if (traverse.Field<GameObject>("m_ObjectiveObject").Value.activeInHierarchy)
                {
                    pin.UpdateLocation();
                    traverse.Method("EnableMapPinGameObject").GetValue();
                }
                else traverse.Method("DisableMapPinGameObject").GetValue();
            }
        }

        public void UpdateCharacterPins()
        {
            foreach (MapPin pin in characterPins)
            {
                if (tempRacerLinks.ContainsKey(pin) || tempRacerLinks.ContainsValue(pin)) continue;

                Traverse traverse = Traverse.Create(pin);

                if (traverse.Field<GameObject>("m_ObjectiveObject").Value.activeInHierarchy)
                {
                    pin.UpdateLocation();
                    traverse.Method("EnableMapPinGameObject").GetValue();
                }
                else traverse.Method("DisableMapPinGameObject").GetValue();
            }
        }

        public void UpdateTempRacerPins()
        {
            foreach(MapPin pin in tempRacerLinks.Keys)
            {
                Traverse traverse = Traverse.Create(pin);
                Traverse traverse2 = Traverse.Create(tempRacerLinks[pin]);
                if (!traverse.Field<GameObject>("m_ObjectiveObject").Value.activeInHierarchy && !traverse2.Field<GameObject>("m_ObjectiveObject").Value.activeInHierarchy)
                {
                    traverse.Method("DisableMapPinGameObject").GetValue();
                    traverse2.Method("DisableMapPinGameObject").GetValue();
                }
                else if (traverse.Field<GameObject>("m_ObjectiveObject").Value.activeInHierarchy)
                {
                    pin.UpdateLocation();
                    traverse.Method("EnableMapPinGameObject").GetValue();
                    traverse2.Method("DisableMapPinGameObject").GetValue();
                }
                else
                {
                    tempRacerLinks[pin].UpdateLocation();
                    traverse2.Method("EnableMapPinGameObject").GetValue();
                    traverse.Method("DisableMapPinGameObject").GetValue();
                }
            }
        }

        public void SetPinsEnabled(List<MapPin> list, bool value)
        {
            if (value) EnableAllPins(list);
            else DisableAllPins(list);
        }

        public void EnableAllPins(List<MapPin> list)
        {
            foreach (MapPin pin in list)
            {
                Traverse traverse = Traverse.Create(pin);
                if (vendingMachineLinks.ContainsValue(pin))
                {
                    VendingMachine vm = vendingMachineLinks.FirstOrDefault(x => x.Value == pin).Key;
                    for (int i = 0; i < vm.rewards.Length; i++)
                    {
                        if (vm.rewards[i] == VendingMachine.Reward.UNLOCKABLE_DROP)
                        {
                            if (vm.RewardIsValid(i)) traverse.Method("EnableMapPinGameObject").GetValue();
                            else traverse.Method("DisableMapPinGameObject").GetValue();
                        }
                        else traverse.Method("DisableMapPinGameObject").GetValue();
                    }
                }
                else traverse.Method("EnableMapPinGameObject").GetValue();
            }
        }

        public void DisableAllPins(List<MapPin> list)
        {
            foreach (MapPin pin in list)
            {
                Traverse.Create(pin).Method("DisableMapPinGameObject").GetValue();
            }
        }

        public int CountActivePins(List<MapPin> list)
        {
            int count = 0;
            foreach (MapPin pin in list)
            {
                if (Traverse.Create(pin).Field<bool>("isMapPinActive").Value) count++;
            }
            return count;
        }

        public void EnablePhotoPins()
        {
            foreach (PhotoObjectiveProgressable photo in poloLinks.Keys)
            {
                if (!photo.HasPhotoGraphedObject) Traverse.Create(poloLinks[photo]).Method("EnableMapPinGameObject").GetValue();
            }
        }

        public bool CanColorThisPin(MapPin pin)
        {
            if (objectivePins.Contains(pin) || taxiPins.Contains(pin) || toiletPins.Contains(pin) || cypherPins.Contains(pin)) return false;
            return true;
        }

        public Color GetPinColor(MapPin pin)
        {
            if (graffitiSpotPins.Contains(pin)) return Core.colorGraffitiSpot.Value;
            if (graffitiCollectiblePins.Contains(pin)) return Core.colorGraffitiCollectible.Value;
            if (musicPins.Contains(pin)) return Core.colorMusic.Value;
            if (outfitPins.Contains(pin)) return Core.colorOutfit.Value;
            if (movestylePins.Contains(pin)) return Core.colorMovestyle.Value;
            if (characterPins.Contains(pin)) return Core.colorCharacter.Value;
            if (poloPins.Contains(pin)) return Core.colorPolo.Value;
            return (Color)Core.colorGraffitiSpot.DefaultValue;
        }

        public void SetPinColor(MapPin pin, Color color)
        {
            if (!CanColorThisPin(pin)) return;
            pin.transform.Find("Cube").GetComponent<MeshRenderer>().material.SetColor("_TintColor", color);
            pin.GetComponent<LineRenderer>().material.color = color;
        }
    }
}
