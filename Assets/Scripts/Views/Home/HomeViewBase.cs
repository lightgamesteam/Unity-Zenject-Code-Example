using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class HomeViewBase : ViewBase
    {
        [Header("Top menu")] 
        [SerializeField] private Toggle _leftMenuToggle;
        [SerializeField] private Toggle _rightMenuToggle;
        [SerializeField] protected internal Button _closeAppButton;
        public TextMeshProUGUI UserIconLabel { get; set; }
        public TextMeshProUGUI CloseAppText { get; set; }
        public Button _imageRecognitionButton;

        [Header("Left menu")] 
        [SerializeField] private GameObject _leftMenu;
        [SerializeField] private GameObject _leftMenuContent;
        
        [Header("Right menu")] 
        [SerializeField] protected internal GameObject _rightMenu;
        public TextMeshProUGUI UserNameLabel { get; set; }
        
        public Toggle ArModeToggle { get; set; }
        public TextMeshProUGUI ArModeToggleText { get; set; }

        [Header("Body content")] 
        [SerializeField] private Transform _topicsSubtopicsContent;
        [SerializeField] private Transform _assetsContent;

        // breadcrumbs
        protected Transform _breadcrumbsPanel { get; set; }
        public TextMeshProUGUI Grade { get; set; }
        protected Button _subjectHyperlink;
        public TextMeshProUGUI SubjectText { get; set; }
        protected GameObject _topicDivider;
        protected Button _topicHyperlink;
        public TextMeshProUGUI TopicText { get; set; }
        protected GameObject _subtopicDivider;
        protected Button _subtopicHyperlink;
        public TextMeshProUGUI SubtopicText { get; set; }
        private GameObject _activitiesDivider;
        private Button _activitiesHyperlink;
        public TextMeshProUGUI ActivitiesText { get; set; }
        private GameObject _chosenActivityDivider;
        public TextMeshProUGUI ChosenActivityText { get; set; }
        
        // category with headers
        public TextMeshProUGUI ActivityHeaderText { get; set; }
        public TextMeshProUGUI LearnTechHeaderText { get; set; }

        // top menu
        public Transform SearchPanel { get; set; }
        public Button SearchOpenButton { get; set; }
        public Button SearchCloseButton { get; set; }
        public Button SearchGoButton { get; set; }
        public InputField SearchInput { get; set; }
        public Text SearchPlaceholder { get; set; }
        public TextMeshProUGUI NoSearchResultsFound { get; set; }

        // home tab favourites
        private Transform _tabsPanel;
        public Button TabFavouritesButton { get; set; }
        public TextMeshProUGUI TabFavouritesText { get; set; }
        public Image TabFavouritesUnderline { get; set; }

        // home tab recent
        public Button TabRecentButton { get; set; }
        public TextMeshProUGUI TabRecentText { get; set; }
        public Image TabRecentUnderline { get; set; }
        
        // home tab myContent
        public Button TabMyContentButton { get; set; }
        public TextMeshProUGUI TabMyContentText { get; set; }
        public Image TabMyContentUnderline { get; set; }
        
        // home tab myTeacher
        public Button TabMyTeacherButton { get; set; }
        public TextMeshProUGUI TabMyTeacherText { get; set; }
        public Image TabMyTeacherUnderline { get; set; }

        // right menu ui
        public TextMeshProUGUI RecentlyViewed { get; set; }
        public TextMeshProUGUI Favourites { get; set; }
        public TextMeshProUGUI MyContent { get; set; }
        public TextMeshProUGUI MyTeacher { get; set; }

        public TextMeshProUGUI MetaData { get; set; }
        public TextMeshProUGUI ChangePassword { get; set; }
        public TextMeshProUGUI Logout { get; set; }
        public TextMeshProUGUI LanguageTitle { get; set; }
        public TextMeshProUGUI LanguageValue { get; set; }
        public TMP_Dropdown LanguageDropdown { get; set; }
        
        // accessibility ui
        public TextMeshProUGUI AccessibilityTitle { get; set; }
        public TextMeshProUGUI AccessibilityTextToAudio { get; set; }
        public TextMeshProUGUI AccessibilityGrayscale { get; set; }
        public TextMeshProUGUI AccessibilityLabelLines { get; set; }
        public TextMeshProUGUI AccessibilityFontSizeKey { get; set; }
        public TMP_Text AccessibilityFontSizeItem { get; set; }
        public TextMeshProUGUI AccessibilityFontSizeValue { get; set; }
        
        private readonly Color HomeTabActive = new Color(0.33f, 0.40f, 0.63f);
        private readonly Color HomeTabInactive = new Color(0.0f, 0.0f, 0.0f);

        public override void InitUiComponents()
        {
            // breadcrumbs
            _breadcrumbsPanel = transform.Get<Transform>("Content/BreadcrumbsPanel");
            Grade = _breadcrumbsPanel.Get<TextMeshProUGUI>("Grade");
            var contentPanel = _breadcrumbsPanel.Get<Transform>("ContentPanel");
            _subjectHyperlink = contentPanel.Get<Button>("Subject");
            SubjectText = _subjectHyperlink.GetComponent<TextMeshProUGUI>();
            _topicDivider = contentPanel.Get<Transform>("TopicDivider").gameObject;
            _topicHyperlink = contentPanel.Get<Button>("Topic");
            TopicText = _topicHyperlink.GetComponent<TextMeshProUGUI>();
            _subtopicDivider = contentPanel.Get<Transform>("SubtopicDivider").gameObject;
            _subtopicHyperlink = contentPanel.Get<Button>("Subtopic");
            SubtopicText = _subtopicHyperlink.GetComponent<TextMeshProUGUI>();
            _activitiesDivider = contentPanel.Get<Transform>("ActivitiesDivider").gameObject;
            _activitiesHyperlink = contentPanel.Get<Button>("Activities");
            ActivitiesText = _activitiesHyperlink.GetComponent<TextMeshProUGUI>();
            _chosenActivityDivider = contentPanel.Get<Transform>("ChosenActivityDivider").gameObject;
            ChosenActivityText = contentPanel.Get<TextMeshProUGUI>("ChosenActivity");

            // top menu
            SearchPanel = transform.Get<Transform>("TopMenu/SearchPanel");
            SearchOpenButton = transform.Get<Button>("TopMenu/SearchIconPanel/OpenSearch");
            SearchCloseButton = SearchPanel.Get<Button>("SearchInputField/CloseSearch");
            SearchGoButton = SearchPanel.Get<Button>("SearchInputField/GoButton");
            NoSearchResultsFound = transform.Get<TextMeshProUGUI>("Content/NoSearchResultsPanel/NoSearchResultsText");
            _imageRecognitionButton = transform.Get<Button>("TopMenu/SearchIconPanel/OpenImageRecognition");
            SearchInput = SearchPanel.Get<InputField>("SearchInputField");
            SearchPlaceholder = SearchInput.transform.Get<Text>("TextViewPort/Placeholder");
            CloseAppText = transform.Get<Transform>("TopMenu/CloseAppPanel").GetComponentInChildren<TextMeshProUGUI>();
            UserIconLabel = transform.Get<Transform>("TopMenu/RightMenuPanel/RightMenuToggle/UserIconLabel").GetComponentInChildren<TextMeshProUGUI>();

            // home tab favourites
            _tabsPanel = transform.Get<Transform>("Content/TabsPanel");
            TabFavouritesButton = _tabsPanel.Get<Button>("FavouritesTab");
            TabFavouritesText = TabFavouritesButton.transform.Get<TextMeshProUGUI>("Title");
            TabFavouritesUnderline = TabFavouritesButton.transform.Get<Image>("Underline");

            // home tab recent
            TabRecentButton = _tabsPanel.Get<Button>("RecentTab");
            TabRecentText = TabRecentButton.transform.Get<TextMeshProUGUI>("Title");
            TabRecentUnderline = TabRecentButton.transform.Get<Image>("Underline");
            
            // home tab myContent
            TabMyContentButton = _tabsPanel.Get<Button>("MyContentTab");
            TabMyContentText = TabMyContentButton.transform.Get<TextMeshProUGUI>("Title");
            TabMyContentUnderline = TabMyContentButton.transform.Get<Image>("Underline");
            
            // home tab myTeacher
            TabMyTeacherButton = _tabsPanel.Get<Button>("MyTeacherTab");
            TabMyTeacherText = TabMyTeacherButton.transform.Get<TextMeshProUGUI>("Title");
            TabMyTeacherUnderline = TabMyTeacherButton.transform.Get<Image>("Underline");

            // right menu
            UserNameLabel = transform.Get<TextMeshProUGUI>("RightMenu/UserName");
            var optionsPanel = transform.Get<Transform>("RightMenu/OptionsPanel");
            RecentlyViewed = optionsPanel.Get<TextMeshProUGUI>("RecentlyViewed/Text");
            Favourites = optionsPanel.Get<TextMeshProUGUI>("Favourites/Text");
            MyContent = optionsPanel.Get<TextMeshProUGUI>("MyContent/Text");
            MyTeacher = optionsPanel.Get<TextMeshProUGUI>("MyTeacher/Text");
            ChangePassword = optionsPanel.Get<TextMeshProUGUI>("ChangePassword/Text");
            MetaData = optionsPanel.Get<TextMeshProUGUI>("MetaData/Text");
            Logout = transform.Get<TextMeshProUGUI>("RightMenu/SignOut/Text");
            LanguageTitle = transform.Get<TextMeshProUGUI>("RightMenu/LanguagePanel/LanguageTitle");
            LanguageDropdown = transform.Get<TMP_Dropdown>("RightMenu/LanguagePanel/LanguageDropdown");
            LanguageValue = LanguageDropdown.transform.Get<TextMeshProUGUI>("Label");
            
            ArModeToggle = transform.Get<Toggle>("RightMenu/OptionsPanel/AR_Mode");
            ArModeToggle.isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.ARmodeSettings);
            ArModeToggleText = ArModeToggle.GetComponentInChildren<TextMeshProUGUI>();
            
            var accessibilityContainer = optionsPanel.Get<Transform>("Accessibility/Container");
            AccessibilityTitle = optionsPanel.Get<TextMeshProUGUI>("Accessibility/Title");
            AccessibilityFontSizeKey = accessibilityContainer.Get<TextMeshProUGUI>("AccessibilityFontSize/Title");
            AccessibilityFontSizeValue = accessibilityContainer.Get<TextMeshProUGUI>("AccessibilityFontSize/FontSizeDropdown/Label");
            AccessibilityFontSizeItem = accessibilityContainer.Get<TMP_Dropdown>("AccessibilityFontSize/FontSizeDropdown").itemText;
            AccessibilityTextToAudio = accessibilityContainer.Get<TextMeshProUGUI>("TextToAudio/Text");
            AccessibilityGrayscale = accessibilityContainer.Get<TextMeshProUGUI>("Grayscale/Text");
            AccessibilityLabelLines = accessibilityContainer.Get<TextMeshProUGUI>("LabelLines/Text");
            
            // category with headers
            var bodyContent = transform.Get<Transform>("Content/AssetsPanel/Viewport/Content"); 
            ActivityHeaderText = bodyContent.Get<TextMeshProUGUI>("<--ActivityHeader-->");
            LearnTechHeaderText = bodyContent.Get<TextMeshProUGUI>("<--LearnTechHeader-->");
            
            _closeAppButton.transform.parent.gameObject.SetActive(false);
        }

        public override void SubscribeOnListeners()
        {
            Signal.Fire(new OnHomeActivatedViewSignal());

            _leftMenuToggle.onValueChanged.AddListener(OnLeftMenuClick);
            _rightMenuToggle.onValueChanged.AddListener(OnRightMenuClick);
            _closeAppButton.onClick.AddListener(OnCloseAppClick);
            _subjectHyperlink.onClick.AddListener(OnSubjectHyperlinkClick);
            _topicHyperlink.onClick.AddListener(OnTopicHyperlinkClick);
            _subtopicHyperlink.onClick.AddListener(OnSubtopicHyperlinkClick);
            _activitiesHyperlink.onClick.AddListener(OnActivitiesHyperlinkClick);
            LanguageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);

            // top menu
            SearchOpenButton.onClick.AddListener(OnSearchOpenClick);
            SearchCloseButton.onClick.AddListener(OnSearchCloseClick);
            SearchGoButton.onClick.AddListener(OnSearchGoClick);
            SearchInput.onEndEdit.AddListener(OnSearchEnterPressKey);
            SearchInput.onValueChanged.AddListener(OnSearchInputChanged);

            // home tab
            TabFavouritesButton.onClick.AddListener(OnHomeTabFavouritesClick);
            TabRecentButton.onClick.AddListener(OnHomeTabRecentClick);
            TabMyContentButton.onClick.AddListener(OnHomeTabMyContentClick);
            TabMyTeacherButton.onClick.AddListener(OnHomeTabMyTeacherClick);
            
            // right menu
            ArModeToggle.onValueChanged.AddListener(ChangeARmodeSettings);
        }

        public override void UnsubscribeFromListeners()
        {
            Signal.Fire(new OnHomeDeactivatedViewSignal());

            _leftMenuToggle.onValueChanged.RemoveAllListeners();
            _rightMenuToggle.onValueChanged.RemoveAllListeners();
            _closeAppButton.onClick.RemoveAllListeners();
            _subjectHyperlink.onClick.RemoveAllListeners();
            _topicHyperlink.onClick.RemoveAllListeners();
            _subtopicHyperlink.onClick.RemoveAllListeners();
            _activitiesHyperlink.onClick.RemoveAllListeners();
            LanguageDropdown.onValueChanged.RemoveAllListeners();

            // top menu
            SearchOpenButton.onClick.RemoveAllListeners();
            SearchCloseButton.onClick.RemoveAllListeners();
            SearchGoButton.onClick.RemoveAllListeners();
            SearchInput.onEndEdit.RemoveAllListeners();
            SearchInput.onValueChanged.RemoveAllListeners();

            // home tabs
            TabFavouritesButton.onClick.RemoveAllListeners();
            TabRecentButton.onClick.RemoveAllListeners();
            TabMyContentButton.onClick.RemoveAllListeners();
            TabMyTeacherButton.onClick.RemoveAllListeners();
            
            //right menu
            ArModeToggle.onValueChanged.RemoveAllListeners();
        }

        #region Search

        private void OnSearchOpenClick()
        {
            if (!SearchInput.gameObject.activeSelf)
            {
                SearchInput.gameObject.SetActive(true);
                SearchInput.Select();
            }
        }

        public void OnSearchCloseClick()
        {
            SearchInput.text = string.Empty;
            SearchInput.gameObject.SetActive(false);
            OnSearchGoClick();
        }
        
        protected void OnSearchInputChanged(string searchValue)
        {
            var canSearch = SearchInput.text.Length >= SearchConstants.MinimumSearchSymbols;
            SearchGoButton.interactable = canSearch;
            if (canSearch)
            {
                OnSearchGoClick();
            }
        }
        
        protected void OnSearchGoClick()
        {
            Signal.Fire(new SearchChangedViewSignal(SearchInput.text));
        }

        private void OnSearchEnterPressKey(string textValue)
        {
            if (IsSearchAllowed())
            {
                var isEnterPressed = Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return);
                if (isEnterPressed)
                {
                    OnSearchGoClick();
                }   
            }
        }

        private bool IsSearchAllowed()
        {
            return SearchGoButton.interactable;
        }

        public void HideSearchInput()
        {
            if (SearchInput.gameObject.activeSelf)
            {
                SearchInput.text = string.Empty;
                SearchInput.gameObject.SetActive(false);
            }
        }

        #endregion

        private void OnLeftMenuClick(bool isEnabled)
        {
            Signal.Fire(new LeftMenuClickViewSignal(isEnabled));
        }

        private void OnRightMenuClick(bool isEnabled)
        {
            Signal.Fire(new RightMenuClickViewSignal(isEnabled));
        }

        protected void OnCloseAppClick()
        {
            Signal.Fire(new CloseAppClickViewSignal());
        }

        private void OnSubjectHyperlinkClick()
        {
            Signal.Fire(new SubjectHyperlinkClickViewSignal());
        }

        private void OnTopicHyperlinkClick()
        {
            Signal.Fire(new TopicHyperlinkClickViewSignal());
        }

        private void OnSubtopicHyperlinkClick()
        {
            Signal.Fire(new SubtopicHyperlinkClickViewSignal());
        }
        
        private void OnActivitiesHyperlinkClick()
        {
            Signal.Fire(new ActivityHyperlinkClickViewSignal());
        }
        
        protected void ChangeARmodeSettings(bool value)
        {
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.ARmodeSettings, value, true);
        }

        public void ShowLeftMenu(bool status)
        {
            if (_leftMenu.activeSelf != status)
            {
                _leftMenu.SetActive(status);
                _leftMenuToggle.isOn = status;
            }
        }

        public virtual void ShowRightMenu(bool status)
        {
            if (_rightMenu.activeSelf != status)
            {
                _rightMenu.SetActive(status);
                _rightMenuToggle.isOn = status;
            }
        }

        public void CloseLanguageDropdownIfOpened()
        {
            var isOpened = LanguageDropdown.IsExpanded;
            if (isOpened)
            {
                LanguageDropdown.Hide();
            }
        }

        public Transform GetLeftMenuContent()
        {
            return _leftMenuContent.transform;
        }

        public Transform GetTopicsSubtopicsContent()
        {
            return _topicsSubtopicsContent;
        }

        public Transform GetAssetsContent()
        {
            return _assetsContent;
        }

        public void SetUserName(string firstLetter, string firstName, string lastName)
        {
            if (UserIconLabel != null)
            {
                UserIconLabel.text = firstLetter;
            }

            if (UserNameLabel != null)
            {
                UserNameLabel.text = firstName + " " + lastName;
            }
        }

        public void ShowCategoryWithHeaders(bool status)
        {
            ActivityHeaderText.gameObject.SetActive(status);
            LearnTechHeaderText.gameObject.SetActive(status);
        }

        # region Breadcrumbs
        
        public void SetBreadcrumbsVisibility(bool status)
        {
            if (_breadcrumbsPanel.gameObject.activeSelf != status)
            {
                _breadcrumbsPanel.gameObject.SetActive(status);
            }
        }

        public void ResetTopicBreadcrumb()
        {
            TopicText.text = string.Empty;
            TopicText.gameObject.SetActive(false);
            
            ShowTopicDivider(false);
        }

        public void ResetSubtopicBreadcrumb()
        {
            SubtopicText.text = string.Empty;
            SubtopicText.gameObject.SetActive(false);
            
            ShowSubtopicDivider(false);
        }
        
        public void ResetActivitiesBreadcrumb()
        {
            ActivitiesText.text = string.Empty;
            ActivitiesText.gameObject.SetActive(false);
            
            ShowActivitiesDivider(false);
        }
        
        public void ResetChosenActivityBreadcrumb()
        {
            ChosenActivityText.text = string.Empty;
            ChosenActivityText.gameObject.SetActive(false);
            
            ShowChosenActivityDivider(false);
        }

        public void ShowTopicDivider(bool status)
        {
            _topicDivider.SetActive(status);
        }

        public void ShowSubtopicDivider(bool status)
        {
            _subtopicDivider.SetActive(status);
        }
        
        public void ShowActivitiesDivider(bool status)
        {
            _activitiesDivider.SetActive(status);
        }
        
        public void ShowChosenActivityDivider(bool status)
        {
            _chosenActivityDivider.SetActive(status);
        }
        
        public void ResetAllBreadcrumbs()
        {
            Grade.text = string.Empty;
            Grade.gameObject.SetActive(false);

            SubjectText.text = string.Empty;
            SubjectText.gameObject.SetActive(false);
            
            ResetTopicBreadcrumb();
            ResetSubtopicBreadcrumb();
            ResetActivitiesBreadcrumb();
            ResetChosenActivityBreadcrumb();
        }
        
        #endregion

        #region Language

        protected void OnLanguageDropdownChanged(int itemIndex)
        {
            Signal.Fire(new OnChangeLanguageClickViewSignal(itemIndex));
        }

        public void UpdateLanguageDropdownManually(int itemIndex)
        {
            LanguageDropdown.value = itemIndex;
        }

        #endregion

        #region Home Tabs

        private void OnHomeTabFavouritesClick()
        {
            Signal.Fire<HomeTabFavouritesClickViewSignal>();
        }

        private void OnHomeTabRecentClick()
        {
            Signal.Fire<HomeTabRecentClickViewSignal>();
        }

        private void OnHomeTabMyContentClick()
        {
            Signal.Fire<HomeTabMyContentClickViewSignal>();
        }
        
        private void OnHomeTabMyTeacherClick()
        {
            Signal.Fire<HomeTabMyTeacherClickViewSignal>();
        }

        public void SetHomeTabsVisibility(bool status)
        {
            if (_tabsPanel.gameObject.activeSelf != status)
            {
                _tabsPanel.gameObject.SetActive(status);
            }
        }

        public void SetHomeTabFavouritesActive(bool status)
        {
            TabFavouritesText.color = status ? HomeTabActive : HomeTabInactive;
            TabFavouritesUnderline.gameObject.SetActive(status);
        }

        public void SetHomeTabRecentActive(bool status)
        {
            TabRecentText.color = status ? HomeTabActive : HomeTabInactive;
            TabRecentUnderline.gameObject.SetActive(status);
        }
        
        public void SetHomeTabMyContentActive(bool status)
        {
            TabMyContentText.color = status ? HomeTabActive : HomeTabInactive;
            TabMyContentUnderline.gameObject.SetActive(status);
        }
        
        public void SetHomeTabMyTeacherActive(bool status)
        {
            TabMyTeacherText.color = status ? HomeTabActive : HomeTabInactive;
            TabMyTeacherUnderline.gameObject.SetActive(status);
        }

        public void SetTabMyTeacherVisibility(bool status)
        {
            TabMyTeacherButton.gameObject.SetActive(status);
        }

        #endregion

        public class Factory : PlaceholderFactory<HomeViewBase>
        {
        }
    }
}