using System;
using System.Collections.Generic;
using TDL.Commands;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class HomeRightMenuViewMediator : IInitializable, IDisposable, IMediator
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private AccessibilityModel _accessibilityModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private MetaDataModel _metaDataModel;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private readonly UserContentAppModel _userContentAppModel;
        [Inject] private readonly ApplicationSettingsInstaller.ServerSettings serverSettings;
        
        private AccessibilityViewItem _accessibilityViewItem;
        private Dictionary<string, ISelectableMenuItem> _rightMenuViews = new Dictionary<string, ISelectableMenuItem>();

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<OnHomeActivatedViewSignal>(OnViewEnable);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(OnViewDisable);
        }

        public void Dispose()
        {
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(OnViewEnable);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(OnViewDisable);
        }
        
        public void OnViewEnable()
        {
            _homeModel.OnRightMenuChanged += OnRightMenuChanged;
            _accessibilityModel.OnGrayscaleChanged += AccessibilityChangeLabelLines;

            _signal.Subscribe<SaveRightMenuItemViewSignal>(OnSaveRightMenuItem);
            _signal.Subscribe<AccessibilityMenuInitializedViewSignal>(UpdateAccessibilitySettings);
            _signal.Subscribe<RecentlyViewedClickViewSignal>(OnRecentlyViewedClick);
            _signal.Subscribe<FavoritesClickViewSignal>(OnFavouritesClick);
            _signal.Subscribe<MyContentClickViewSignal>(OnMyContentClick);
            _signal.Subscribe<RightMenuMyTeacherClickViewSignal>(OnMyTeacherClick);
            _signal.Subscribe<FavoritesClickFromHomeTabsViewSignal>(FavoritesClickFromHomeTabs);
            _signal.Subscribe<RecentlyViewedClickFromHomeTabsViewSignal>(RecentlyViewedClickFromHomeTabs);
            _signal.Subscribe<MyContentViewedClickFromHomeTabsViewSignal>(MyContentViewedClickFromHomeTabs);
            _signal.Subscribe<MyTeacherViewedClickFromHomeTabsViewSignal>(MyTeacherViewedClickFromHomeTabs);
            _signal.Subscribe<MetaDataClickViewSignal>(OnMetaDataClick);
            _signal.Subscribe<SignOutClickViewSignal>(OnSignOutClick);

            // accessibility        
            _signal.Subscribe<AccessibilityFontSizeClickViewSignal>(OnAccessibilityFontSizeClick);
            _signal.Subscribe<AccessibilityTextToAudioClickViewSignal>(OnAccessibilityTextToAudioClick);
            _signal.Subscribe<AccessibilityGrayscaleClickViewSignal>(OnAccessibilityGrayscaleClick);
            _signal.Subscribe<AccessibilityLabelLinesClickViewSignal>(OnAccessibilityLabelLinesClick);
        }

        public void OnViewDisable()
        {
            if (_homeModel != null)
            {
                _homeModel.OnRightMenuChanged -= OnRightMenuChanged;
            }

            if (_accessibilityModel != null)
            {
                _accessibilityModel.OnGrayscaleChanged -= AccessibilityChangeLabelLines;
            }

            _signal.Unsubscribe<SaveRightMenuItemViewSignal>(OnSaveRightMenuItem);
            _signal.Unsubscribe<AccessibilityMenuInitializedViewSignal>(UpdateAccessibilitySettings);
            _signal.Unsubscribe<RecentlyViewedClickViewSignal>(OnRecentlyViewedClick);
            _signal.Unsubscribe<FavoritesClickViewSignal>(OnFavouritesClick);
            _signal.Unsubscribe<MyContentClickViewSignal>(OnMyContentClick);
            _signal.Unsubscribe<RightMenuMyTeacherClickViewSignal>(OnMyTeacherClick);
            _signal.Unsubscribe<FavoritesClickFromHomeTabsViewSignal>(FavoritesClickFromHomeTabs);
            _signal.Unsubscribe<RecentlyViewedClickFromHomeTabsViewSignal>(RecentlyViewedClickFromHomeTabs);
            _signal.Unsubscribe<MyContentViewedClickFromHomeTabsViewSignal>(MyContentViewedClickFromHomeTabs);
            _signal.Unsubscribe<MyTeacherViewedClickFromHomeTabsViewSignal>(MyTeacherViewedClickFromHomeTabs);
            _signal.Unsubscribe<MetaDataClickViewSignal>(OnMetaDataClick);
            _signal.Unsubscribe<SignOutClickViewSignal>(OnSignOutClick);

            // accessibility        
            _signal.TryUnsubscribe<AccessibilityFontSizeClickViewSignal>(OnAccessibilityFontSizeClick);
            _signal.TryUnsubscribe<AccessibilityTextToAudioClickViewSignal>(OnAccessibilityTextToAudioClick);
            _signal.TryUnsubscribe<AccessibilityGrayscaleClickViewSignal>(OnAccessibilityGrayscaleClick);
            _signal.TryUnsubscribe<AccessibilityLabelLinesClickViewSignal>(OnAccessibilityLabelLinesClick);
        }

        private void OnRightMenuChanged(bool isActive)
        {
            if (isActive)
            {
                SelectMenuItem();
                SetMyTeacherVisibility();
                HideMetaDataItemIfUnavailable();
            }
        }

        private void SelectMenuItem()
        {
            if (_homeModel.HomeTabFavouritesActive && !IsMenuItemSelected(LocalizationConstants.FavouritesKey))
            {
                SetSelectedMenuItemFromRightMenu(LocalizationConstants.FavouritesKey);
            }
            else if (_homeModel.HomeTabRecentActive && !IsMenuItemSelected(LocalizationConstants.RecentlyViewedKey))
            {
                SetSelectedMenuItemFromRightMenu(LocalizationConstants.RecentlyViewedKey);
            }
        }

        private void OnSaveRightMenuItem(SaveRightMenuItemViewSignal signal)
        {
            _rightMenuViews.Add(signal.MenuItemName, signal.MenuItem);
            _homeModel.SelectableMenuItems.Add(signal.MenuItemName, signal.MenuItem);
        }

        private void FavoritesClickFromHomeTabs(FavoritesClickFromHomeTabsViewSignal signal)
        {
            if (signal.Status)
            {
                if (!IsMenuItemSelected(LocalizationConstants.FavouritesKey))
                {
                    _signal.Fire(new ShowFavouritesCommandSignal());
                    _signal.TryFire(new HideSearchInputViewSignal());

                    HideSideMenus();
                    SetSelectedMenuItemFromRightMenu(LocalizationConstants.FavouritesKey);
                }
            }
        }

        private void RecentlyViewedClickFromHomeTabs(RecentlyViewedClickFromHomeTabsViewSignal signal)
        {
            if (signal.Status)
            {
                if (!IsMenuItemSelected(LocalizationConstants.RecentlyViewedKey))
                {
                    _signal.Fire(new ShowRecentlyViewedCommandSignal());
                    _signal.TryFire(new HideSearchInputViewSignal());

                    HideSideMenus();
                    SetSelectedMenuItemFromRightMenu(LocalizationConstants.RecentlyViewedKey);
                }
            }
        }
        
        private void MyContentViewedClickFromHomeTabs(MyContentViewedClickFromHomeTabsViewSignal signal)
        {
            if (signal.Status)
            {
                if (!IsMenuItemSelected(LocalizationConstants.MyContentKey))
                {
                    _userContentAppModel.IsTeacherContent = false;
                    _signal.Fire(new GetUserContentCommandSignal());
                    _signal.TryFire(new HideSearchInputViewSignal());

                    HideSideMenus();
                    SetSelectedMenuItemFromRightMenu(LocalizationConstants.MyContentKey);
                }
            }
        }

        #region MyTreacher
        
        private void MyTeacherViewedClickFromHomeTabs(MyTeacherViewedClickFromHomeTabsViewSignal signal)
        {
            if (signal.Status)
            {
                if (!IsMenuItemSelected(LocalizationConstants.MyTeacherKey))
                {
                    _userContentAppModel.IsTeacherContent = true;
                    _signal.Fire(new GetTeacherContentCommandSignal());
                    _signal.TryFire(new HideSearchInputViewSignal());

                    HideSideMenus();
                    SetSelectedMenuItemFromRightMenu(LocalizationConstants.MyTeacherKey);
                }
            }
        }
        
        private void SetMyTeacherVisibility()
        {
            if (_rightMenuViews.ContainsKey(LocalizationConstants.MyTeacherKey))
            {
                var myTeacherView = _rightMenuViews[LocalizationConstants.MyTeacherKey] as MyTeacherViewItem;
                if (myTeacherView != null)
                {
                    myTeacherView.gameObject.SetActive(!_userLoginModel.IsTeacher);
                }
            }   
        }
        
        #endregion

        private void OnRecentlyViewedClick()
        {
            if (!IsMenuItemSelected(LocalizationConstants.RecentlyViewedKey))
            {
                _signal.Fire<ActivateHomeTabRecentCommandSignal>();
            }
        }

        private void OnFavouritesClick()
        {
            if (!IsMenuItemSelected(LocalizationConstants.FavouritesKey))
            {
                _signal.Fire<ActivateHomeTabFavouritesCommandSignal>();
            }
        }
        
        private void OnMyContentClick()
        {
            if (!IsMenuItemSelected(LocalizationConstants.MyContentKey))
            {
                _signal.Fire<ActivateHomeTabMyContentCommandSignal>();
            }
        }
        
        private void OnMyTeacherClick()
        {
            if (!IsMenuItemSelected(LocalizationConstants.MyTeacherKey))
            {
                _signal.Fire<ActivateHomeTabMyTeacherCommandSignal>();
            }
        }

        private void OnSignOutClick()
        {
            _signal.TryUnsubscribe<AccessibilityTextToAudioClickViewSignal>(OnAccessibilityTextToAudioClick);
            
            DeselectAnySelectedRightMenuItem();
            ResetAccessibilitySettings();

            _signal.TryFire(new HideSearchInputViewSignal());
            _signal.Fire(new ResetAllBreadcrumbsViewSignal());
            _signal.Fire(new SignOutClickCommandSignal());

            _homeModel.SelectableMenuItems.Add(LocalizationConstants.RecentlyViewedKey, _rightMenuViews[LocalizationConstants.RecentlyViewedKey]);
            _homeModel.SelectableMenuItems.Add(LocalizationConstants.FavouritesKey, _rightMenuViews[LocalizationConstants.FavouritesKey]);
            Debug.Log("Sign outt");
            _userLoginModel.IsSigningOut = true;
            if (_userLoginModel.IsMicrosoftLogin)
            {
                Application.OpenURL("https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=https://api.prod.3dl.no/Teams/teamsLogout&client_id=55daecc5-accc-4657-be01-59233ea6a2da");
            }
        }

        private void HideSideMenus()
        {
            _signal.Fire(new ShowLeftMenuCommandSignal(false));
            _signal.Fire(new ShowRightMenuCommandSignal(false));
        }

        private void SetSelectedMenuItemFromRightMenu(string itemName)
        {
            if (_homeModel.SelectedMenuItem != null)
            {
                _homeModel.SelectedMenuItem.Deselect();
            }

            if (_homeModel.SelectableMenuItems.ContainsKey(itemName))
            {
                _homeModel.SelectedMenuItem = _homeModel.SelectableMenuItems[itemName];
                _homeModel.SelectedMenuItem.Select();
            }
        }

        private void DeselectAnySelectedRightMenuItem()
        {
            foreach (var menuViewKey in _rightMenuViews.Keys)
            {
                if (_homeModel.SelectableMenuItems.ContainsKey(menuViewKey))
                {
                    var item = _homeModel.SelectableMenuItems[menuViewKey];
                    if (item.IsSelected())
                    {
                        item.Deselect();
                    }
                }
            }
        }

        private bool IsMenuItemSelected(string itemName)
        {
            if (_homeModel.SelectableMenuItems.ContainsKey(itemName))
            {
                return _homeModel.SelectableMenuItems[itemName].IsSelected();
            }

            return false;
        }

        #region Accessibility
        
        private void UpdateAccessibilitySettings()
        {
            _accessibilityViewItem = (AccessibilityViewItem) _rightMenuViews[LocalizationConstants.RightMenuAccessibilityKey];
            
            _accessibilityViewItem.AccessibilityTextToAudio.isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio);
            _accessibilityViewItem.AccessibilityGrayscale.isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityGrayscale);
            _accessibilityViewItem.AccessibilityLabelLines.isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines);
            _accessibilityViewItem.AccessibilityFontSize.value = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);

            if (!_accessibilityModel.UpdateLabelLines)
            {
                _accessibilityModel.UpdateLabelLines = true;
            }
        }
        
        private void ResetAccessibilitySettings()
        {
            _accessibilityViewItem.AccessibilityTextToAudio.isOn = false;
            _accessibilityViewItem.AccessibilityGrayscale.isOn = false;
            _accessibilityViewItem.AccessibilityLabelLines.isOn = false;
            _accessibilityViewItem.AccessibilityFontSize.value = AccessibilityConstants.FontSizeMedium150;

            _signal.Fire<ResetAccessibilityCommandSignal>();
        }

        private void OnAccessibilityFontSizeClick(AccessibilityFontSizeClickViewSignal signal)
        {
            _signal.Fire(new AccessibilityFontSizeClickCommandSignal(signal.FontSize));
        }

        private void OnAccessibilityTextToAudioClick(AccessibilityTextToAudioClickViewSignal signal)
        {
            PlayTextToSpeechAccessibility(GetAccessibilityTextName(LocalizationConstants.AccessibilityTextToAudioKey) 
                                          + GetAccessibilityTextStatus(signal.IsEnabled), true);

            _signal.Fire(new AccessibilityTextToAudioClickCommandSignal(signal.IsEnabled));
        }

        private void OnAccessibilityGrayscaleClick(AccessibilityGrayscaleClickViewSignal signal)
        {
            _signal.Fire(new AccessibilityGrayscaleClickCommandSignal(signal.IsEnabled));
        }

        private void OnAccessibilityLabelLinesClick(AccessibilityLabelLinesClickViewSignal signal)
        {
            _signal.Fire(new AccessibilityLabelLinesClickCommandSignal(signal.IsEnabled));
        }

        private void AccessibilityChangeLabelLines(bool status)
        {
            if (_accessibilityModel.UpdateLabelLines)
            {
                if (_accessibilityViewItem.AccessibilityLabelLines.isOn != status)
                {
                    _accessibilityViewItem.AccessibilityLabelLines.isOn = status;
                }
            }
        }
        
        private string GetAccessibilityTextName(string accessibilityName)
        {
            return _localizationModel.GetCurrentSystemTranslations(accessibilityName);
        }

        private string GetAccessibilityTextStatus(bool status)
        {
            return status
                ? _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityIsEnabledKey)
                : _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityIsDisabledKey);
        }

        private void PlayTextToSpeechAccessibility(string text, bool forcePlay = false)
        {
            _signal.Fire(new AccessibilityTTSPlayCommandSignal(text, forcePlay));
        }

        #endregion

        #region Meta Data

        private void HideMetaDataItemIfUnavailable()
        {
            if (_metaDataModel.Link == null)
            {
                if (_rightMenuViews.ContainsKey(LocalizationConstants.MetaDataKey))
                {
                    var metaDtaItem = _rightMenuViews[LocalizationConstants.MetaDataKey] as MetaDataItem;
                    if (metaDtaItem != null)
                    {
                        metaDtaItem.gameObject.SetActive(false);
                    }
                    
                    #if UNITY_WEBGL
                    metaDtaItem.gameObject.SetActive(false);
                    #endif
                }   
            }
        }

        private void OnMetaDataClick()
        {
            var modifiedLink = _metaDataModel.Link.Uri
                .Replace(ServerConstants.MetaDataTokenField, _userLoginModel.AuthorizationToken.Replace(ServerConstants.MetaDataTokenUselessWordField, string.Empty))
                .Replace(ServerConstants.MetaDataCultureField, _localizationModel.CurrentLanguageCultureCode);
            
            _signal.Fire(new OpenUrlCommandSignal(modifiedLink));
        }

        #endregion
    }
}