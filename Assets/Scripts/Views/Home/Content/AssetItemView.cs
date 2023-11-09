using DG.Tweening;
using TDL.Core;
using TDL.Models;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class AssetItemView : ViewBase, IThumbnailView, IFontSizeView
    {
        public int GradeId { get; set; }
        public int AssetId { get; set; }
        
        public float DefaultFontSize { get; set; }
        public TextMeshProUGUI Title
        {
            get => _title;
        }

        [Inject] private readonly UserLoginModel _userLoginModel;
        [SerializeField] private RawImage _thumbnail;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _clickableArea;

        [Header("Icons panel")] 
        [SerializeField] private Image _assetType;
        [SerializeField] private Image _puzzle;
        private Button _puzzleButton;
        [SerializeField] private Image _quiz;
        private Button _quizButton;
        [SerializeField] private Button _multipleQuizButton;
        [SerializeField] private Button _multiplePuzzleButton;
        
        [SerializeField] private Button _studentNotesButton;
        [SerializeField] private Button _generateInternalLinkButton;
        [SerializeField] private Button _descriptionButton;
        [SerializeField] private Toggle _favoriteToggle;
        [SerializeField] private Toggle _downloadToggle;
        [SerializeField] private Slider _progressSlider;

        [Header("More Dropdown")] 
        [SerializeField] private Toggle _moreDropdownToggle;

        [SerializeField] public TextMeshProUGUI MoreFavoriteAdd;
        [SerializeField] public TextMeshProUGUI MoreFavoriteRemove;
        [SerializeField] public TextMeshProUGUI MoreFavoriteTextToSpeech;
        [SerializeField] public TextMeshProUGUI MoreDownloadTextToSpeech;
        [SerializeField] public TextMeshProUGUI MoreDownload;
        [SerializeField] public TextMeshProUGUI MoreDownloadDelete;
        [SerializeField] public TextMeshProUGUI MoreDescription;
        [SerializeField] public TextMeshProUGUI StudentNotesText;
        [SerializeField] public TextMeshProUGUI MorePuzzle;
        [SerializeField] public TextMeshProUGUI MoreQuiz;
        
        public float DefaultDropdownMoreFontSize { get; set; }
        
        [Header("Multiple Activity Dropdowns")] 
        public Transform DropdownMultipleQuizContainer;
        public Transform DropdownMultiplePuzzleContainer;
        public bool HasDropdownMultipleQuiz { get; set; }
        public bool HasDropdownMultiplePuzzle { get; set; }

        private bool _isProgrammaticallyClickOnFavorites;
        private bool _isProgrammaticallyClickOnDownload;

        [Header("Feedback")] 
        [SerializeField] private Button _feedbackButton;
        public TextMeshProUGUI MoreFeedback;

        public override void InitUiComponents()
        {
            if (DeviceInfo.IsPCInterface())
            {
                _puzzleButton = _puzzle?.GetComponent<Button>();
                _quizButton = _quiz?.GetComponent<Button>();
            
                SaveDefaultFontSizes();
            }

        }

        private void SaveDefaultFontSizes()
        {
            DefaultFontSize = _title.fontSize;
            DefaultDropdownMoreFontSize = MoreFavoriteAdd.fontSize;            
        }

        public override void SubscribeOnListeners()
        {
            _clickableArea.onClick.AddListener(OnItemClick);
            _puzzleButton?.onClick.AddListener(OnPuzzleClick);
            _quizButton?.onClick.AddListener(OnQuizClick);
            _multipleQuizButton?.onClick.AddListener(OnMultipleQuizClick);
            _multiplePuzzleButton?.onClick.AddListener(OnMultiplePuzzleClick);
            _descriptionButton.onClick.AddListener(OnDescriptionButtonClick);
            _studentNotesButton.onClick.AddListener(ShowStudentNotes);
            _favoriteToggle.onValueChanged.AddListener(OnFavoritesToggleClick);
            _downloadToggle.onValueChanged.AddListener(OnDownloadToggleClick);
            _moreDropdownToggle.onValueChanged.AddListener(OnMoreDropdownClick);
            _generateInternalLinkButton?.onClick.AddListener(OnGenerateInternalLinkClick);
            if (DeviceInfo.IsPCInterface())
            {
                _feedbackButton.onClick.AddListener(OnFeedbackButtonClick);
            }
        }

        private void OnGenerateInternalLinkClick()
        {
            Debug.Log("Generate IL pressed");
            Signal.Fire(new GenerateInternalAssetLinkCommandSignal(AssetId.ToString()));
        }

        public override void UnsubscribeFromListeners()
        {
            _clickableArea.onClick.RemoveAllListeners();
            _puzzleButton?.onClick.RemoveAllListeners();
            _quizButton?.onClick.RemoveAllListeners();
            _multipleQuizButton?.onClick.RemoveAllListeners();
            _multiplePuzzleButton?.onClick.RemoveAllListeners();
            _descriptionButton.onClick.RemoveAllListeners();
            _studentNotesButton?.onClick.RemoveAllListeners();
            _favoriteToggle.onValueChanged.RemoveAllListeners();
            _downloadToggle.onValueChanged.RemoveAllListeners();
            _moreDropdownToggle.onValueChanged.RemoveAllListeners();
            _generateInternalLinkButton?.onClick.RemoveAllListeners();
            
            if (DeviceInfo.IsPCInterface())
            {
                _feedbackButton.onClick.RemoveAllListeners();
            }
        }
        
        private void ShowStudentNotes()
        {
            Signal.Fire(new ShowStudentNotesPanelViewSignal(AssetId));
        }

        public void SetThumbnail(Texture thumbnail)
        {
            _thumbnail.texture = thumbnail;
            _thumbnail.color = new Color(1, 1, 1, 0);
            _thumbnail.DOFade(1, 1);
        }

        public void SetAssetType(Sprite type)
        {
            _assetType.sprite = type;
        }

        public void SetPuzzleVisibility(int puzzleCount)
        {
            if (_puzzleButton)
            {
                if (puzzleCount == 0)
                {
                    _puzzleButton.gameObject.SetActive(false);
                    _multiplePuzzleButton.gameObject.SetActive(false);
                }
                else
                {
                    _puzzleButton.gameObject.SetActive(!HasDropdownMultiplePuzzle);
                    _multiplePuzzleButton.gameObject.SetActive(HasDropdownMultiplePuzzle);
                }   
            }
        }

        public void SetQuizVisibility(int quizCount)
        {
            if (_quizButton)
            {
                if (quizCount == 0)
                {
                    _quizButton.gameObject.SetActive(false);
                    _multipleQuizButton.gameObject.SetActive(false);   
                }
                else
                {
                    _quizButton.gameObject.SetActive(!HasDropdownMultipleQuiz);
                    _multipleQuizButton.gameObject.SetActive(HasDropdownMultipleQuiz);   
                }   
            }
        }

        public void SetGenerateInternalLinkVisibility(bool isActive)
        {
            _generateInternalLinkButton.gameObject.SetActive(isActive);
        }

        public void SetActiveDownload(bool isActive)
        {
            _downloadToggle.gameObject.SetActive(isActive);
        }

        public void SetDownloadStatus(bool status, bool isProgrammaticallyClick = false)
        {
            if (_downloadToggle.isOn != status)
            {
                _isProgrammaticallyClickOnDownload = isProgrammaticallyClick;
                _downloadToggle.isOn = status;
            }
        }

        public void SetMoreMenuStatus(bool status)
        {
            _moreDropdownToggle.gameObject.SetActive(status);
        }
        
        private void OnDownloadToggleClick(bool status)
        {
            if (_isProgrammaticallyClickOnDownload)
            {
                _isProgrammaticallyClickOnDownload = false;
                return;
            }
            
            Signal.Fire(new DownloadToggleClickViewSignal(AssetId, status));
        }

        private void OnMoreDropdownClick(bool status)
        {
            Signal.Fire(new MoreDropdownClickViewSignal(AssetId, status));
        }

        public void SetFavoriteStatus(bool isAdded)
        {
            if (_favoriteToggle.isOn != isAdded)
            {
                _isProgrammaticallyClickOnFavorites = true;
                _favoriteToggle.isOn = isAdded;
            }
        }

        public void ShowProgressSlider(bool status)
        {
            if (_progressSlider.gameObject.activeSelf != status)
            {
                _progressSlider.gameObject.SetActive(status);
            }
        }
        
        public void SetDescriptionVisibility(bool isVisible)
        {
            _descriptionButton.gameObject.SetActive(isVisible);
        }

        public void SetFeedbackAvailability(bool isAvailable)
        {
            _feedbackButton.gameObject.SetActive(isAvailable);
        }
        
        public void SetStudentNotesAvailability(bool isAvailable)
        {
            _studentNotesButton.gameObject.SetActive(isAvailable);
        }

        public void UpdateProgressSlider(float progress)
        {
            if (DeviceInfo.IsMobile())
            {
                progress /= 100f;
            }

            _progressSlider.value = progress;
        }

        public void ResetProgress()
        {
            _progressSlider.value = 0.0f;
        }

        protected virtual void OnItemClick()
        {
            Debug.Log("AssetItemView => OnItemClick = AssetId = " + AssetId + " | GradeId = " + GradeId);
            Signal.Fire(new AssetItemClickViewSignal(AssetId, GradeId));
            var a = new Animator();
        }

        private void OnPuzzleClick()
        {
            Signal.Fire(new PuzzleClickViewSignal(AssetId, GradeId));
        }

        private void OnQuizClick()
        {
            Signal.Fire(new QuizClickViewSignal(AssetId, GradeId));
        }

        #region Dropdown Activity

        private void OnMultipleQuizClick()
        {
            if (ShouldCreateActivityDropdown(DropdownMultipleQuizContainer))
            {
                Signal.Fire(new OnDropdownMultipleQuizClickViewSignal(this));
            }
        }
        
        private void OnMultiplePuzzleClick()
        {
            if (ShouldCreateActivityDropdown(DropdownMultiplePuzzleContainer))
            {
                Signal.Fire(new OnDropdownMultiplePuzzleClickViewSignal(this));
            }
        }

        private bool ShouldCreateActivityDropdown(Transform activityDropdown)
        {
            return activityDropdown.childCount == 0;
        }

        #endregion

        private void OnDescriptionButtonClick()
        {
            Signal.Fire(new DescriptionClickViewSignal(AssetId));
        }

        private void OnFavoritesToggleClick(bool isChanged)
        {
            if (_isProgrammaticallyClickOnFavorites)
            {
                _isProgrammaticallyClickOnFavorites = false;
                return;
            }

            Signal.Fire(new FavouriteToggleClickViewSignal(GradeId, AssetId, isChanged));
        }

        private void OnFeedbackButtonClick()
        {
            Signal.Fire(new ShowFeedbackPopupFromAssetViewSignal(AssetId));
        }

        protected void ResetView()
        {
            _title.text = string.Empty;
            _thumbnail.texture = null;

            HasDropdownMultipleQuiz = false;

            ResetProgress();
            ShowProgressSlider(false);
        }

        #region More dropdown

        public void OnMouseClickToggle()
        {
            SetMoreFavoriteForTextToSpeech(MoreFavoriteTextToSpeech.text.Equals(MoreFavoriteAdd.text)
                ? MoreFavoriteAdd.text
                : MoreFavoriteRemove.text);
            
            SetMoreDownloadForTextToSpeech(MoreDownloadTextToSpeech.text.Equals(MoreDownload.text)
                ? MoreDownload.text
                : MoreDownloadDelete.text);
        }

        public void OnKeyboardClickToggle()
        {
            SetMoreFavoriteForTextToSpeech(MoreFavoriteTextToSpeech.text.Equals(MoreFavoriteAdd.text)
                ? MoreFavoriteRemove.text
                : MoreFavoriteAdd.text);
            
            SetMoreDownloadForTextToSpeech(MoreDownloadTextToSpeech.text.Equals(MoreDownload.text)
                ? MoreDownloadDelete.text
                : MoreDownload.text);
        }

        public void SetMoreFavoriteAddText(string text)
        {
            MoreFavoriteAdd.text = text;
        }
        
        public void SetMoreFavoriteRemoveText(string text)
        {
            MoreFavoriteRemove.text = text;
        }
        
        private void SetMoreFavoriteForTextToSpeech(string text)
        {
            MoreFavoriteTextToSpeech.text = text;
        }
        
        private void SetMoreDownloadForTextToSpeech(string text)
        {
            MoreDownloadTextToSpeech.text = text;
        }
        
        public void UpdateTextToSpeechToggles()
        {
            UpdateMoreFavoriteTextToSpeech();
            UpdateMoreDownloadTextToSpeech();
        }

        private void UpdateMoreFavoriteTextToSpeech()
        {
            MoreFavoriteTextToSpeech.text = _favoriteToggle.isOn 
                ? MoreFavoriteRemove.text 
                : MoreFavoriteAdd.text;
        }
        
        private void UpdateMoreDownloadTextToSpeech()
        {
            MoreDownloadTextToSpeech.text = _downloadToggle.isOn 
                ? MoreDownloadDelete.text 
                : MoreDownload.text;
        }

        public void SetMoreDownloadText(string text)
        {
            MoreDownload.text = text;
        }
        
        public void SetMoreDownloadDeleteText(string text)
        {
            MoreDownloadDelete.text = text;
        }
        
        public void SetMoreDescriptionText(string text)
        {
            MoreDescription.text = text;
        }
        
        public void SetStudentNotesText(string text)
        {
            StudentNotesText.text = text;
        }
        
        public void SetMorePuzzleText(string text)
        {
            MorePuzzle.text = text;
        }
        
        public void SetMoreQuizText(string text)
        {
            MoreQuiz.text = text;
        }

        public void SetMoreFeedbackText(string text)
        {
            MoreFeedback.text = text;
        }

        #endregion
        
        public void OnKeyboardNavigationOut()
        {
            if (_moreDropdownToggle.isOn)
            {
                _moreDropdownToggle.isOn = false;
            }
        }

        public class Pool : MonoMemoryPool<Transform, AssetItemView>
        {
            protected override void Reinitialize(Transform viewParent, AssetItemView view)
            {
                if (view.transform.parent == null)
                {
                    view.transform.SetParent(viewParent, false);
                }

                view.ResetView();
            }
        }
    }   
}