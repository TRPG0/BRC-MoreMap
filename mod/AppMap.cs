using HarmonyLib;
using Reptile;
using Reptile.Phone;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;

namespace MoreMap
{
    public class AppMap : App
    {
        public static AppMap Instance { get; private set; }

        public static Color AppBlue => new Color(0.224f, 0.302f, 0.624f);
        public static Color AppOrange => new Color(0.978f, 0.424f, 0.216f);

        public static Sprite AppIcon { get; private set; } = Core.Bundle.LoadAsset<Sprite>("assets/icon.png");
        public static Sprite TrueSprite { get; private set; } = Core.Bundle.LoadAsset<Sprite>("assets/true.png");
        public static Sprite FalseSprite { get; private set; } = Core.Bundle.LoadAsset<Sprite>("assets/false.png");
        public static Sprite PinSprite { get; private set; } = Core.Bundle.LoadAsset<Sprite>("assets/pin.png");
        public static Sprite PinDisabledSprite { get; private set; } = Core.Bundle.LoadAsset<Sprite>("assets/pin_disabled.png");
        public static Sprite PinXSprite { get; private set; } = Core.Bundle.LoadAsset<Sprite>("assets/pin_x.png");

        public static Sprite TaxiSprite { get; private set; }
        public static Sprite ToiletSprite { get; private set; }
        public static Sprite CypherSprite { get; private set; }

        public TextMeshProUGUI title;
        public TextMeshProUGUI selectText;
        public TextMeshProUGUI prevText;
        public TextMeshProUGUI next1Text;
        public TextMeshProUGUI next2Text;
        public TextMeshProUGUI toggleText;
        public TextMeshProUGUI pinCount;
        public Image toggleSelectIcon;
        public Image togglePrevIcon;
        public Image toggleNext1Icon;
        public Image toggleNext2Icon;
        public Image pin;
        public Image pinX;
        public Image specialIcon;

        public enum AppMapOptions
        {
            GraffitiSpot,
            Taxi,
            Toilet,
            Cypher,
            GraffitiCollectible,
            Music,
            Outfit,
            Movestyle,
            Character,
            Polo
        }

        public static AppMapOptions CurrentOption { get; private set; } = AppMapOptions.GraffitiSpot;

        public static AppMapOptions PrevOption
        {
            get
            {
                AppMapOptions option = CurrentOption - 1;
                if (option < AppMapOptions.GraffitiSpot) option = (AppMapOptions)((int)AppMapOptions.Polo + 1 + (int)option);
                return option;
            }
        }

        public static AppMapOptions Next1Option
        {
            get
            {
                AppMapOptions option = CurrentOption + 1;
                if (option > AppMapOptions.Polo) option = (AppMapOptions)((int)option - ((int)AppMapOptions.Polo + 1));
                return option;
            }
        }

        public static AppMapOptions Next2Option
        {
            get
            {
                AppMapOptions option = CurrentOption + 2;
                if (option > AppMapOptions.Polo) option = (AppMapOptions)((int)option - ((int)AppMapOptions.Polo + 1));
                return option;
            }
        }

        public static void DoAppSetup()
        {
            if (!Reptile.Core.Instance.BaseModule.IsPlayingInStage) return;
            if (Instance != null) return;

            Phone phone = Traverse.Create(WorldHandler.instance.GetCurrentPlayer()).Field<Phone>("phone").Value;

            Instance = GameObject.Instantiate(phone.GetComponentInChildren<AppEmail>(true).transform.gameObject, phone.GetComponentInChildren<AppEmail>(true).transform.parent).AddComponent<AppMap>();
            Instance.gameObject.name = "AppMap";
            Component.DestroyImmediate(Instance.GetComponent<AppEmail>());
            Traverse appT = Traverse.Create(Instance);
            appT.Method("Awake").GetValue();
            appT.Field<Phone>("<MyPhone>k__BackingField").Value = phone;
            appT.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] {};

            GameObject.DestroyImmediate(Instance.Content.Find("EmailScroll").gameObject);
            GameObject.DestroyImmediate(Instance.Content.Find("MessagePanel").gameObject);

