using System;
using System.Collections.Generic;
using TDL.Constants;
using TDL.Models;
using TDL.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace TDL.Views
{
    public class HomeViewMobileMediator : IInitializable, IDisposable
    {
        private HomeViewMobile _homeView;

        [Inject] private HomeViewBase.Factory _homeViewFactory;
        [Inject] private readonly SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        
        [Inject] private MetaDataModel _metaDataModel;
        [Inject] private UserLoginModel _userLoginModel;

        public void Initialize()
        {
            CreateView();
            SubscribeOnListeners();
            SaveContentPanels();
        }

        private void CreateView()
        {
            _homeView = (HomeViewMobile)_homeViewFactory.Create();
            _homeView.InitUiComponents();
            _signal.Fire(new RegisterScreenCommandSignal(WindowConstants.Home, _homeView.gameObject));
        }

        private void SubscribeOnListeners()
        {
            SubscribeHistory();
            _signal.Subscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
            _signal.Subscribe<OnHomeActivatedViewSignal>(CheckTerm);
            
            _userLoginModel.OnUserLogout += OnUserLogout;
            _contentModel.OnContentModelChanged += ShowHomeMobileScreen;
            
            _contentModel.OnGradeChanged += OnGradeSelected;

            _contentModel.OnSubjectChanged += OnSubjectSelected;
            _contentModel.OnTopicChanged += OnTopicSelected;
            _contentModel.OnSubtopicChanged += OnSubtopicSelected;
            _contentModel.OnActivityChanged += OnActivityChanged;
            _contentModel.OnChosenActivityChanged += OnChosenActivityChanged;
            _contentModel.OnCategoryWithHeadersChanged += OnCategoryWithHeadersChanged;
        
            _homeView.backButton.onClick.AddListener(OnBackButtonClick);
        
            _homeModel.OnNoSearchResultsChanged += SetNoSearchResultsStatus;
            _homeView.homeTabToggle.onValueChanged.AddListener(OnHomeToggleClick);
            _homeView.favoriteTabToggle.onValueChanged.AddListener(OnFavouritesToggleClick);
            _homeView.recentlyTabToggle.onValueChanged.AddListener(OnRecentToggleClick);
            _homeView.myContentToggle.onValueChanged.AddListener(OnMyContentToggleClick);
            _homeView.rightTabToggle.onValueChanged.AddListener(OnRightMenuClick);
        
            _homeView.languageButton.onClick.AddListener(() => _signal.Fire(new PopupOverlaySignal(true,"",false, PopupOverlayType.LanguageBox)));
        
            _homeView._imageRecognitionButton.onClick.AddListener(StartImageRecognition);

            _signal.Subscribe<SearchChangedViewSignal>(OnSearchChanged);
            _signal.Subscribe<CloseAppClickViewSignal>(OnCloseAppClick);
            _signal.Subscribe<OnLoginSuccessCommandSignal>(OnUserNameUpdated);
            _signal.Subscribe<SubjectHyperlinkClickViewSignal>(OnSubjectHyperlinkClick);
            _signal.Subscribe<TopicHyperlinkClickViewSignal>(OnTopicHyperlinkClick);
            _signal.Subscribe<SubtopicHyperlinkClickViewSignal>(OnSubtopicHyperlinkClick);
            _signal.Subscribe<ResetAllBreadcrumbsViewSignal>(ResetAllBreadcrumbs);
            _signal.Subscribe<OnChangeLanguageClickViewSignal>(OnChangeLanguageClick);
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

        private void OnUserLogout()
        {
            _homeView.homeTabToggle.isOn = true;
        }

        private void SubscribeOnHomeActivated()
        {
            _localizationModel.OnLanguageChanged += ChangeUiInterface;

            UpdateHomeLocalizationAtStart();
            
            _signal.Unsubscribe<OnHomeActivatedViewSignal>(SubscribeOnHomeActivated);
        }

        private void ShowHomeMobileScreen()
        {
            CreateGradesContent();

            _signal.Fire(new ShowHomeScreenCommandSignal());
            UpdateHomeScreenBreadcrumbs();
            
            ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.GradesKey).ToUpper()} ({_contentModel.GetGrades().Count})");
            HideMetaDataItemIfUnavailable();
            SetMyTeacherVisibility();
        }
        
#region CheckTerm

        private void CheckTerm()
        {
            AsyncProcessorService.Instance.Wait(0, () =>
            {
                if(_homeView.gameObject.activeSelf)
                    _signal.Fire(new OpenTermCommandSignal(true));
            });
        }

#endregion
        
        private void ShowTabTitle(bool status, string title = "")
        {
            _homeView.tabTitleText.gameObject.SetActive(status);
            
            if (status)
            {
                _homeView.tabTitleText.SetText(title);
            }
        }
        
        private void OnGradeSelected()
        {
            if(_homeModel.HomeTabFavouritesActive || _homeModel.HomeTabRecentActive)
                return;
            
            CreateSubjectsContent();
            UpdateGradeBreadcrumbs();
            ShowTabTitle(false);
        }

        private void OnSubjectSelected()
        {
            CreateTopicsContent();
            UpdateGradeSubjectBreadcrumbs();
            ShowTabTitle(false);
        }
        
        private void OnTopicSelected()
        {
            CreateSubtopicsContent();
            UpdateTopicBreadcrumbs();
        }

        private void OnSubtopicSelected()
        {
            CreateAssets();
            UpdateSubtopicBreadcrumbs();
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

        private void UpdateHomeScreenBreadcrumbs()
        {
            _homeView.SetBreadcrumbsVisibility(false);

            _homeView.ResetAllBreadcrumbs();
        }
     
        private void UpdateGradeBreadcrumbs()
        {
            _homeView.SetBreadcrumbsVisibility(true);

            _homeView.Grade.text = _contentModel.SelectedGrade.Grade.name;
            _homeView.SubjectText.text = $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.SubjectsKey).ToUpper()} ({_contentModel.SelectedGrade.Grade.Subjects.Count})";
            _homeView.backButton.gameObject.SetActive(true);
            _homeView.ResetTopicBreadcrumb();
            _homeView.ResetSubtopicBreadcrumb();
        }
        
        private void UpdateGradeSubjectBreadcrumbs()
        {
            _homeView.SetBreadcrumbsVisibility(true);

            _homeView.Grade.text = _contentModel.SelectedGrade.Grade.name;
            _homeView.SubjectText.text = _contentModel.SelectedSubject.Subject.name;
            _homeView.backButton.gameObject.SetActive(true);
            _homeView.ResetTopicBreadcrumb();
            _homeView.ResetSubtopicBreadcrumb();
            _breadcrumbs.Clear();
            UpdateBreadcrumbs((_contentModel.SelectedSubject.Subject.name, 0));
        }
        
        private void UpdateTopicBreadcrumbs()
        {
            UpdateBreadcrumbs((_contentModel.SelectedTopic.Topic.name,1));
        }

        private void UpdateSubtopicBreadcrumbs()
        {
            UpdateBreadcrumbs((_contentModel.SelectedSubtopic.Subtopic.name,2));
        }
        
        private void UpdateActivitiesBreadcrumbs()
        {
            var name = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ActivitiesKey);
            UpdateBreadcrumbs((name,3));
        }

        private void UpdateChosenActivityBreadcrumbs()
        {
            var name = _localizationModel.GetCurrentSystemTranslations(_contentModel.SelectedChosenActivity.ActivityName);
            UpdateBreadcrumbs((name,4));
        }

        Stack<(string name, int level)> _breadcrumbs = new Stack<(string name, int level)>();
        private void UpdateBreadcrumbs((string name, int level) input)
        {
            if (input.level == 0)
            {
                _breadcrumbs.Push(input);
            }
            else
            {
                _homeView.SetBreadcrumbsVisibility(true);
                _homeView.TopicText.text = input.name;
                _homeView.TopicText.gameObject.SetActive(true);
                _homeView.ShowTopicDivider(true);
                _homeView.ResetSubtopicBreadcrumb();
            }

            if (input.level > _breadcrumbs.Peek().level)
            {
                _homeView.SubjectText.text = _breadcrumbs.Peek().name;
                
                if(!input.Equals(_breadcrumbs.Peek()))
                    _breadcrumbs.Push(input);
            }
            else if (input.level < _breadcrumbs.Peek().level)
            {
                _breadcrumbs.Pop();
                _breadcrumbs.Pop();
                _homeView.SubjectText.text = _breadcrumbs.Peek().name;
                
                if(!input.Equals(_breadcrumbs.Peek()))
                    _breadcrumbs.Push(input);
            }
        }
        
        private bool _homeStatus = true;
        public void OnHomeToggleClick(bool status)
        {
            if (_homeStatus == status)
            {
                _contentModel.SelectedGrade = null;
                _contentModel.SelectedSubject = null;
                ShowHomeMobileScreen();
            }
            else if(status)
            {
                _homeModel.HomeTabFavouritesActive = false;
                _homeModel.HomeTabRecentActive = false;
                _homeModel.HomeTabMyContentActive = false;
                OnHomeTabClick();
                
                _homeView.ClearSearchInput();
            }

            _homeStatus = status;
        }
        
        public void OnFavouritesToggleClick(bool status)
        {
            if(!status) return;

            if (!_homeModel.HomeTabFavouritesActive)
            {
                ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FavouritesKey).ToUpper()} ({_contentModel.FavoriteAssets.Count})");
                _homeView.SetBreadcrumbsVisibility(false);

                _signal.Fire<ActivateHomeTabFavouritesCommandSignal>();
                _signal.Fire(new FavoritesClickFromHomeTabsViewSignal(status));
                
                _homeView.ClearSearchInput();
            }
        }
        
        public void OnRecentToggleClick(bool status)
        {
            if(!status) return;

            if (!_homeModel.HomeTabRecentActive)
            {
                ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.RecentlyViewedKey).ToUpper()} ({_contentModel.GetRecentAssetsCount()})");
                _homeView.SetBreadcrumbsVisibility(false);

                _signal.Fire<ActivateHomeTabRecentCommandSignal>();
                _signal.Fire(new RecentlyViewedClickFromHomeTabsViewSignal(status));
                
                _homeView.ClearSearchInput();
            }
        }

        public void OnMyContentToggleClick(bool status)
        {
            if(!status) return;

            if (!_homeModel.HomeTabMyContentActive)
            {
                ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyContentKey).ToUpper()}");
                _homeView.SetBreadcrumbsVisibility(false);

                _signal.Fire<ActivateHomeTabMyContentCommandSignal>();
                _signal.Fire(new MyContentViewedClickFromHomeTabsViewSignal(status));
                
                _homeView.ClearSearchInput();
            }
        }
        
        public void OnRightMenuClick(bool status)
        {
            _homeView._rightMenu.SetActive(status);
            _homeView.SearchPanel.gameObject.SetActive(!status);
            
            if(status)
                _contentModel.ClearSelectedCategories();
            
            _homeView.ClearSearchInput();
        }
        
        private void ShowHomeScreen()
        {
            _signal.Fire(new ShowHomeScreenCommandSignal());
        }
        
        private void CreateAssets()
        {
            _signal.Fire(new CreateAssetsCommandSignal());
        }

        private void SaveContentPanels()
        {
            _signal.Fire(new SaveContentPanelsCommandSignal(
                null,
                _homeView.GetTopicsSubtopicsContent(),
                _homeView.GetAssetsContent()));
        }
        
        private void CreateGradesContent()
        {
            _signal.Fire(new CreateGradesContentCommandSignal());
        }
        
        private void CreateSubjectsContent()
        {
            _signal.Fire(new CreateSubjectsContentCommandSignal());
        }
        
        private void CreateSubtopicsContent()
        {
            _signal.Fire(new CreateSubtopicsContentCommandSignal());
        }
        
        private void CreateTopicsContent()
        {
            _signal.Fire(new CreateTopicsContentCommandSignal());
        }
        
        public void ResetAllBreadcrumbs()
        {
            _homeView.ResetAllBreadcrumbs();
        }

        private void OnSearchChanged(SearchChangedViewSignal signal)
        {
            var searchValue = signal.SearchValue;

            if (string.IsNullOrEmpty(searchValue))
            {
                ShowTabTitle(false);
                _signal.Fire<ShowLastShownCategoryCommandSignal>();
                ShowTabTitleForLastShownCategory();
            }
            else
            {
                _signal.Subscribe<ShowSearchAssetsCommandSignal>(OnSearchShowTabTitle);
                _signal.Fire(new GetSearchAssetsCommandSignal(searchValue, _localizationModel.CurrentLanguageCultureCode));
                _homeView.SetBreadcrumbsVisibility(false);
            }
        }

        private void OnSearchShowTabTitle(ShowSearchAssetsCommandSignal signal)
        {
            _signal.Unsubscribe<ShowSearchAssetsCommandSignal>(OnSearchShowTabTitle);
            int i = _contentModel.GetAssetsByIds(signal.FoundedAssetIds).Count;
            ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.SearchResultsKey).ToUpper()} ({i})");
        }

        private void ShowTabTitleForLastShownCategory()
        {
            var lastShownCategorySignal = _homeModel.LastShownCategory;

            if(lastShownCategorySignal is CreateSubjectsContentCommandSignal)
            {
                ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.SubjectsKey).ToUpper()} ({_contentModel.GetAllSubjects().Count})");
                _homeView.SetBreadcrumbsVisibility(false);
            }
            else if (lastShownCategorySignal is ShowRecentlyViewedCommandSignal)
            {
                ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.RecentlyViewedKey).ToUpper()} ({_contentModel.GetRecentAssetsCount()})");
                _homeView.SetBreadcrumbsVisibility(false);
            }
            else if (lastShownCategorySignal is ShowFavouritesCommandSignal)
            {
                ShowTabTitle(true, $"{_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FavouritesKey).ToUpper()} ({_contentModel.FavoriteAssets.Count})");
                _homeView.SetBreadcrumbsVisibility(false);
            }
            else
            {
                ShowTabTitle(false);
                _homeView.SetBreadcrumbsVisibility(true);
            }
        }

        private void OnSubjectHyperlinkClick()
        {
            if (_contentModel.SelectedSubject != null)
            {
                _signal.Fire(new SubjectMenuItemClickCommandSignal(_contentModel.SelectedSubject.ParentGrade.Grade.id,
                    _contentModel.SelectedSubject.Subject.id, true));
            
                _contentModel.SelectedSubtopic = null;
                _contentModel.SelectedTopic = null;
                _signal.Fire(new ShowDebugLogCommandSignal($"@ null = SubTopic | Topic  >>>  subtopic = <<{_contentModel.SelectedSubtopic}>>  || topic = <<{_contentModel.SelectedTopic}>> "));
            }
        }
        
        private void OnTopicHyperlinkClick()
        {
            if (_contentModel.SelectedTopic != null)
            {
                _contentModel.SelectedSubtopic = null;
                _signal.Fire(new ShowDebugLogCommandSignal("@ null = SubTopic"));

                _signal.Fire(new TopicItemClickCommandSignal(_contentModel.SelectedTopic.ParentSubject.Subject.id,
                    _contentModel.SelectedTopic.Topic.id));
            }
        }

        private void OnSubtopicHyperlinkClick()
        {
            if (_contentModel.SelectedSubtopic != null)
            {
                _signal.Fire(new ShowDebugLogCommandSignal("@ OnSubtopicHyperlinkClick()"));
            }
        }

        private void OnUserNameUpdated()
        {
            _homeView.SetUserName(_userLoginModel.FirstLetter, _userLoginModel.Firstname, _userLoginModel.Lastname);
        }
        
        public void Dispose()
        {
            UnsubscribeFromHomeActivated();
            UnsubscribeHistory();

            if (_userLoginModel != null)
            {
                _userLoginModel.OnUserLogout -= OnUserLogout;
            }

            if (_homeModel != null)
            {
                _homeModel.OnNoSearchResultsChanged -= SetNoSearchResultsStatus;
            }

            if (_contentModel != null)
            {
                _contentModel.OnContentModelChanged -= ShowHomeScreen;
                _contentModel.OnGradeChanged -= OnGradeSelected;
                _contentModel.OnSubjectChanged -= OnSubjectSelected;
                _contentModel.OnTopicChanged -= OnTopicSelected;
                _contentModel.OnSubtopicChanged -= OnSubtopicSelected;
                _contentModel.OnActivityChanged -= OnActivityChanged;
                _contentModel.OnChosenActivityChanged -= OnChosenActivityChanged;
            }
        
            _signal.Unsubscribe<OnLoginSuccessCommandSignal>(OnUserNameUpdated);
            _signal.Unsubscribe<SearchChangedViewSignal>(OnSearchChanged);
            _signal.Unsubscribe<ResetAllBreadcrumbsViewSignal>(ResetAllBreadcrumbs);
            _signal.Unsubscribe<SubjectHyperlinkClickViewSignal>(OnSubjectHyperlinkClick);
            _signal.Unsubscribe<TopicHyperlinkClickViewSignal>(OnTopicHyperlinkClick);
            _signal.Unsubscribe<SubtopicHyperlinkClickViewSignal>(OnSubtopicHyperlinkClick);
            _signal.Unsubscribe<CloseAppClickViewSignal>(OnCloseAppClick);
        }
        
        private void UnsubscribeFromHomeActivated()
        {
            if (_localizationModel != null)
            {
                _localizationModel.OnLanguageChanged -= ChangeUiInterface;
            }

            _signal.Unsubscribe<OnChangeLanguageClickViewSignal>(OnChangeLanguageClick);
        }
        
        private void OnCloseAppClick()
        {
            _signal.Fire(new CloseAppCommandSignal());
        }

        #region History

        private List<ISignal> _lastShownCategory = new List<ISignal>();

        private void SubscribeHistory()
        {
            _signal.Subscribe<GradeMenuItemClickViewSignal>(AddHistory);
            _signal.Subscribe<SubjectMenuItemClickViewSignal>(AddHistory);
            _signal.Subscribe<TopicItemClickViewSignal>(AddHistory);
            _signal.Subscribe<SubtopicItemClickViewSignal>(AddHistory);
            
            _signal.Subscribe<ActivityItemClickViewSignal>(AddHistory);
            _signal.Subscribe<ActivityQuizClickViewSignal>(AddHistory);
            _signal.Subscribe<ActivityPuzzleClickViewSignal>(AddHistory);
            _signal.Subscribe<ActivityMultipleQuizItemClickViewSignal>(AddHistory);
            _signal.Subscribe<ActivityMultiplePuzzlesItemClickViewSignal>(AddHistory);
            _signal.Subscribe<ActivityClassificationItemClickViewSignal>(AddHistory);
        }
        
        private void UnsubscribeHistory()
        {
            _signal.Unsubscribe<GradeMenuItemClickViewSignal>(AddHistory);
            _signal.Unsubscribe<SubjectMenuItemClickViewSignal>(AddHistory);
            _signal.Unsubscribe<TopicItemClickViewSignal>(AddHistory);
            _signal.Unsubscribe<SubtopicItemClickViewSignal>(AddHistory);
            
            _signal.Unsubscribe<ActivityItemClickViewSignal>(AddHistory);
            _signal.Unsubscribe<ActivityQuizClickViewSignal>(AddHistory);
            _signal.Unsubscribe<ActivityPuzzleClickViewSignal>(AddHistory);
            _signal.Unsubscribe<ActivityMultipleQuizItemClickViewSignal>(AddHistory);
            _signal.Unsubscribe<ActivityMultiplePuzzlesItemClickViewSignal>(AddHistory);
            _signal.Unsubscribe<ActivityClassificationItemClickViewSignal>(AddHistory);
        }

        private void AddHistory(ISignal obj)
        {
            // need create null signal
            ISignal lastSignal = new CloseAppCommandSignal();
            
            if (HistoryContainItems())
                lastSignal = GetLastItemFromHistory();

            if (lastSignal != obj)
            {
                _lastShownCategory.Add(obj);
            }
        }
        
        private void OnBackButtonClick()
        {
            if(HistoryContainItems())
                UndoHistory();

            if (HistoryContainItems())
            {
                var signal = GetLastItemFromHistory();
               
                UndoHistory();
                FireHistory(signal);
            }
            else
            {
                _contentModel.SelectedGrade = null;
                _contentModel.SelectedSubject = null;
                ShowHomeMobileScreen();
            }
        }
        
        private void OnHomeTabClick()
        {
            ShowHomeMobileScreen();

            if (HistoryContainItems())
            {
                List<ISignal> tempList = new List<ISignal>(_lastShownCategory);
                
                foreach (ISignal signal in tempList)
                {
                    FireHistory(signal);
                    _lastShownCategory.Remove(signal);
                }
            }
        }

        private void FireHistory(ISignal signal)
        {
            switch (signal)
            {
                case GradeMenuItemClickViewSignal s:
                    _contentModel.SelectedGrade = null;
                    _contentModel.SelectedSubject = null;
                    _signal.Fire(new GradeMenuItemClickViewSignal(s.Id, s.Id));
                    break;
                
                case SubjectMenuItemClickViewSignal s:
                    _contentModel.SelectedSubject = null;
                    _signal.Fire(new SubjectMenuItemClickViewSignal(s.ParentId, s.Id));
                    break;
                
                case TopicItemClickViewSignal s:
                    _contentModel.SelectedTopic = null;
                    _signal.Fire(new TopicItemClickViewSignal(s.ParentId, s.Id));
                    break;
                
                case SubtopicItemClickViewSignal s:
                    _contentModel.SelectedSubtopic = null;
                    _signal.Fire(new SubtopicItemClickViewSignal(s.ParentId, s.Id));
                    break;
                
                case ActivityItemClickViewSignal s:
                    _signal.Fire(new ActivityItemClickViewSignal());
                    break;
                
                case ActivityQuizClickViewSignal s:
                    _signal.Fire(new ActivityQuizClickViewSignal());
                    break;
                
                case ActivityPuzzleClickViewSignal s:
                    _signal.Fire(new ActivityPuzzleClickViewSignal());
                    break;
                
                case ActivityMultipleQuizItemClickViewSignal s:
                    _signal.Fire(new ActivityMultipleQuizItemClickViewSignal());
                    break;
                
                case ActivityMultiplePuzzlesItemClickViewSignal s:
                    _signal.Fire(new ActivityMultiplePuzzlesItemClickViewSignal());
                    break;
                
                case ActivityClassificationItemClickViewSignal s:
                    _signal.Fire(new ActivityClassificationItemClickViewSignal());
                    break;
            }
        }
        
        private void UndoHistory()
        {
            RemoveLastItemFromHistory();
        }

        private ISignal GetLastItemFromHistory()
        {
            return _lastShownCategory[GetLastIndexFromHistory()];
        }
        
        private void RemoveLastItemFromHistory()
        {
            if (HistoryContainItems())
                _lastShownCategory.RemoveAt(GetLastIndexFromHistory());
        }
        
        private int GetLastIndexFromHistory()
        {
            return _lastShownCategory.Count - 1;
        }
        
        private bool HistoryContainItems()
        {
            return _lastShownCategory.Count > 0;
        }

        #endregion
        
        #region Localization

        private void OnChangeLanguageClick(OnChangeLanguageClickViewSignal signal)
        {
            _signal.Fire(new ChangeLanguageCommandSignal(signal.LanguageIndex));
        }

        private void UpdateHomeLocalizationAtStart()
        {
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

        private void ChangeUiInterface()
        {
            _homeView.languageDropdownText?.SetText(_localizationModel.GetLanguageNameByCultureCode(_localizationModel.CurrentLanguageCultureCode));
            // top menu
            _homeView.SearchPlaceholder.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.PlaceholderSearchKey);
            _homeView.NoSearchResultsFound.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.NoSearchResultsKey);

            // home tabs
            _homeView.TabFavouritesText?.SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.FavouritesKey));
            _homeView.TabRecentText?.SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.RecentKey));
            _homeView.TabMyContentText?.SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyContentKey));
            
            // right menu
            _homeView.ChangePassword?.SetText( _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ChangePasswordKey));
            _homeView.Logout?.SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LogoutKey));
            _homeView?.LanguageTitle.SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.LanguageTitleKey));
            _homeView?._closeAppButton.GetComponentInChildren<TextMeshProUGUI>().SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.CloseAppKey));
            _homeView.accessibilityLabelLines.GetComponentInChildren<TextMeshProUGUI>().SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AccessibilityLabelLinesKey));
            _homeView?.MyTeacher.SetText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MyTeacherKey)); 

            // category with headers
            _homeView.ActivityHeaderText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.ActivityWithHeadersKey); 
            _homeView.LearnTechHeaderText.text = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.AssetsWithHeadersKey); 
            
            if (_metaDataModel.LinkLocal != null)
            {
                _homeView.MetaData.text = _metaDataModel.LinkLocal.ContainsKey(_localizationModel.CurrentLanguageCultureCode)
                    ? _metaDataModel.LinkLocal[_localizationModel.CurrentLanguageCultureCode]
                    : _metaDataModel.LinkLocal[_localizationModel.FallbackCultureCode];   
            }
        }

        #endregion
        
        #region Meta Data

        private void HideMetaDataItemIfUnavailable()
        {
            if (_metaDataModel.Link == null)
            {
                _homeView.MetaDataEmptySeparator.SetActive(false);
                _homeView.MetaDataButton.gameObject.SetActive(false);
            }
        }
        
        #endregion

        #region MyTeacher

        private void SetMyTeacherVisibility()
        {
            _homeView.MyTeacherEmptySeparator.gameObject.SetActive(false);
            _homeView.MyTeacher.gameObject.SetActive(false);
        }

        #endregion
        
        #region No Search Results

        private void SetNoSearchResultsStatus(bool status)
        {
            _homeView.NoSearchResultsFound.gameObject.SetActive(status);
        }

        #endregion
    }
}