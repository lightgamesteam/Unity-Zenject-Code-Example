using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Views
{
    public class HomeViewMobile : HomeViewBase
    {
        [Header("Mobile Tab menu")] 
        public Toggle homeTabToggle;
        public Toggle favoriteTabToggle;
        public Toggle recentlyTabToggle;
        public Toggle myContentToggle;
        public Toggle rightTabToggle;

        [Header("Breadcrumbs back button")]
        public Button backButton;
        
        [Header("Home Screen Tab Title")]
        public TextMeshProUGUI tabTitleText;

        [Header("Right Menu Buttons")]     
        public GameObject MetaDataEmptySeparator;
        public Button MetaDataButton;
        public GameObject MyTeacherEmptySeparator;
        public Button MyTeacherButton;
        public Button changePasswordButton;
        
        public Button logoutButton;
        
        public Button languageButton;
        public TextMeshProUGUI languageDropdownText;

        public Toggle accessibilityLabelLines;
        
        public override void InitUiComponents()
        {
            // top menu
            SearchPanel = transform.Get<Transform>("TopMenu/SearchPanel");
            SearchInput = SearchPanel.Get<InputField>("SearchInputField");
            _imageRecognitionButton = SearchPanel.Get<Button>("SearchInputField/Button_ImageRecognition");
            SearchPlaceholder = SearchInput.transform.Get<Text>("TextViewPort/PanelText/Placeholder");
            tabTitleText = transform.Get<TextMeshProUGUI>("Content/TabTitle");
            SearchGoButton = SearchPanel.Get<Button>("GoButton");
            NoSearchResultsFound = transform.Get<TextMeshProUGUI>("Content/NoSearchResultsPanel/NoSearchResultsText");

            // breadcrumbs
            backButton = transform.Get<Button>("Content/BreadcrumbsPanel/SubjectTopicSubtopicPanel/Back");
            _breadcrumbsPanel = transform.Get<Transform>("Content/BreadcrumbsPanel");
            Grade = _breadcrumbsPanel.Get<TextMeshProUGUI>("Grade");
            var subjectTopicSubtopicPanel = _breadcrumbsPanel.Get<Transform>("SubjectTopicSubtopicPanel");
            _subjectHyperlink = subjectTopicSubtopicPanel.Get<Button>("Subject");
            SubjectText = _subjectHyperlink.GetComponent<TextMeshProUGUI>();
            _topicDivider = subjectTopicSubtopicPanel.Get<Transform>("TopicDivider").gameObject;
            _topicHyperlink = subjectTopicSubtopicPanel.Get<Button>("Topic");
            TopicText = _topicHyperlink.GetComponent<TextMeshProUGUI>();
            _subtopicDivider = subjectTopicSubtopicPanel.Get<Transform>("SubtopicDivider").gameObject;
            _subtopicHyperlink = subjectTopicSubtopicPanel.Get<Button>("Subtopic");
            SubtopicText = _subtopicHyperlink.GetComponent<TextMeshProUGUI>();
            
            //right menu
            var optionsPanel = transform.Get<Transform>("Content/X_RightMenu/OptionsPanel");
            UserNameLabel = optionsPanel.transform.parent.Get<TextMeshProUGUI>("UserName");

            MetaData = optionsPanel.Get<TextMeshProUGUI>("MetaData/Text");
            MyTeacher = optionsPanel.Get<TextMeshProUGUI>("MyTeacher/Text");
            ChangePassword = optionsPanel.Get<TextMeshProUGUI>("ChangePassword/Text");
            Logout = optionsPanel.Get<TextMeshProUGUI>("SignOut/Text");
            
            ArModeToggle = transform.Get<Toggle>("Content/X_RightMenu/OptionsPanel/AR_Mode");
            ArModeToggle.isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.ARmodeSettings);
            
            accessibilityLabelLines = transform.Get<Toggle>("Content/X_RightMenu/OptionsPanel/LabelLines");
            accessibilityLabelLines.isOn = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines);
            
            languageDropdownText = transform.Get<TextMeshProUGUI>("Content/X_RightMenu/LanguagePanel/LanguageDropdown/Label");
            LanguageTitle = transform.Get<TextMeshProUGUI>("Content/X_RightMenu/LanguagePanel/LanguageTitle");
            LanguageDropdown = transform.Get<TMP_Dropdown>("Content/X_RightMenu/LanguagePanel/LanguageDropdown");
            languageButton = transform.Get<Button>("Content/X_RightMenu/LanguagePanel/LanguageDropdown/LanguageButton");
            
            // category with headers
            var bodyContent = transform.Get<Transform>("Content/AssetsPanel/Viewport/Content"); 
            ActivityHeaderText = bodyContent.Get<TextMeshProUGUI>("<--ActivityHeader-->");
            LearnTechHeaderText = bodyContent.Get<TextMeshProUGUI>("<--LearnTechHeader-->");
        }

        public override void SubscribeOnListeners()
        {
            SearchGoButton.onClick.AddListener(OnSearchGoClick);
            SearchInput.onValueChanged.AddListener(OnSearchInputChanged);
            
            //right menu
            _closeAppButton.onClick.AddListener(OnCloseAppClick);
            MetaDataButton.onClick.AddListener(OnMetaDataClick);
            MyTeacherButton.onClick.AddListener(OnMyTeacherClick);
            changePasswordButton.onClick.AddListener(OnChangePasswordViewClick);
            logoutButton.onClick.AddListener(OnSignOutViewClick);
            ArModeToggle.onValueChanged.AddListener(ChangeARmodeSettings);
            accessibilityLabelLines.onValueChanged.AddListener(ChangeLabelLinesSettings);
            
            Signal.Fire(new OnHomeActivatedViewSignal());
        }

        public override void UnsubscribeFromListeners()
        {
            SearchGoButton.onClick.RemoveAllListeners();
            SearchInput.onValueChanged.RemoveAllListeners();
            
            //hyperlink
            _subjectHyperlink.onClick.RemoveAllListeners();
            _topicHyperlink.onClick.RemoveAllListeners();
            _subtopicHyperlink.onClick.RemoveAllListeners();
            
            //right menu
            MetaDataButton.onClick.RemoveAllListeners();
            MyTeacherButton.onClick.RemoveAllListeners();
            
            Signal.Fire(new OnHomeDeactivatedViewSignal());
        }

        private void OnMetaDataClick()
        {
            Signal.Fire<MetaDataClickViewSignal>();
        }
        
        private void OnMyTeacherClick()
        {
            Signal.Fire<RightMenuMyTeacherClickViewSignal>();
        }
        
        private void OnChangePasswordViewClick()
        {
            Signal.Fire(new ChangePasswordClickViewSignal());
        }
        
        private void OnSignOutViewClick()
        {
            Signal.Fire<SignOutClickCommandSignal>();
        }

        public void ChangeLabelLinesSettings(bool value)
        {
            PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines, value, true);
        }

        public void ClearSearchInput()
        {
            if (!string.IsNullOrEmpty(SearchInput.text))
            {
                SearchInput.text = string.Empty;
            }
        }
        
        public void ResetAllBreadcrumbs()
        {
            Grade.text = string.Empty;
            SubjectText.text = string.Empty;
            ResetTopicBreadcrumb();
            ResetSubtopicBreadcrumb();
        }
    }
}