            Instance.Content.Find("Overlay").GetComponentInChildren<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            Component.Destroy(Instance.Content.Find("Overlay").GetComponentInChildren<TMProFontLocalizer>());
            Component.Destroy(Instance.Content.Find("Overlay").GetComponentInChildren<TMProLocalizationAddOn>());
            Instance.title = Instance.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            Instance.title.text = "Map";
            TMP_FontAsset font = Instance.title.font;

            Image icon = Instance.Content.Find("Overlay").Find("Icons").Find("AppIcon").GetComponentInChildren<Image>();
            icon.sprite = AppIcon;

            GameObject selectBackground = GameObject.Instantiate(phone.GetAppInstance<AppHomeScreen>().Content.Find("BottomView").Find("ButtonContainer").Find("Selector").Find("Background").gameObject, Instance.Content);
            selectBackground.transform.localPosition = new Vector3(100, 250, 0);
            selectBackground.name = "Selection Background";

            GameObject arrowDown = GameObject.Instantiate(phone.GetAppInstance<AppHomeScreen>().Content.Find("BottomView").Find("OtherElements").Find("ArrowsContainer").Find("ArrowDown").gameObject, Instance.Content);
            arrowDown.transform.localPosition = new Vector3(75, 50, 0);
            arrowDown.name = "Arrow Down";

            GameObject arrowUp = GameObject.Instantiate(arrowDown, Instance.Content);
            arrowUp.transform.Rotate(0, 0, 180);
            arrowUp.transform.localPosition = new Vector3(75, 450, 0);
            arrowUp.name = "Arrow Up";

            GameObject arrowRight = GameObject.Instantiate(Instance.Content.Find("Overlay").Find("Icons").Find("Arrow").gameObject, Instance.Content);
            arrowRight.transform.Rotate(0, 0, 180);
            arrowRight.transform.localPosition = new Vector3(450, -600, 0);
            arrowRight.name = "Arrow Right";

            GameObject pinBackground = GameObject.Instantiate(selectBackground, Instance.Content);
            pinBackground.transform.Rotate(0, 0, 180);
            pinBackground.transform.localPosition = new Vector3(-700, -600, 0);
            pinBackground.name = "Pin Background";

            GameObject selectText = GameObject.Instantiate(selectBackground, Instance.Content);
            Component.DestroyImmediate(selectText.GetComponent<Image>());
            selectText.transform.localPosition = new Vector3(-180, 250, 0);
            selectText.name = "Selection Text";
            Instance.selectText = selectText.AddComponent<TextMeshProUGUI>();
            Instance.selectText.alignment = TextAlignmentOptions.Right;
            Instance.selectText.fontSize = 65;
            Instance.selectText.font = font;
            Instance.selectText.text = "Graffiti Designs";

            Instance.prevText = GameObject.Instantiate(Instance.selectText.gameObject, Instance.Content).GetComponent<TextMeshProUGUI>();
            Instance.prevText.name = "Previous Text";
            Instance.prevText.color = AppBlue;
            Instance.prevText.fontSize = 45;
            Instance.prevText.transform.localPosition = new Vector3(-155, 550, 0);

            Instance.next1Text = GameObject.Instantiate(Instance.prevText.gameObject, Instance.Content).GetComponent<TextMeshProUGUI>();
            Instance.next1Text.name = "Next 1 Text";
            Instance.next1Text.transform.localPosition = new Vector3(-155, -50, 0);

            Instance.next2Text = GameObject.Instantiate(Instance.next1Text.gameObject, Instance.Content).GetComponent<TextMeshProUGUI>();
            Instance.next2Text.name = "Next 2 Text";
            Instance.next2Text.transform.localPosition = new Vector3(-155, -175, 0);

            Instance.toggleText = GameObject.Instantiate(Instance.next2Text.gameObject, Instance.Content).GetComponent<TextMeshProUGUI>();
            Instance.toggleText.name = "Toggle Text";
            Instance.toggleText.color = AppOrange;
            Instance.toggleText.fontSize = 75;
            Instance.toggleText.text = "Disable";
            Instance.toggleText.transform.localPosition = new Vector3(-175, -600, 0);

