using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MoreMap
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Core : BaseUnityPlugin
    {
        public const string PluginGUID = "trpg.brc.moremap";
        public const string PluginName = "MoreMap";
        public const string PluginVersion = "1.0.2";

        public static Core Instance { get; private set; }
        public static new ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("MoreMap");
        public static AssetBundle Bundle { get; private set; } = AssetBundle.LoadFromMemory(Properties.Resources.moremap);

        public static ConfigEntry<bool> requireMap;
        public static ConfigEntry<bool> showGraffitiSpot;
        public static ConfigEntry<bool> showTaxi;
        public static ConfigEntry<bool> showToilet;
        public static ConfigEntry<bool> showCypher;
        public static ConfigEntry<bool> showGraffitiCollectible;
        public static ConfigEntry<bool> showMusic;
        public static ConfigEntry<bool> showOutfit;
        public static ConfigEntry<bool> showMovestyle;
        public static ConfigEntry<bool> showCharacter;
        public static ConfigEntry<bool> showPolo;

        public static ConfigEntry<Color> colorGraffitiSpot;
        public static ConfigEntry<Color> colorGraffitiCollectible;
        public static ConfigEntry<Color> colorMusic;
        public static ConfigEntry<Color> colorOutfit;
        public static ConfigEntry<Color> colorMovestyle;
        public static ConfigEntry<Color> colorCharacter;
        public static ConfigEntry<Color> colorPolo;

        private void Awake()
        {
            Instance = this;

            Harmony harmony = new Harmony("MoreMap");
            harmony.PatchAll();

            StageManager.OnStagePostInitialization += () =>
            {
                bool mapFound = Reptile.Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().mapFound;
                Core.Logger.LogInfo($"Current stage: {Reptile.Core.Instance.BaseModule.CurrentStage} | Map found? {mapFound}");
                if (mapFound || !requireMap.Value) DoMapSetup();
            };

            requireMap = Config.Bind("Toggles",
                "Require map",
                true,
                "Whether the map must be collected before the custom pins will appear.");

            showGraffitiSpot = Config.Bind("Toggles",
                "Show Graffiti Spot pins",
                true);

            showTaxi = Config.Bind("Toggles",
                "Show Taxi pins",
                true);

            showToilet = Config.Bind("Toggles",
                "Show Toilet pins",
                true);

            showCypher = Config.Bind("Toggles",
                "Show Cypher Spot pins",
                true);

            showGraffitiCollectible = Config.Bind("Toggles",
                "Show Graffiti Collectible pins",
                true);

            showMusic = Config.Bind("Toggles",
                "Show Music CD pins",
                true);

            showOutfit = Config.Bind("Toggles",
                "Show Outfit pins",
                true);

            showMovestyle = Config.Bind("Toggles",
                "Show Movestyle pins",
                true);

            showCharacter = Config.Bind("Toggles",
                "Show Character pins",
                true);

            showPolo = Config.Bind("Toggles",
                "Show Polo pins",
                true);

            colorGraffitiSpot = Config.Bind("Colors",
                "Graffiti Spot pin color",
                new Color(0.956f, 0.827f, 0.258f));

            colorGraffitiCollectible = Config.Bind("Colors",
                "Graffiti Collectible pin color",
                new Color(0.953f, 0.259f, 0.259f));

            colorMusic = Config.Bind("Colors",
                "Music CD pin color",
                new Color(0.259f, 0.608f, 0.953f));

            colorOutfit = Config.Bind("Colors",
                "Outfit pin color",
                new Color(0.961f, 0.961f, 0.961f));

            colorMovestyle = Config.Bind("Colors",
                "Movestyle pin color",
                new Color(0.259f, 0.953f, 0.375f));

            colorCharacter = Config.Bind("Colors",
                "Character pin color",
                new Color(0.956f, 0.321f, 0.7339f));

            colorPolo = Config.Bind("Colors",
                "Polo pin color",
                new Color(0.259f, 0.259f, 0.957f));
        }

        public void DoMapSetup()
        {
            Mapcontroller.Instance.gameObject.AddComponent<PinManager>();
            AppMap.DoAppSetup();
        }

        public void DelayMapSetup(float seconds)
        {
            Invoke("DoMapSetup", seconds);
        }
    }
}
