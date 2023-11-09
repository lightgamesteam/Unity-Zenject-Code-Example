using System.Collections;
using TDL.Commands;
using TDL.Constants;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class DescriptionView : ViewBase, IDragHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string AssetId { get; set; }
        public string LabelId { get; set; }
        public string GradeId { get; set; }
        public bool IsMultiViewSecond { get; set; }
        public string CultureCode { get; set; }
        public string AudioFileURL { get; set; }

        private AudioSource _audioSource;
        private RectTransform _viewTransform;
        private ContentSizeFitter _contentSizeFitter;
        private CanvasGroup _canvas;
        private RectTransform _contentPanel;
        private TMP_InputField _description;
        private RectTransform _scrollbarRect;
        private Scrollbar _scrollbar;
        private CanvasGroup _scrollbarCanvas;
        private TextMeshProUGUI _noTranslationText;
        
        // Top panel
        private TextMeshProUGUI _title;
        private Toggle _playButton;
        private Button _moveButton;
        private Button _closeButton;
        
        // Drag feature
        private bool _isViewBeingDrag;
        private Vector2 _originalLocalPointerPos;
        private Vector3 _originalPanelLocalPos;
        private RectTransform _parentRectTransform;

        // Block model movements
        private bool _isViewClicked;
        private bool _isMouseOverView;

        private const float MaxHeight = 800.0f;
        private const float AlphaVisible = 1.0f;
        private const float AlphaInvisible = 0.0f;
        private const float MultimodelViewPosition = 1040.0f;
        private const float DelayBeforeHideLoading = 0.3f;
        private const float AudioLengthThreshold = 0.1f;
        
        public override void InitUiComponents()
        {
            _audioSource = GetComponent<AudioSource>();
            _viewTransform = GetComponent<RectTransform>();
            _contentSizeFitter = GetComponent<ContentSizeFitter>();
            _canvas = GetComponent<CanvasGroup>();
            _contentPanel = transform.Get<RectTransform>("ContentPanel");
            _description = _contentPanel.Get<TMP_InputField>("InputField");
            _scrollbarRect = _contentPanel.Get<RectTransform>("PCScrollbar");
            _scrollbarCanvas = _scrollbarRect.GetComponent<CanvasGroup>();
            _scrollbar = _scrollbarRect.GetComponent<Scrollbar>();
            _noTranslationText = _description.transform.Get<TextMeshProUGUI>("NoTranslationText");

            // Top panel
            var topPanelButtons = transform.Get<Transform>("TopPanel/Buttons");
            _title = transform.Get<TextMeshProUGUI>("TopPanel/Title");
            _playButton = topPanelButtons.Get<Toggle>("PlayToggle");
            _moveButton = topPanelButtons.Get<Button>("Move");
            _closeButton = topPanelButtons.Get<Button>("Close");
            
//#if UNITY_WEBGL
            _playButton.gameObject.SetActive(false);
            //#endif
        }

        private void Start()
        {
            StartCoroutine(SetScrollToTop());
        }

        private IEnumerator SetScrollToTop()
        {
            if (_scrollbar != null)
            {
                //yield return new WaitForSeconds(0.3f);
                yield return null;
                yield return null;
                yield return null;
                yield return null;
                _scrollbar.value = 0f;
            }
        }

        public override void SubscribeOnListeners()
        {
            Signal.Subscribe<CloseDescriptionViewSignal>(OnCloseClick);
            _closeButton.onClick.AddListener(OnCloseClick);
            
//#if !UNITY_WEBGL
            _playButton.onValueChanged.AddListener(OnPlayClick);
//#endif
        }

        public override void UnsubscribeFromListeners()
        {
            _closeButton.onClick.RemoveAllListeners();
            _playButton.onValueChanged.RemoveAllListeners();
        }
  
//#if !UNITY_WEBGL
        private void OnPlayClick(bool isOn)
        {
            if (_audioSource.clip == null)
            {
                LoadAndPlaySound();
                StartCoroutine(HideLoadingOverlayOnPlay());
                StartCoroutine(TrackIfFAudioFinished());
            }
            else
            {
                if (isOn)
                {
                    PauseAllExceptActiveDescription();
                    StartCoroutine(TrackIfFAudioFinished());
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.Pause();
                }
            }
        }