            Instance.pinCount = GameObject.Instantiate(Instance.toggleText.gameObject, Instance.Content).GetComponent<TextMeshProUGUI>();
            Instance.pinCount.name = "Pin Count";
            Instance.pinCount.color = Color.white;
            Instance.pinCount.fontSize = 60;
            Instance.pinCount.transform.localPosition = new Vector3(-175, -700, 0);

            GameObject isActive = new GameObject()
            {
                name = "Selection IsActive",
                layer = Layers.Phone
            };
            isActive.transform.SetParent(Instance.Content);
            Instance.toggleSelectIcon = isActive.AddComponent<Image>();
            Instance.toggleSelectIcon.sprite = TrueSprite;
            Instance.toggleSelectIcon.transform.localScale = Vector3.one;
            Instance.toggleSelectIcon.transform.localPosition = new Vector3(450, 250, 0);

            Instance.togglePrevIcon = GameObject.Instantiate(Instance.toggleSelectIcon.gameObject, Instance.Content).GetComponent<Image>();
            Instance.togglePrevIcon.name = "Previous Icon";
            Instance.togglePrevIcon.color = AppBlue;
            Instance.togglePrevIcon.transform.localPosition = new Vector3(460, 550, 0);
            Instance.togglePrevIcon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

            Instance.toggleNext1Icon = GameObject.Instantiate(Instance.togglePrevIcon.gameObject, Instance.Content).GetComponent<Image>();
            Instance.toggleNext1Icon.name = "Next 1 Icon";
            Instance.toggleNext1Icon.transform.localPosition = new Vector3(460, -50, 0);

            Instance.toggleNext2Icon = GameObject.Instantiate(Instance.togglePrevIcon.gameObject, Instance.Content).GetComponent<Image>();
            Instance.toggleNext2Icon.name = "Next 2 Icon";
            Instance.toggleNext2Icon.transform.localPosition = new Vector3(460, -175, 0);

            GameObject pin = new GameObject()
            {
                name = "Pin",
                layer = Layers.Phone
            };
            pin.transform.SetParent(Instance.Content);
            Instance.pin = pin.AddComponent<Image>();
            Instance.pin.sprite = PinSprite;
            Instance.pin.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 256);
            Instance.pin.transform.localScale = new Vector3(2, 2, 1);
            Instance.pin.transform.localPosition = new Vector3(-325, -350, 0);

