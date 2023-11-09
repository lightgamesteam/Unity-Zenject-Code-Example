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
    public class HomeViewMediator : IInitializable, IDisposable, IMediator
    {
        private HomeViewBase _homeView;

        [Inject] private HomeViewBase.Factory _homeViewFactory;
        [Inject] private readonly SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private UserLoginModel _userLoginModelModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private MetaDataModel _metaDataModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        public void Initialize()
        {
            CreateView();
            SubscribeOnListeners();
            SaveContentPanels();
            SubscribeImageRecognition();
            ARModeRightPanel();
        }

        private void CreateView()
        {
            _homeView = _homeViewFactory.Create();
            _homeView.InitUiComponents();
            _signal.Fire(new RegisterScreenCommandSignal(WindowConstants.Home, _homeView.gameObject));
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<OnHomeActivatedViewSignal>(OnViewEnable);
            _signal.Subscribe<OnHomeDeactivatedViewSignal>(OnViewDisable);

            _contentModel.OnContentModelChanged += ShowHomeScreen;
            _homeModel.OnLeftMenuChanged += ShowLeftMenu;
            _homeModel.OnRightMenuChanged += ShowRightMenu;
            _homeModel.OnHomeTabMyTeacherVisibilityChanged += SetTabMyTeacherVisibility;
            _contentModel.OnSubjectChanged += OnSubjectChanged;
            _contentModel.OnTopicChanged += OnTopicChanged;
            _contentModel.OnSubtopicChanged += OnSubtopicChanged;
            _contentModel.OnActivityChanged += OnActivityChanged;
            _contentModel.OnChosenActivityChanged += OnChosenActivityChanged;
            _contentModel.OnCategoryWithHeadersChanged += OnCategoryWithHeadersChanged;

            _signal.Subscribe<SearchChangedViewSignal>(OnSearchChanged);
            _signal.Subscribe<LeftMenuClickViewSignal>(OnLeftMenuClick);
            _signal.Subscribe<RightMenuClickViewSignal>(OnRightMenuClick);
            _signal.Subscribe<CloseAppClickViewSignal>(OnCloseAppClick);
            _signal.Subscribe<OnLoginSuccessCommandSignal>(OnUserNameUpdated);
            _signal.Subscribe<SubjectHyperlinkClickViewSignal>(OnSubjectHyperlinkClick);
            _signal.Subscribe<TopicHyperlinkClickViewSignal>(OnTopicHyperlinkClick);
            _signal.Subscribe<SubtopicHyperlinkClickViewSignal>(OnSubtopicHyperlinkClick);
            _signal.Subscribe<ActivityHyperlinkClickViewSignal>(OnActivityHyperlinkClick);
            _signal.Subscribe<HomeTabFavouritesClickViewSignal>(OnHomeTabFavouritesClick);
            _signal.Subscribe<HomeTabRecentClickViewSignal>(OnHomeTabRecentClick);
            _signal.Subscribe<HomeTabMyContentClickViewSignal>(OnHomeTabMyContentClick);
            _signal.Subscribe<HomeTabMyTeacherClickViewSignal>(OnHomeTabMyTeacherClick);
            _signal.Subscribe<HideSearchInputViewSignal>(HideSearchInput);
            _signal.Subscribe<ResetAllBreadcrumbsViewSignal>(ResetAllBreadcrumbs);
            _signal.Subscribe<SetHomeTabsVisibilityViewSignal>(signal => SetHomeTabsVisibility(signal.Status));
            
            _accessibilityModel.OnFontSizeChanged += OnFontSizeChanged;
        }

        public void OnViewEnable()
        {
            _localizationModel.OnLanguageChanged += OnLanguageChanged;
            _homeModel.OnHomeTabFavouritesChanged += OnHomeTabFavouritesChanged;
            _homeModel.OnHomeTabRecentChanged += OnHomeTabRecentChanged;
            _homeModel.OnHomeTabMyContentChanged += OnHomeTabMyContentChanged;
            _homeModel.OnHomeTabMyTeacherChanged += OnHomeTabMyTeacherChanged;
            _homeModel.OnNoSearchResultsChanged += SetNoSearchResultsStatus;

            _signal.Subscribe<OnChangeLanguageClickViewSignal>(OnChangeLanguageClick);
            
            // description
            _signal.Subscribe<OnDescriptionCloseClickViewSignal>(OnDescriptionClose);
            _signal.Subscribe<LoadAndPlayDescriptionViewSignal>(LoadAndPlayDescription);
            _signal.Subscribe<PauseAllExceptActiveDescriptionViewSignal>(PauseAllExceptActiveDescription);

            CheckTerm();
        }
        
        public void OnViewDisable()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= OnLanguageChanged;
            }

            if (_homeModel != null)
            {
                _homeModel.OnHomeTabFavouritesChanged -= OnHomeTabFavouritesChanged;
                _homeModel.OnHomeTabRecentChanged -= OnHomeTabRecentChanged;
                _homeModel.OnHomeTabMyContentChanged -= OnHomeTabMyContentChanged;
                _homeModel.OnHomeTabMyTeacherChanged -= OnHomeTabMyTeacherChanged;
                _homeModel.OnNoSearchResultsChanged -= SetNoSearchResultsStatus;
            }
            
            _signal.Unsubscribe<OnChangeLanguageClickViewSignal>(OnChangeLanguageClick);
            
            // description
            _signal.Unsubscribe<OnDescriptionCloseClickViewSignal>(OnDescriptionClose);
            _signal.Unsubscribe<LoadAndPlayDescriptionViewSignal>(LoadAndPlayDescription);
            _signal.Unsubscribe<PauseAllExceptActiveDescriptionViewSignal>(PauseAllExceptActiveDescription);
            _signal.Fire<CloseAllOpenedDescriptionsCommandSignal>();
        }
        