//#endif

        private IEnumerator HideLoadingOverlayOnPlay()
        {
            while (!_audioSource.isPlaying)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(DelayBeforeHideLoading);
            
            Signal.Fire(new PopupOverlaySignal(false));
        }

        private IEnumerator TrackIfFAudioFinished()
        {
            while (_audioSource.clip == null)
            {
                yield return null;
            }

            while (_audioSource.isPlaying)
            {
                yield return null;
            }

            while (_audioSource.clip != null && _audioSource.clip.length - _audioSource.time > AudioLengthThreshold)
            {
                yield return null;
            }

            _audioSource.Stop();
            PlaySound(false);
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void SetParentContainer(Transform parentView)
        {
            _parentRectTransform = parentView as RectTransform;
        }
        
        public void UpdatePlayButtonVisibility()
        {
            _playButton.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(AudioFileURL))
            {
                if (LocalizationConstants.EnUSLanguage.Equals(CultureCode))
                {
//#if !UNITY_WEBGL
                    _playButton.gameObject.SetActive(true);
                    //#endif
                }
            }
            else
            {
//#if !UNITY_WEBGL
                    _playButton.gameObject.SetActive(true);
//#endif
            }
        }
        
        public void SetFontSize(float scaleFactor)
        {
            _title.fontSize = _title.fontSize / scaleFactor;
            _description.textComponent.fontSize = _description.textComponent.fontSize / scaleFactor;
        }

        public void SetDescription(string description)
        {
            _description.text = description;
            
            HideNoTranslationText();
            ResizeView();
        }

        public void SetNoTranslationText(string text)
        {
            _noTranslationText.text = text;
            _description.text = string.Empty;
            
            SetScrollbarVisibility(AlphaInvisible);
        }

        private void HideNoTranslationText()
        {
            if (!string.IsNullOrEmpty(_noTranslationText.text))
            {
                _noTranslationText.text = string.Empty;
            }
        }
        
        private void SetScrollbarVisibility(float status)
        {
            if (_scrollbarCanvas.alpha != status)
            {
                _scrollbarCanvas.alpha = status;
            }
        }

        public void RepositionView()
        {
            _viewTransform.anchoredPosition = new Vector2(MultimodelViewPosition, _viewTransform.anchoredPosition.y);
        }

        public void ResizeView()
        {
            SetScrollbarVisibility(AlphaVisible);
            StartCoroutine(ResizeViewCoroutine());
        }

        private IEnumerator ResizeViewCoroutine()
        {
            _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _contentPanel.gameObject.SetActive(false);

            yield return null;
            
            _contentPanel.gameObject.SetActive(true);
            
            yield return null;

            if (_viewTransform.sizeDelta.y > MaxHeight)
            {
                _contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                _viewTransform.sizeDelta = new Vector2(_viewTransform.sizeDelta.x, MaxHeight);
                
                _scrollbarRect.gameObject.SetActive(true);
            }
            else
            {
                SetScrollbarVisibility(AlphaInvisible);
            }
            
            yield return null;
            
            _canvas.alpha = AlphaVisible;
        }

        public void OnCloseClick()
        {
            Signal.Unsubscribe<CloseDescriptionViewSignal>(OnCloseClick);
            Signal.Fire(new OnDescriptionCloseClickViewSignal(AssetId, LabelId));
            Destroy(gameObject);
        }

        #region Play feature

        private void PauseAllExceptActiveDescription()
        {
            Signal.Fire(new PauseAllExceptActiveDescriptionViewSignal(string.IsNullOrEmpty(LabelId) ? AssetId : LabelId));
        }

//#if !UNITY_WEBGL
        private void LoadAndPlaySound()
        {
            if (string.IsNullOrEmpty(AudioFileURL))
            {
                if (LocalizationConstants.EnUSLanguage.Equals(CultureCode))
                {
                    Signal.Fire(new LoadAndPlayDescriptionViewSignal(
                        string.IsNullOrEmpty(LabelId) ? AssetId : LabelId,
                        _audioSource,
                        CultureCode, 
                        string.IsNullOrEmpty(_description.text)
                            ? _noTranslationText.text
                            : _description.text));
                }
            }
            else
            {
                Signal.Fire(new PopupOverlaySignal(true, cultureCode: CultureCode, localizationKey: LocalizationConstants.LoadingKey));
                Signal.Fire(new GetAudioFileCommandSignal(AudioFileURL, (clip) =>
                {
                    Signal.Fire(new PopupOverlaySignal(false));

                    if (clip != null)
                    {
                        _audioSource.clip = clip;
                        _audioSource.Play();
                    }
                }));
            }
        }
        //#endif

        public void PlaySound(bool status)
        {
            _playButton.isOn = status;
        }

        public void ClearAudioSource()
        {
            if (_audioSource.clip != null)
            {
                _audioSource.clip.UnloadAudioData();
                _audioSource.clip = null;
            }
        }

        #endregion

        #region Drag feature

        public void OnPointerDown(PointerEventData eventData)
        {
            _isViewClicked = true;

            if (eventData.pointerEnter == null || !eventData.pointerEnter.name.Equals(_moveButton.name))
                return;

            _originalPanelLocalPos = _viewTransform.localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, eventData.pressEventCamera, out _originalLocalPointerPos);

            _isViewBeingDrag = true;

            BlockModelMovements(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isViewClicked = false;
            _isViewBeingDrag = false;

            BlockModelMovements(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isViewBeingDrag == false || _viewTransform == null || _parentRectTransform == null)
                return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, eventData.position, eventData.pressEventCamera, out var localPointerPos))
            {
                Vector3 offsetToOriginal = localPointerPos - _originalLocalPointerPos;
                _viewTransform.localPosition = _originalPanelLocalPos + offsetToOriginal;
            }

            ClampToWindow();
        }

        private void ClampToWindow()
        {
            var pos = _viewTransform.localPosition;

            var minPos = _parentRectTransform.rect.min - _viewTransform.rect.min;
            var maxPos = _parentRectTransform.rect.max - _viewTransform.rect.max;

            pos.x = Mathf.Clamp(_viewTransform.localPosition.x, minPos.x, maxPos.x);
            pos.y = Mathf.Clamp(_viewTransform.localPosition.y, minPos.y, maxPos.y);

            _viewTransform.localPosition = pos;
        }

        public void SetViewOnTop()
        {
            _isViewClicked = true;
            transform.SetAsLastSibling();
        }

        #endregion

        #region Block model movements

        private void Update()
        {
            if (IsMouseWheelDetected())
            {
                BlockModelMovements(true);
            }
        }

        private bool IsMouseWheelDetected()
        {
            return _isMouseOverView && Input.mouseScrollDelta.y != 0;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null)
            {
                _isMouseOverView = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null)
            {
                _isMouseOverView = false;

                if (_isViewClicked)
                {
                    BlockModelMovements(true);
                }
            }
        }

        private void BlockModelMovements(bool status)
        {
            Signal.Fire(new OnDescriptionBlockModelMovementsViewSignal(!status));
        }

        #endregion

        public class Factory : PlaceholderFactory<DescriptionView>
        {
        }
    }
}