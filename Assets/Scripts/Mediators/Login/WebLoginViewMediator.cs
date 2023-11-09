using System;
using System.Collections;
using System.Collections.Generic;
using Signals;
using Signals.Login;
using TDL.Commands;
using TDL.Constants;
using Zenject;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace TDL.Views
{
    public class WebLoginViewMediator : LoginViewMediator
    {
        private EventTrigger eventTrigger;
        
        protected override void SubscribeOnListeners()
        {
            base.SubscribeOnListeners();
            LanguageButtonHandler();

            eventTrigger = _loginView.LanguageDropdown.GetComponent<EventTrigger>();
            var entryEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryEnter.callback.AddListener( (eventData) => { ShowLanguagesDropdown(); } );
            eventTrigger.triggers.Add(entryEnter);
        }

        private void LanguageButtonHandler()
        {
            var view = _loginView as LoginWebViewPC;
            if (view == null)
            {
                return;
            }
            foreach (var item in view.BtnLanguages)
            {
                if (item == null)
                {
                    continue;
                }
                if (item.button == null)
                {
                    continue;
                }
                item.button.onClick.AddListener(() =>
                {
                    var cultureCode = item.culture;
                    _localizationModel.CurrentLanguageCultureCode = cultureCode;
                });
            }
        }

        protected override void ChangeUiInterface()
        {
            base.ChangeUiInterface();

            var view = _loginView as LoginWebViewPC;
            if (view == null)
            {
                return;
            }
            UpdateTextLocal(view.TextHome, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextHome));
            UpdateTextLocal(view.TextVr, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextVr));
            UpdateTextLocal(view.TextAr, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextAr));
            UpdateTextLocal(view.TextDownload, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextDownload));
            UpdateTextLocal(view.TextRequestDemo, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextRequestDemo));
            UpdateTextLocal(view.TextAboutAs, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextAboutAs));
            UpdateTextLocal(view.TextCamp3d, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextCamp3d));
            UpdateTextLocal(view.TextAccessoriesForVr, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextAccessoriesForVr));
            UpdateTextLocal(view.TextAbout3dl, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextAbout3dl));
            UpdateTextLocal(view.TextPrivacy, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextPrivacy));
            UpdateTextLocal(view.TextContactUs, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextContactUs));
            UpdateTextLocal(view.TextButtonContacktUs, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextButtonContacktUs));
            UpdateTextLocal(view.TextCountTitle1, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextCountTitle1));
            UpdateTextLocal(view.TextCountTitle2, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextCountTitle2));
            UpdateTextLocal(view.TextMicrosoft, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextMicrosoft));
            UpdateTextLocal(view.TextMacAppStore, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextMacAppStore));
            UpdateTextLocal(view.TextAppStore, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextAppStore));
            UpdateTextLocal(view.TextGooglePlay, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MainScreenTextGooglePlay));
        }

        private void UpdateTextLocal(TextMeshProUGUI txt, string text)
        {
            if (txt == null || txt.gameObject == null)
            {
                return;
            }
            txt.text = text;
        }

        protected override void UpdateLanguageUI()
        {
            base.UpdateLanguageUI();
            ForceRebuildLayout();
        }

        private void ForceRebuildLayout()
        {
            var obj = (_loginView as LoginWebViewPC);
            if (obj == null) 
                return;
            
            var root = obj.RootLayout;
            obj.StartCoroutine(ReEnableAfterFrame(root.gameObject));
        }

        private IEnumerator ReEnableAfterFrame(GameObject obj)
        {
            obj.SetActive(false);
            yield return new WaitForEndOfFrame();
            obj.SetActive(true);
            _loginView.LanguageDropdown.RefreshShownValue();
            _loginView.LanguageDropdown.Hide();
        }

        private void ShowLanguagesDropdown()
        {
            _loginView.LanguageDropdown.Show();
        }

        protected override void CreateLanguageUI()
        {
            base.CreateLanguageUI();
        }
    }
}