            Instance.specialIcon = GameObject.Instantiate(Instance.pin.gameObject, Instance.Content).GetComponent<Image>();
            Instance.specialIcon.name = "Special Icon";
            Instance.specialIcon.transform.localPosition = Instance.pin.transform.localPosition;
            Instance.specialIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 128);

            Instance.pinX = GameObject.Instantiate(Instance.pin.gameObject, Instance.Content).GetComponent<Image>();
            Instance.pinX.name = "PinX";
            Instance.pinX.transform.localPosition = Instance.pin.transform.localPosition;
            Instance.pinX.sprite = PinXSprite;
            Instance.pinX.gameObject.SetActive(false);

            HomeScreenApp hsa = ScriptableObject.CreateInstance<HomeScreenApp>();
            hsa.name = "AppMap";
            Traverse hsaT = Traverse.Create(hsa);
            hsaT.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsaT.Field<string>("m_AppName").Value = "AppMap";
            hsaT.Field<string>("m_DisplayName").Value = "MAP";
            hsaT.Field<Sprite>("m_AppIcon").Value = AppIcon;

            Traverse.Create(phone).Field<Dictionary<string, App>>("<AppInstances>k__BackingField").Value.Add("AppMap", Instance);
            Traverse.Create(phone.GetAppInstance<AppHomeScreen>()).Method("AddApp", new object[] { hsa }).GetValue();
        }

        public void LoadMinimapSprites()
        {
            TaxiSprite = Sprite.Create(Reptile.Core.Instance.Assets.LoadAssetFromBundle<Texture2D>("minimap", "assets/games/phone/appassets/minimap/taxipin.png"), new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            ToiletSprite = Sprite.Create(Reptile.Core.Instance.Assets.LoadAssetFromBundle<Texture2D>("minimap", "assets/games/phone/appassets/minimap/toiletpin.png"), new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            CypherSprite = Sprite.Create(Reptile.Core.Instance.Assets.LoadAssetFromBundle<Texture2D>("minimap", "assets/games/phone/appassets/minimap/cypherpin.png"), new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        }

        private string GetOptionName(AppMapOptions option)
        {
            return option switch
            {
                AppMapOptions.GraffitiSpot => "Graffiti Spots",
                AppMapOptions.Taxi => "Taxi Spots",
                AppMapOptions.Toilet => "Toilets",
                AppMapOptions.Cypher => "Cypher Spots",
                AppMapOptions.GraffitiCollectible => "Graffiti Designs",
                AppMapOptions.Music => "Music CDs",
                AppMapOptions.Outfit => "Outfits",
                AppMapOptions.Movestyle => "Movestyles",
                AppMapOptions.Character => "Characters",
                AppMapOptions.Polo => "Polo Photos",
                _ => "?"
            };
        }

        private Color GetOptionColor(AppMapOptions option)
        {
            return option switch
            {
                AppMapOptions.GraffitiSpot => Core.colorGraffitiSpot.Value,
                AppMapOptions.GraffitiCollectible => Core.colorGraffitiCollectible.Value,
                AppMapOptions.Music => Core.colorMusic.Value,
                AppMapOptions.Outfit => Core.colorOutfit.Value,
                AppMapOptions.Movestyle => Core.colorMovestyle.Value,
                AppMapOptions.Character => Core.colorCharacter.Value,
                AppMapOptions.Polo => Core.colorPolo.Value,
                _ => Color.white
            };
        }

        private bool OptionValue(AppMapOptions option, bool? value = null)
        {
            switch (option)
            {
                case AppMapOptions.GraffitiSpot:
                    if (value.HasValue) Core.showGraffitiSpot.Value = value.Value;
                    return Core.showGraffitiSpot.Value;
                case AppMapOptions.Taxi:
                    if (value.HasValue) Core.showTaxi.Value = value.Value;
                    return Core.showTaxi.Value;
                case AppMapOptions.Toilet:
                    if (value.HasValue) Core.showToilet.Value = value.Value;
                    return Core.showToilet.Value;
                case AppMapOptions.Cypher:
                    if (value.HasValue) Core.showCypher.Value = value.Value;
                    return Core.showCypher.Value;
                case AppMapOptions.GraffitiCollectible:
                    if (value.HasValue) Core.showGraffitiCollectible.Value = value.Value;
                    return Core.showGraffitiCollectible.Value;
                case AppMapOptions.Music:
                    if (value.HasValue) Core.showMusic.Value = value.Value;
                    return Core.showMusic.Value;
                case AppMapOptions.Outfit:
                    if (value.HasValue) Core.showOutfit.Value = value.Value;
                    return Core.showOutfit.Value;
                case AppMapOptions.Movestyle:
                    if (value.HasValue) Core.showMovestyle.Value = value.Value;
                    return Core.showMovestyle.Value;
                case AppMapOptions.Character:
                    if (value.HasValue) Core.showCharacter.Value = value.Value;
                    return Core.showCharacter.Value;
                case AppMapOptions.Polo:
                    if (value.HasValue) Core.showPolo.Value = value.Value;
                    return Core.showPolo.Value;
            }
            return true;
        }

        public override void Awake()
        {
            base.Awake();
            LoadMinimapSprites();
        }

        public override void OnAppUpdate()
        {
            base.OnAppUpdate();
            UpdateCount();
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            HandleInput = true;
            UpdateText();
            UpdateImages();
        }

        public override void OnAppDisable()
        {
            base.OnAppDisable();
            HandleInput = false;
        }

        public override void OnReleaseLeft()
        {
            base.OnReleaseLeft();
            MyPhone.CloseCurrentApp();
        }

        public override void OnReleaseUp()
        {
            PlaySfx(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
            CurrentOption -= 1;
            if (CurrentOption < AppMapOptions.GraffitiSpot) CurrentOption = AppMapOptions.Polo;

            UpdateText();
            UpdateImages();
        }

        public override void OnReleaseDown()
        {
            PlaySfx(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
            CurrentOption += 1;
            if (CurrentOption > AppMapOptions.Polo) CurrentOption = AppMapOptions.GraffitiSpot;

            UpdateText();
            UpdateImages();
        }

        public override void OnReleaseRight()
        {
            PlaySfx(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
            OptionValue(CurrentOption, !OptionValue(CurrentOption));

            if (CurrentOption == AppMapOptions.Polo && OptionValue(AppMapOptions.Polo)) PinManager.Instance.EnablePhotoPins();
            else PinManager.Instance.SetPinsEnabled(GetPinsForOption(CurrentOption), OptionValue(CurrentOption));

            UpdateText();
            UpdateImages();
        }

        public void UpdateText()
        {
            selectText.text = GetOptionName(CurrentOption);
            prevText.text = GetOptionName(PrevOption);
            next1Text.text = GetOptionName(Next1Option);
            next2Text.text = GetOptionName(Next2Option);
            toggleText.text = OptionValue(CurrentOption) ? "Disable" : "Enable";
        }

        public void UpdateCount()
        {
            if (OptionValue(CurrentOption))
            {
                int activePins = PinManager.Instance.CountActivePins(GetPinsForOption(CurrentOption));
                pinCount.text = activePins == 1 ? $"{activePins} pin" : $"{activePins} pins";
                pinCount.gameObject.SetActive(true);
            }
            else pinCount.gameObject.SetActive(false);
        }

        public void UpdateImages()
        {
            if (CurrentOption == AppMapOptions.Taxi || CurrentOption == AppMapOptions.Toilet || CurrentOption == AppMapOptions.Cypher)
            {
                pin.gameObject.SetActive(false);
                specialIcon.gameObject.SetActive(true);

                if (CurrentOption == AppMapOptions.Taxi) specialIcon.sprite = TaxiSprite;
                else if (CurrentOption == AppMapOptions.Toilet) specialIcon.sprite = ToiletSprite;
                else if (CurrentOption == AppMapOptions.Cypher) specialIcon.sprite = CypherSprite;

                specialIcon.color = OptionValue(CurrentOption) ? Color.white : new Color(1, 1, 1, 0.5f);
                pinX.gameObject.SetActive(!OptionValue(CurrentOption));
            }
            else
            {
                specialIcon.gameObject.SetActive(false);
                pin.gameObject.SetActive(true);
                pin.sprite = OptionValue(CurrentOption) ? PinSprite : PinDisabledSprite;
                pin.color = GetOptionColor(CurrentOption);
                pinX.gameObject.SetActive(!OptionValue(CurrentOption));
            }

            toggleSelectIcon.sprite = OptionValue(CurrentOption) ? TrueSprite : FalseSprite;
            togglePrevIcon.sprite = OptionValue(PrevOption) ? TrueSprite : FalseSprite;
            toggleNext1Icon.sprite = OptionValue(Next1Option) ? TrueSprite : FalseSprite;
            toggleNext2Icon.sprite = OptionValue(Next2Option) ? TrueSprite : FalseSprite;
        }

        public List<MapPin> GetPinsForOption(AppMapOptions option)
        {
            return option switch
            {
                AppMapOptions.GraffitiSpot => PinManager.Instance.graffitiSpotPins,
                AppMapOptions.Taxi => PinManager.Instance.taxiPins,
                AppMapOptions.Toilet => PinManager.Instance.toiletPins,
                AppMapOptions.Cypher => PinManager.Instance.cypherPins,
                AppMapOptions.GraffitiCollectible => PinManager.Instance.graffitiCollectiblePins,
                AppMapOptions.Music => PinManager.Instance.musicPins,
                AppMapOptions.Outfit => PinManager.Instance.outfitPins,
                AppMapOptions.Movestyle => PinManager.Instance.movestylePins,
                AppMapOptions.Character => PinManager.Instance.characterPins,
                AppMapOptions.Polo => PinManager.Instance.poloPins,
                _ => null
            };
        }

        public void PlaySfx(SfxCollectionID collectionId, AudioClipID audioClipId)
        {
            Traverse.Create(Reptile.Core.Instance.AudioManager).Method("PlaySfxGameplay", new object[] { collectionId, audioClipId, 0f }).GetValue();
        }
    }
}
