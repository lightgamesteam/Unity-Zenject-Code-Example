using DG.Tweening;
using System.Collections.Generic;
using TDL.Models;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Grids;
using Zenject;

namespace TDL.Views
{
    public class LoginWebViewPC : LoginViewPC
    {
        [Inject] private LocalizationModel _localization;
        
        [System.Serializable]
        public struct ButtonUrl
        {
            public Button button;
            public string url;
            public bool haveDifferentLocales;

            public string enLocaleEnd;
        }

        [System.Serializable]
        public class ButtonLanguage
        {
            public Button button;
            public TextMeshProUGUI tile;
            public string culture;
        }

        [SerializeField] private RectTransform rootLayout;

        [Header("LoginWeb:")]
        [SerializeField] protected Transform container;
        [SerializeField] protected MainScreenGridAdapter gridAdapter;

        [Header("TextMenu:")]
        [SerializeField] private TextMeshProUGUI textHome;
        [SerializeField] private TextMeshProUGUI textVr;
        [SerializeField] private TextMeshProUGUI textAr;
        [SerializeField] private TextMeshProUGUI textDownload;
        [SerializeField] private TextMeshProUGUI textRequestDemo;
        [SerializeField] private TextMeshProUGUI textAboutAs;
        [SerializeField] private TextMeshProUGUI textLanguage;

        [Header("TextContextMenu1:")]
        [SerializeField] private TextMeshProUGUI textCamp3d;
        [SerializeField] private TextMeshProUGUI textAccessoriesForVr;

        [Header("TextContextMenu2:")]
        [SerializeField] private TextMeshProUGUI textAbout3dl;
        [SerializeField] private TextMeshProUGUI textPrivacy;
        [SerializeField] private TextMeshProUGUI textContactUs;

        [Header("TextService:")]
        [SerializeField] private TextMeshProUGUI textMicrosoft;
        [SerializeField] private TextMeshProUGUI textMacAppStore;
        [SerializeField] private TextMeshProUGUI textAppStore;
        [SerializeField] private TextMeshProUGUI textGooglePlay;

        [Header("Counts:")]
        [SerializeField] private float countTweenDuration = 2.5f;
        [SerializeField] private int count1Value = 900;
        [SerializeField] private int count2Value = 200;
        [SerializeField] private TextMeshProUGUI textCount1;
        [SerializeField] private TextMeshProUGUI textCount2;
        [SerializeField] private TextMeshProUGUI textCountTitle1;
        [SerializeField] private TextMeshProUGUI textCountTitle2;

        [Header("Text Buttons:")]
        [SerializeField] private TextMeshProUGUI textButtonContacktUs;

        [Header("Language:")]
        [SerializeField] private ButtonLanguage[] btnLanguages;

        [Space(10)]
        [SerializeField] protected ButtonUrl[] menuButtons;

        private bool isInitialize = false;

        public override void InitUiComponents()
        {
            base.InitUiComponents();
            if (!isInitialize)
            {
                isInitialize = true;
                SetupMenuButtonsHandler();
                SaveContentPanels();
                TweenCounts();
            }
        }

        private void TweenCounts()
        {
            TweenCount(textCount1, count1Value);
            TweenCount(textCount2, count2Value);
        }

        private void TweenCount(TextMeshProUGUI textCount, int targetValue)
        {
            float count = 0;
            DOTween.To(() => count, x => count = x, targetValue, countTweenDuration)
                .OnUpdate(() => {
                    textCount.text = $"{(int)count}+";
                });
        }

        private void SaveContentPanels()
        {
            Signal.Fire(new SaveMainScreenContentPanelsCommandSignal(container, gridAdapter));
        }

        private void OpenUrl(string url)
        {
            Signal.Fire(new OpenUrlCommandSignal(url));
        }

        private void SetupMenuButtonsHandler()
        {
            foreach (var item in menuButtons)
            {
                if (item.button == null)
                {
                    continue;
                }
                item.button.onClick.AddListener(() =>
                {
                    var url = item.url;
                    if (item.haveDifferentLocales)
                    {
                        if (_localization.CurrentLanguageCultureCode == "en-US")
                        {
                            url += item.enLocaleEnd;
                        }
                    }
                    OpenUrl(url);
                });
            }
        }

        /*public ButtonLanguage FindLanguage(string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return null;
            }
            foreach (var item in btnLanguages)
            {
                if (item == null)
                {
                    continue;
                }
                if (item.culture == culture)
                {
                    return item;
                }
            }

            return null;
        }

        public void HideLanguageButtons()
        {
            foreach (var item in btnLanguages)
            {
                if (item == null)
                {
                    continue;
                }
                if (item.button == null)
                {
                    continue;
                }
                item.button.gameObject.SetActive(false);
            }
        }*/

        /*public void ShowFirstLanguageButtons()
        {
            ShowLanguageButtons(btnLanguages[0]);
        }

        public void ShowLanguageButtons(ButtonLanguage obj)
        {
            if (obj == null || obj.button == null)
            {
                return;
            }
            obj.button.gameObject.SetActive(true);
        }

        public void ShowNextLanguageButtons(ButtonLanguage obj)
        {
            foreach (var item in btnLanguages)
            {
                if (item == null)
                {
                    continue;
                }
                if (item == obj)
                {
                    continue;
                }
                if (item.button == null)
                {
                    continue;
                }
                item.button.gameObject.SetActive(true);
            }
        }*/

        public RectTransform RootLayout { get => rootLayout; }
        
        public TextMeshProUGUI TextHome { get => textHome; }
        public TextMeshProUGUI TextVr { get => textVr; }
        public TextMeshProUGUI TextAr { get => textAr; }
        public TextMeshProUGUI TextDownload { get => textDownload; }
        public TextMeshProUGUI TextRequestDemo { get => textRequestDemo; }
        public TextMeshProUGUI TextAboutAs { get => textAboutAs; }
        public TextMeshProUGUI TextLanguage { get => textLanguage; }
        public TextMeshProUGUI TextCamp3d { get => textCamp3d; }
        public TextMeshProUGUI TextAccessoriesForVr { get => textAccessoriesForVr; }
        public TextMeshProUGUI TextAbout3dl { get => textAbout3dl; }
        public TextMeshProUGUI TextPrivacy { get => textPrivacy; }
        public TextMeshProUGUI TextContactUs { get => textContactUs; }
        public TextMeshProUGUI TextButtonContacktUs { get => textButtonContacktUs; }
        public TextMeshProUGUI TextCount1 { get => textCount1; }
        public TextMeshProUGUI TextCount2 { get => textCount2; }
        public TextMeshProUGUI TextCountTitle1 { get => textCountTitle1; }
        public TextMeshProUGUI TextCountTitle2 { get => textCountTitle2; }
        public TextMeshProUGUI TextMicrosoft { get => textMicrosoft; }
        public TextMeshProUGUI TextMacAppStore { get => textMacAppStore; }
        public TextMeshProUGUI TextAppStore { get => textAppStore; }
        public TextMeshProUGUI TextGooglePlay { get => textGooglePlay; }
        public ButtonLanguage[] BtnLanguages { get => btnLanguages; }
    }
}