#region CheckTerm

        private void CheckTerm()
        {
            _signal.Fire(new OpenTermCommandSignal(true));
        }
        
#endregion

        private void SubscribeImageRecognition()
        {
            _homeView._imageRecognitionButton.gameObject.SetActive(DeviceInfo.IsTablet());
            _homeView._imageRecognitionButton.onClick.AddListener(StartImageRecognition);
        }
        
        private void StartImageRecognition()
        {
            _signal.Subscribe<ImageRecognitionResponseSignal>(ResponseImageRecognition);

            _signal.Fire<StartImageRecognitionModuleCommandSignal>();
        }

        private void ResponseImageRecognition(ISignal signal)
        {
            _signal.Unsubscribe<ImageRecognitionResponseSignal>(ResponseImageRecognition);

            ImageRecognitionResponseSignal s = (ImageRecognitionResponseSignal) signal;
        
            if(!s.HasResponse)
                return;
        
            int id = -1;
        
            List<string> str = new List<string>();
            str.AddRange(s.Name.Split('-'));

            if (str.Count > 0)
            {
                if (int.TryParse(str[0], out id))
                {
                    if (id >= 0)
                    {
                        if (_contentModel.HasAssetById(id))
                        {
                            _contentModel.AssetDetailsSignalSource = new AssetItemClickCommandSignal(id, -1);
                            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {id}));
                            return;
                        }

                        _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ImgRecoAssetNotAvailableKey), type: PopupOverlayType.MessageBox));
                        return;
                    }
                }
            }
        
            _signal.Fire(new PopupOverlaySignal(true, _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ImgRecoAssetDoesntExistKey), type: PopupOverlayType.MessageBox));
        }

        private void OnSearchChanged(SearchChangedViewSignal signal)
        {
            var searchValue = signal.SearchValue;

            if (string.IsNullOrEmpty(searchValue))
            {
                _signal.Fire<ShowLastShownCategoryCommandSignal>();
                SetBreadcrumbsVisibility(true);
            }
            else
            {
                _signal.Fire(new GetSearchAssetsCommandSignal(searchValue, _localizationModel.CurrentLanguageCultureCode));
                SetBreadcrumbsVisibility(false);
            }
        }

        private void ARModeRightPanel()
        {
            _homeView.ArModeToggle.gameObject.SetActive(DeviceInfo.IsTablet());

            if (Application.isEditor) // Test AR <<<<<<<<<<<
            {
                _homeView.ArModeToggle.gameObject.SetActive(true);
            }
        }

        private void OnSubjectHyperlinkClick()
        {
            if (_contentModel.SelectedSubject != null)
            {
                _signal.Fire(new SubjectMenuItemClickCommandSignal(_contentModel.SelectedSubject.ParentGrade.Grade.id,
                    _contentModel.SelectedSubject.Subject.id, true));
            }
        }

        private void OnTopicHyperlinkClick()
        {
            if (_contentModel.SelectedTopic != null)
            {
                _signal.Fire(new TopicItemClickCommandSignal(_contentModel.SelectedTopic.ParentSubject.Subject.id,
                    _contentModel.SelectedTopic.Topic.id));
            }
        }

        private void OnSubtopicHyperlinkClick()
        {
            if (_contentModel.SelectedSubtopic != null)
            {
                _signal.Fire(new SubtopicItemClickCommandSignal(_contentModel.SelectedSubtopic.ParentTopic.Topic.id,
                    _contentModel.SelectedSubtopic.Subtopic.id));
            }
        }

        private void OnActivityHyperlinkClick()
        {
            if (_contentModel.SelectedChosenActivity.IsSelected)
            {
                _signal.Fire<CreateActivitiesScreenCommandSignal>();
                _homeView.ResetChosenActivityBreadcrumb();
            }
        }

        private void OnUserNameUpdated()
        {
            _homeView.SetUserName(_userLoginModelModel.FirstLetter, _userLoginModelModel.Firstname, _userLoginModelModel.Lastname);
        }

        public void Dispose()
        {
            if (_homeModel != null)
            {
                _homeModel.OnLeftMenuChanged -= ShowLeftMenu;
                _homeModel.OnRightMenuChanged -= ShowRightMenu;
                _homeModel.OnHomeTabMyTeacherVisibilityChanged -= SetTabMyTeacherVisibility;
            }

            if (_contentModel != null)
            {
                _contentModel.OnContentModelChanged -= ShowHomeScreen;
                _contentModel.OnSubjectChanged -= OnSubjectChanged;
                _contentModel.OnTopicChanged -= OnTopicChanged;
                _contentModel.OnSubtopicChanged -= OnSubtopicChanged;
                _contentModel.OnActivityChanged -= OnActivityChanged;
                _contentModel.OnChosenActivityChanged -= OnChosenActivityChanged;
                _contentModel.OnCategoryWithHeadersChanged -= OnCategoryWithHeadersChanged;
            }
            
            if (_accessibilityModel != null)
            {
                _accessibilityModel.OnFontSizeChanged -= OnFontSizeChanged;
            }

            _signal.Unsubscribe<SearchChangedViewSignal>(OnSearchChanged);
            _signal.Unsubscribe<LeftMenuClickViewSignal>(OnLeftMenuClick);
            _signal.Unsubscribe<RightMenuClickViewSignal>(OnRightMenuClick);
            _signal.Unsubscribe<CloseAppClickViewSignal>(OnCloseAppClick);
            _signal.Unsubscribe<SubjectHyperlinkClickViewSignal>(OnSubjectHyperlinkClick);
            _signal.Unsubscribe<TopicHyperlinkClickViewSignal>(OnTopicHyperlinkClick);
            _signal.Unsubscribe<SubtopicHyperlinkClickViewSignal>(OnSubtopicHyperlinkClick);
            _signal.Unsubscribe<ActivityHyperlinkClickViewSignal>(OnActivityHyperlinkClick);
            _signal.Unsubscribe<HomeTabFavouritesClickViewSignal>(OnHomeTabFavouritesClick);
            _signal.Unsubscribe<HomeTabRecentClickViewSignal>(OnHomeTabRecentClick);
            _signal.Unsubscribe<HomeTabMyContentClickViewSignal>(OnHomeTabMyContentClick);
            _signal.Unsubscribe<HomeTabMyTeacherClickViewSignal>(OnHomeTabMyTeacherClick);
            _signal.Unsubscribe<HideSearchInputViewSignal>(HideSearchInput);
            _signal.Unsubscribe<ResetAllBreadcrumbsViewSignal>(ResetAllBreadcrumbs);
            _signal.TryUnsubscribe<SetHomeTabsVisibilityViewSignal>(signal => SetHomeTabsVisibility(signal.Status));

            _signal.Unsubscribe<OnHomeActivatedViewSignal>(OnViewEnable);
            _signal.Unsubscribe<OnHomeDeactivatedViewSignal>(OnViewDisable);
        }

        private void SaveContentPanels()
        {
            _signal.Fire(new SaveContentPanelsCommandSignal(
                _homeView.GetLeftMenuContent(),
                _homeView.GetTopicsSubtopicsContent(),
                _homeView.GetAssetsContent()));
        }

        private void ShowHomeScreen()
        {
            _signal.Fire(new CreateLeftMenuCommandSignal());
            _signal.Fire(new SetMyTeacherVisibilityCommandSignal());
            _signal.Fire(new ShowHomeScreenCommandSignal());

            UpdateHomeLocalizationOnStart();
            OnHomeTabFavouritesClick();
        }
        
        private void OnCloseAppClick()
        {
            _signal.Fire(new CloseAppCommandSignal());
        }

        private void OnLeftMenuClick(LeftMenuClickViewSignal viewSignal)
        {
            _signal.Fire(new ShowLeftMenuCommandSignal(viewSignal.IsEnabled));
        }

        private void OnRightMenuClick(RightMenuClickViewSignal viewSignal)
        {
            _signal.Fire(new ShowRightMenuCommandSignal(viewSignal.IsEnabled));
        }

        private void ShowLeftMenu(bool status)
        {
            _homeView.ShowLeftMenu(status);
        }

        private void ShowRightMenu(bool status)
        {
            _homeView.ShowRightMenu(status);
            _homeView.CloseLanguageDropdownIfOpened();
        }

        private void OnSubjectChanged()
        {
            CreateTopicsContent();
            UpdateGradeSubjectBreadcrumbs();
            HideSearchInput();
            DeactivateHomeTabs();
            HideSideMenus();
        }

        private void OnTopicChanged()
        {
            CreateSubtopicsContent();
            UpdateTopicBreadcrumbs();
            HideSearchInput();
            HideSideMenus();
        }

        private void OnSubtopicChanged()
        {
            CreateAssets();
            UpdateSubtopicBreadcrumbs();
            HideSearchInput();
            HideSideMenus();
        }

        private void OnActivityChanged()
        {
            if (!_contentModel.HasCategoryOnlyOwnAssets())
            {
                UpdateActivitiesBreadcrumbs();
            }
        }

        private void OnChosenActivityChanged()
        {
            UpdateChosenActivityBreadcrumbs();
        }
        
        private void OnCategoryWithHeadersChanged(bool status)
        {
            _homeView.ShowCategoryWithHeaders(status);
        }

        private void CreateTopicsContent()
        {
            _signal.Fire(new CreateTopicsContentCommandSignal());
        }

        private void CreateSubtopicsContent()
        {
            _signal.Fire(new CreateSubtopicsContentCommandSignal());
        }

        private void CreateAssets()
        {
            _signal.Fire(new CreateAssetsCommandSignal());
        }

        private void HideSideMenus()
        {
            _signal.Fire(new ShowLeftMenuCommandSignal(false));
            _signal.Fire(new ShowRightMenuCommandSignal(false));
        }

        private void HideSearchInput()
        {
            _homeView.HideSearchInput();
        }

        #region Breadcrumbs

        private void SetBreadcrumbsVisibility(bool status)
        {
            _homeView.SetBreadcrumbsVisibility(status);
        }
        
        private void UpdateGradeSubjectBreadcrumbs()
        {
            _homeView.Grade.gameObject.SetActive(true);
            _homeView.Grade.text = _contentModel.SelectedGrade.Grade.name;
            _homeView.SubjectText.gameObject.SetActive(true);
            _homeView.SubjectText.text = _contentModel.SelectedSubject.Subject.name;
            
            SetBreadcrumbsVisibility(true);
            _homeView.ResetTopicBreadcrumb();
            _homeView.ResetSubtopicBreadcrumb();
            _homeView.ResetActivitiesBreadcrumb();
            _homeView.ResetChosenActivityBreadcrumb();
        }

        private void UpdateTopicBreadcrumbs()
        {
            _homeView.TopicText.gameObject.SetActive(true);
            _homeView.TopicText.text = _contentModel.SelectedTopic.Topic.name;
            _homeView.ShowTopicDivider(true);
            _homeView.ResetSubtopicBreadcrumb();
            _homeView.ResetActivitiesBreadcrumb();
            _homeView.ResetChosenActivityBreadcrumb();
        }

        private void UpdateSubtopicBreadcrumbs()
        {
            _homeView.SubtopicText.gameObject.SetActive(true);
            _homeView.SubtopicText.text = _contentModel.SelectedSubtopic.Subtopic.name;
            _homeView.ShowSubtopicDivider(true);
            _homeView.ResetActivitiesBreadcrumb();
            _homeView.ResetChosenActivityBreadcrumb();
        }
        
        private void UpdateActivitiesBreadcrumbs()
        {
            _homeView.ActivitiesText.gameObject.SetActive(true);
            _homeView.ActivitiesText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ActivitiesKey);
            _homeView.ShowActivitiesDivider(true);
        }

        private void UpdateChosenActivityBreadcrumbs()
        {
            _homeView.ChosenActivityText.gameObject.SetActive(true);
            _homeView.ChosenActivityText.text = _localizationModel.GetCurrentSystemTranslations(_contentModel.SelectedChosenActivity.ActivityName);
            _homeView.ShowChosenActivityDivider(true);
        }

        private void DeactivateHomeTabs()
        {
            _signal.Fire(new DeactivateHomeTabsCommandSignal());
            SetHomeTabsVisibility(false);
        }

        public void ResetAllBreadcrumbs()
        {
            _homeView.ResetAllBreadcrumbs();
        }
        
        #endregion

        #region Home Tabs
        
        private void SetHomeTabsVisibility(bool status)
        {
            _homeView.SetHomeTabsVisibility(status);
        }

        private void OnHomeTabFavouritesClick()
        {
            if (!_homeModel.HomeTabFavouritesActive)
            {
                _signal.Fire<ActivateHomeTabFavouritesCommandSignal>();
            }
        }

        private void OnHomeTabRecentClick()
        {
            if (!_homeModel.HomeTabRecentActive)
            {
                _signal.Fire<ActivateHomeTabRecentCommandSignal>();
            }
        }
        
        private void OnHomeTabMyContentClick()
        {
            if (!_homeModel.HomeTabMyContentActive)
            {
                _signal.Fire<ActivateHomeTabMyContentCommandSignal>();
            } 
        }
        
        private void OnHomeTabMyTeacherClick()
        {
            if (!_homeModel.HomeTabMyTeacherActive)
            {
                _signal.Fire<ActivateHomeTabMyTeacherCommandSignal>();
            } 
        }

        private void OnHomeTabFavouritesChanged(bool status)
        {
            _signal.Fire(new FavoritesClickFromHomeTabsViewSignal(status));
            
            _homeView.SetHomeTabsVisibility(true);
            _homeView.SetHomeTabFavouritesActive(status);
        }

        private void OnHomeTabRecentChanged(bool status)
        {
            _signal.Fire(new RecentlyViewedClickFromHomeTabsViewSignal(status));
            
            _homeView.SetHomeTabsVisibility(true);
            _homeView.SetHomeTabRecentActive(status);
        }
        
        private void OnHomeTabMyContentChanged(bool status)
        {
            _signal.Fire(new MyContentViewedClickFromHomeTabsViewSignal(status));
            
            _homeView.SetHomeTabsVisibility(true);
            _homeView.SetHomeTabMyContentActive(status);
        }
        
        private void OnHomeTabMyTeacherChanged(bool status)
        {
            _signal.Fire(new MyTeacherViewedClickFromHomeTabsViewSignal(status));
            
            _homeView.SetHomeTabsVisibility(true);
            _homeView.SetHomeTabMyTeacherActive(status);
        }

        private void SetTabMyTeacherVisibility(bool status)
        {
            _homeView.SetTabMyTeacherVisibility(status);
        }

        #endregion

        #region Localization

        private void OnChangeLanguageClick(OnChangeLanguageClickViewSignal signal)
        {
            HideSearchInput();
            _signal.Fire(new ChangeLanguageCommandSignal(signal.LanguageIndex));
        }

        private void UpdateHomeLocalizationOnStart()
        {
            CreateLanguageDropdown();
            UpdateLanguageDropdown();
            ChangeUiInterface();
        }

        private void CreateLanguageDropdown()
        {
            if (_localizationModel.AvailableLanguages.Count > 0)
            {
                var allLanguages = new List<string>();

                foreach (var language in _localizationModel.AvailableLanguages)
                {
                    allLanguages.Add(language.Name);
                }

                _homeView.LanguageDropdown.ClearOptions();
                _homeView.LanguageDropdown.AddOptions(allLanguages);
            }
        }

        private void UpdateLanguageDropdown()
        {
            var foundedLanguage = _localizationModel.AvailableLanguages.Find(item =>
                item.Culture.Equals(_localizationModel.CurrentLanguageCultureCode));
            
            if (foundedLanguage != null)
            {
                var foundedIndex = _homeView.LanguageDropdown.options.FindIndex(item => item.text.Equals(foundedLanguage.Name));
                _homeView.UpdateLanguageDropdownManually(foundedIndex);
            }
        }

        private void OnLanguageChanged()
        {
            UpdateLanguageDropdown();
            ChangeUiInterface();
            ResetSearchResults();
            UpdateDescriptionLanguage();
        }

        private void ChangeUiInterface()
        {
            // top menu
            _homeView.SearchPlaceholder.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.PlaceholderSearchKey);
            _homeView.CloseAppText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.QuitKey);
            _homeView.NoSearchResultsFound.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.NoSearchResultsKey);

            // breadcrumbs
            if (!string.IsNullOrEmpty(_homeView.ActivitiesText.text))
                _homeView.ActivitiesText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ActivitiesKey);
            if (_contentModel.SelectedChosenActivity.IsSelected)
                _homeView.ChosenActivityText.text = _localizationModel.GetCurrentSystemTranslations(_contentModel.SelectedChosenActivity.ActivityName);
            
            // category with headers
            _homeView.ActivityHeaderText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ActivityWithHeadersKey); 
            _homeView.LearnTechHeaderText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AssetsWithHeadersKey); 

            // home tabs
            _homeView.TabFavouritesText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FavouritesKey);
            _homeView.TabRecentText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.RecentKey);
            _homeView.TabMyContentText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyContentKey);
            _homeView.TabMyTeacherText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyTeacherKey);

            // right menu
            _homeView.RecentlyViewed.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.RecentlyViewedKey);
            _homeView.Favourites.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FavouritesKey);
            _homeView.MyContent.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyContentKey);
            _homeView.MyTeacher.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyTeacherKey);

            if (_metaDataModel.LinkLocal != null)
            {
                _homeView.MetaData.text = _metaDataModel.LinkLocal.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                    ? _metaDataModel.LinkLocal[_localizationModel.CurrentLanguageCultureCode]
                    : _metaDataModel.LinkLocal[_localizationModel.FallbackCultureCode];   
            }

            _homeView.ChangePassword.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ChangePasswordKey);
            _homeView.Logout.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LogoutKey);
            _homeView.LanguageTitle.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LanguageTitleKey);
            _homeView.ArModeToggleText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ArModeKey);
            
            // home -> right menu -> accessibility
            _homeView.AccessibilityFontSizeKey.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityFontSizeKey);
            _homeView.AccessibilityTitle.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityTitleKey);
            _homeView.AccessibilityTextToAudio.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityTextToAudioKey);
            _homeView.AccessibilityGrayscale.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityGrayscaleKey);
            _homeView.AccessibilityLabelLines.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityLabelLinesKey);
        }

        #endregion

        #region No Search Results

        private void ResetSearchResults()
        {
            _homeView.OnSearchCloseClick();
        }

        private void SetNoSearchResultsStatus(bool status)
        {
            _homeView.NoSearchResultsFound.gameObject.SetActive(status);
        }

        #endregion

        #region Accessibility Font Size
        
        private void OnFontSizeChanged(float fontSizeScaleFactor)
        {
            if (fontSizeScaleFactor != AccessibilityConstants.FontSizeDefaultScaleFactor)
            {
                // top menu
                _homeView.SearchPlaceholder.fontSize = Mathf.RoundToInt(_homeView.SearchPlaceholder.fontSize / fontSizeScaleFactor);
                _homeView.SearchInput.textComponent.fontSize = Mathf.RoundToInt(_homeView.SearchInput.textComponent.fontSize / fontSizeScaleFactor);
                _homeView.CloseAppText.fontSize = Mathf.RoundToInt(_homeView.CloseAppText.fontSize / fontSizeScaleFactor);
                _homeView.NoSearchResultsFound.fontSize = Mathf.RoundToInt(_homeView.NoSearchResultsFound.fontSize / fontSizeScaleFactor); 

                // breadcrumbs
                _homeView.Grade.fontSize = Mathf.RoundToInt(_homeView.Grade.fontSize / fontSizeScaleFactor);
                _homeView.SubjectText.fontSize = Mathf.RoundToInt(_homeView.SubjectText.fontSize / fontSizeScaleFactor);
                _homeView.TopicText.fontSize = Mathf.RoundToInt(_homeView.TopicText.fontSize / fontSizeScaleFactor);
                _homeView.SubtopicText.fontSize = Mathf.RoundToInt(_homeView.SubtopicText.fontSize / fontSizeScaleFactor);
                _homeView.ActivitiesText.fontSize = Mathf.RoundToInt(_homeView.ActivitiesText.fontSize / fontSizeScaleFactor);
                _homeView.ChosenActivityText.fontSize = Mathf.RoundToInt(_homeView.ChosenActivityText.fontSize / fontSizeScaleFactor);
                
                // category with headers
                _homeView.ActivityHeaderText.fontSize = Mathf.RoundToInt(_homeView.ActivityHeaderText.fontSize / fontSizeScaleFactor); 
                _homeView.LearnTechHeaderText.fontSize = Mathf.RoundToInt(_homeView.LearnTechHeaderText.fontSize / fontSizeScaleFactor); 

                // home tabs
                _homeView.TabFavouritesText.fontSize = Mathf.RoundToInt(_homeView.TabFavouritesText.fontSize / fontSizeScaleFactor);
                _homeView.TabRecentText.fontSize = Mathf.RoundToInt(_homeView.TabRecentText.fontSize / fontSizeScaleFactor);
                _homeView.TabMyContentText.fontSize = Mathf.RoundToInt(_homeView.TabMyContentText.fontSize / fontSizeScaleFactor);
                _homeView.TabMyTeacherText.fontSize = Mathf.RoundToInt(_homeView.TabMyTeacherText.fontSize / fontSizeScaleFactor);

                // right menu
                _homeView.UserNameLabel.fontSize = Mathf.RoundToInt(_homeView.UserNameLabel.fontSize / fontSizeScaleFactor);
                _homeView.RecentlyViewed.fontSize = Mathf.RoundToInt(_homeView.RecentlyViewed.fontSize / fontSizeScaleFactor);
                _homeView.Favourites.fontSize = Mathf.RoundToInt(_homeView.Favourites.fontSize / fontSizeScaleFactor);
                _homeView.MyContent.fontSize = Mathf.RoundToInt(_homeView.MyContent.fontSize / fontSizeScaleFactor);
                _homeView.MyTeacher.fontSize = Mathf.RoundToInt(_homeView.MyTeacher.fontSize / fontSizeScaleFactor);
                _homeView.MetaData.fontSize = Mathf.RoundToInt(_homeView.MetaData.fontSize / fontSizeScaleFactor);

                _homeView.ChangePassword.fontSize = Mathf.RoundToInt(_homeView.ChangePassword.fontSize / fontSizeScaleFactor);
                _homeView.Logout.fontSize = Mathf.RoundToInt(_homeView.Logout.fontSize / fontSizeScaleFactor);
                _homeView.LanguageTitle.fontSize = Mathf.RoundToInt(_homeView.LanguageTitle.fontSize / fontSizeScaleFactor);
                _homeView.LanguageValue.fontSize = Mathf.RoundToInt(_homeView.LanguageValue.fontSize / fontSizeScaleFactor);
                _homeView.LanguageDropdown.itemText.fontSize = Mathf.RoundToInt(_homeView.LanguageDropdown.itemText.fontSize / fontSizeScaleFactor);
                _homeView.ArModeToggleText.fontSize = Mathf.RoundToInt(_homeView.ArModeToggleText.fontSize / fontSizeScaleFactor);
                
                // home -> right menu -> accessibility
                _homeView.AccessibilityTitle.fontSize = Mathf.RoundToInt(_homeView.AccessibilityTitle.fontSize / fontSizeScaleFactor);
                _homeView.AccessibilityFontSizeKey.fontSize = Mathf.RoundToInt(_homeView.AccessibilityFontSizeKey.fontSize / fontSizeScaleFactor);
                _homeView.AccessibilityFontSizeItem.fontSize = Mathf.RoundToInt(_homeView.AccessibilityFontSizeItem.fontSize / fontSizeScaleFactor);
                _homeView.AccessibilityFontSizeValue.fontSize = Mathf.RoundToInt(_homeView.AccessibilityFontSizeValue.fontSize / fontSizeScaleFactor);
                _homeView.AccessibilityTextToAudio.fontSize = Mathf.RoundToInt(_homeView.AccessibilityTextToAudio.fontSize / fontSizeScaleFactor);
                _homeView.AccessibilityGrayscale.fontSize = Mathf.RoundToInt(_homeView.AccessibilityGrayscale.fontSize / fontSizeScaleFactor);
                _homeView.AccessibilityLabelLines.fontSize = Mathf.RoundToInt(_homeView.AccessibilityLabelLines.fontSize / fontSizeScaleFactor);
                
                UpdateDescriptionFontSize();
            }
        }

        #endregion
        
        #region Description

        private void OnDescriptionClose(OnDescriptionCloseClickViewSignal signal)
        {
            _signal.Fire(new RemoveDescriptionFromArrayCommandSignal(signal.AssetId, signal.LabelId));
        }
        
        private void LoadAndPlayDescription(LoadAndPlayDescriptionViewSignal signal)
        {
            _signal.Fire(new PauseAllExceptActiveDescriptionCommandSignal(signal.Id));
            _signal.Fire(new LoadAndPlayDescriptionCommandSignal(signal.AudioSource, signal.CultureCode, signal.Description));
        }

        private void PauseAllExceptActiveDescription(PauseAllExceptActiveDescriptionViewSignal signal)
        {
            _signal.Fire(new PauseAllExceptActiveDescriptionCommandSignal(signal.Id));
        }
        
        private void UpdateDescriptionFontSize()
        {
            _signal.Fire<UpdateDescriptionFontSizeCommandSignal>();
        }
        
        private void UpdateDescriptionLanguage()
        {
            _signal.Fire(new GetLanguageChangedDescriptionViewCommandSignal(_localizationModel.CurrentLanguageCultureCode, false));
        }

        #endregion
    }
}