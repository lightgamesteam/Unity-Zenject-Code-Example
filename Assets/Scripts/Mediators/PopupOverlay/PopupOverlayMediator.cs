using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class PopupOverlayMediator : IInitializable, IDisposable
    {
        [Inject] private PopupOverlayBase.Factory _popupOverlayPCFactory;
        [Inject] private SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private readonly PopupModel _popupModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;
        [Inject] private ServerService _serverService;
    
        private PopupOverlayBase _popupOverlayView;
    
        [InjectOptional] private CanvasPanel.Factory _popupLanguageFactory;
        private CanvasPanel _popupLanguageView;
        private PopupOverlayBase _recordingFramePopup;
        
        [InjectOptional] private PopupInputView.Factory _popupInputViewFactory;
        [InjectOptional] private PopupInfoView.Factory _popupInfoViewFactory;
        [InjectOptional] private PopupWarningARView.Factory _popupWarningARViewFactory;
        [InjectOptional] private PopupHotSpotListView.Factory _popupHotSpotListViewFactory;

        //private PopupInputView _popupInputView;
    
        public void Initialize()
        {
            CreateView();
            SubscribeOnListeners();
        }

        private void CreateView()
        {
            _popupOverlayView = _popupOverlayPCFactory.Create();
            _popupOverlayView.InitUiComponents();

            if (DeviceInfo.IsMobile())
            {
                _popupLanguageView = _popupLanguageFactory.Create();
            }
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<PopupOverlaySignal>(Show);
            _signal.Subscribe<OnCancelProgressClickViewSignal>(OnCancelProgressClick);
            _signal.Subscribe<OnCloseProgressClickViewSignal>(OnCloseProgressClick);
            _signal.Subscribe<PopupInputViewSignal>(ShowPopupInput);
            _signal.Subscribe<PopupInfoViewSignal>(ShowPopupInfo);
            _signal.Subscribe<PopupWarningARViewSignal>(ShowPopupWarningAR);
            _signal.Subscribe<PopupHotSpotListViewSignal>(ShowPopupHotSpotList);

            _homeModel.AssetsToDownload.CollectionChanged += OnAssetsToDownloadChanged;
            _popupModel.OnResetChanged += OnResetPopup;

            if (DeviceInfo.IsMobile())
            {
                _signal.Subscribe<MoreDropdownClickViewSignal>(CreateMoreDropdownMobile);
                _localizationModel.OnLanguageChanged += CreateLanguagesPopup;
            }
        }

        private void ShowPopupHotSpotList(PopupHotSpotListViewSignal signal)
        {
            PopupHotSpotListView popup = _popupHotSpotListViewFactory.Create();

            popup.SetPanelName(signal.PanelName);
            popup.InitHotSpotList(signal.HotSpotList, signal.Callback);
            popup.SetCloseButtonTooltip(_localizationModel.GetSystemTranslations(signal.CultureCode, LocalizationConstants.CloseKey));
            
            popup.ScaleText(_accessibilityModel.ModulesFontSizeScaler);
            popup.ShowView();
        }

        private void ShowPopupWarningAR(PopupWarningARViewSignal signal)
        {
          
            
            PopupWarningARView popup = _popupWarningARViewFactory.Create();
            
            popup.SetOkButtonName(_localizationModel.GetSystemTranslations(_localizationModel.CurrentLanguageCultureCode, LocalizationConstants.OkKey));
            
            popup.OkCallback += OnOkCallback;
            void OnOkCallback()
            {
                signal.OkCallback?.Invoke();
                popup.OkCallback -= OnOkCallback;
            }
            
            _serverService.DownloadWarningARImage(_localizationModel.CurrentLanguageCultureCode, texture2D =>
            {
                Sprite s = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                popup.SetWarningSprite(s);
            });
            
            popup.ScaleText(_accessibilityModel.ModulesFontSizeScaler);
            popup.ShowView();

            
        }

        private void ShowPopupInfo(PopupInfoViewSignal signal)
        {
            PopupInfoView popup = _popupInfoViewFactory.Create();
            
            popup.SetInfoText(signal.InfoText);
            popup.SetToggleName(signal.ToggleText);
            popup.SetOkButtonName(signal.OkButtonText);
            popup.SetHelpButtonName(signal.HelpButtonText);
            
            popup.OkCallback += OnOkCallback;
            void OnOkCallback(bool value)
            {
                signal.OkCallback?.Invoke(value);
                popup.OkCallback -= OnOkCallback;
            }
            
            popup.HelpCallback += OnHelpCallback;
            void OnHelpCallback()
            {
                signal.HelpCallback?.Invoke();
                popup.HelpCallback -= OnHelpCallback;
            }
            
            popup.ScaleText(_accessibilityModel.ModulesFontSizeScaler);
            popup.ShowView();
            
            signal.PopupInfoCallback?.Invoke(popup);
        }

        private void ShowPopupInput(PopupInputViewSignal signal)
        {
            PopupInputView popup = _popupInputViewFactory.Create();

            popup.SetPanelName(signal.PanelName);
            popup.SetInputValue(signal.DefaultInputValue);
            popup.SetInputReadonly(signal.IsReadonly);
            popup.SetInputFieldPlaceholderTitle(_localizationModel.GetSystemTranslations(signal.CultureCode, LocalizationConstants.EnterFileNameKey));
            popup.SetCancelButtonName(_localizationModel.GetSystemTranslations(signal.CultureCode, LocalizationConstants.CancelKey));
            popup.SetOkButtonName(_localizationModel.GetSystemTranslations(signal.CultureCode, LocalizationConstants.OkKey));
            
            popup.OnCancel += OnCancel;
            void OnCancel()
            {
                signal.Callback?.Invoke(false, string.Empty);
                popup.OnCancel -= OnCancel;
            }
            
            popup.OnSubmit += OnSubmit;
            void OnSubmit(string value)
            {
                signal.Callback?.Invoke(true, value);
                popup.OnSubmit -= OnSubmit;
            }

            popup.ScaleText(_accessibilityModel.ModulesFontSizeScaler);
            popup.ShowView();
        }

        public void Dispose()
        {
            _signal.Unsubscribe<PopupOverlaySignal>(Show);
            _signal.Unsubscribe<OnCancelProgressClickViewSignal>(OnCancelProgressClick);
            _signal.Unsubscribe<OnCloseProgressClickViewSignal>(OnCloseProgressClick);
            _signal.Unsubscribe<PopupInputViewSignal>(ShowPopupInput);

            if (_homeModel != null)
            {
                _homeModel.AssetsToDownload.CollectionChanged -= OnAssetsToDownloadChanged;
            }

            if (_popupModel != null)
            {
                _popupModel.OnResetChanged -= OnResetPopup;
            }

            if (DeviceInfo.IsMobile())
            {
                _signal.Unsubscribe<MoreDropdownClickViewSignal>(CreateMoreDropdownMobile);
                _localizationModel.OnLanguageChanged -= CreateLanguagesPopup;
            }
        }

        private bool isCreateLanguagesPopup;

        private void CreateLanguagesPopup()
        {
            var currentSystemTranslations =
                _localizationModel.AllSystemTranslations[_localizationModel.CurrentLanguageCultureCode];
            _popupLanguageView.SetPanelName(currentSystemTranslations[LocalizationConstants.LanguageTitleKey]);

            if (isCreateLanguagesPopup) return;

            _popupLanguageView.EnableLeftButton(false);
            _popupLanguageView.SetRightButtonName("OK");
            _popupLanguageView.rightButton.onClick.AddListener(_popupLanguageView.CloseCanvasPanel);

            foreach (LanguageResource l in _localizationModel.AvailableLanguages)
            {
                var tgl = _popupLanguageView.contentPanel.CreateButton(l.Name) as Toggle;
                var index = _localizationModel.AvailableLanguages.IndexOf(l);

                tgl.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        ChangeLanguage(index);
                    }
                });

                if (l.Culture.Equals(_localizationModel.CurrentLanguageCultureCode))
                    tgl.isOn = true;
            }

            isCreateLanguagesPopup = true;
        }

        private void ChangeLanguage(int index)
        {
            _signal.Fire(new ChangeLanguageCommandSignal(index));
        }

        private void OnAssetsToDownloadChanged(object sender, NotifyCollectionChangedEventArgs changeEvent)
        {
            if (_popupOverlayView.IsVisible())
            {
                if (changeEvent.Action == NotifyCollectionChangedAction.Replace)
                {
                    UpdateProgressSlider(changeEvent.NewStartingIndex);
                }
            }
        }

        private void UpdateProgressSlider(int indexOfChangedAsset)
        {
            if (IsMultipleActivitySelected())
            {
                UpdateMultipleAssetProgressSlider();
            }
            else
            {
                UpdateAssetProgressSlider(indexOfChangedAsset);
            }
        }

        private void UpdateMultipleAssetProgressSlider()
        {
            var foundMultipleAssetIds = GetMultipleActivityAssetIds();

            if (foundMultipleAssetIds == null)
                return;
            
            var totalDownloadSize = 0.0f;
            var totalDownloadedSize = 0.0f;

            foreach (var asset in foundMultipleAssetIds)
            {
                var downloadAsset = _homeModel.AssetsToDownload.FirstOrDefault(item => item.Id == asset);

                if (downloadAsset == null)
                    continue;
                
                totalDownloadSize += downloadAsset.ItemRequest.DownloadLength;
                totalDownloadedSize += downloadAsset.ItemRequest.Downloaded;
            }

            var progress = totalDownloadedSize / totalDownloadSize * 100.0f;

            if (progress > _popupOverlayView.GetProgressValue() && progress <= 100f)
                _popupOverlayView.UpdateProgress(progress);
        }

        private void UpdateAssetProgressSlider(int indexOfChangedAsset)
        {
            var downloadItem = _homeModel.AssetsToDownload[indexOfChangedAsset];
            if (downloadItem.Id == _contentModel?.SelectedAsset?.Asset?.Id)
            {
                _popupOverlayView.UpdateProgress(downloadItem.Progress);
            }
        }

        private List<int> GetMultipleActivityAssetIds()
        {
            var activeId = 0;

            if (_contentModel.SelectedAsset.IsMultipleQuizSelected)
            {
                activeId = _contentModel.SelectedAsset.Quiz[0].itemId;
            }
            else if (_contentModel.SelectedAsset.IsMultiplePuzzleSelected)
            {
                activeId = _contentModel.SelectedAsset.Puzzle[0].itemId;
            }
            else if (_contentModel.SelectedAsset.IsClassificationSelected)
            {
                activeId = _contentModel.SelectedAsset.Classification.itemId;
            }

            return _homeModel.MultipleAssetIdQueueToDownload.ContainsKey(activeId)
                ? _homeModel.MultipleAssetIdQueueToDownload[activeId]
                : null;
        }

        private void Show(PopupOverlaySignal signal)
        {
            _popupOverlayView.ShowOkButton(false);
            _popupOverlayView.ShowLoadingBar(false);

            switch (signal.Type)
            {
                case PopupOverlayType.Overlay:
                    ShowPopup(signal.Active);
                    _popupOverlayView.ShowProgressPanel(signal.ShowProgress);
                    SetPopupData(signal.Message, signal.CultureCode, signal.LocalizationKey);
                    
                    if(signal.Active)
                        _popupOverlayView.transform.SetAsLastSibling();
                    break;

                case PopupOverlayType.MessageBox:
                    var msgBox = _popupOverlayPCFactory.Create();
                    msgBox.InitUiComponents();
                    msgBox.SetText(signal.Message, TextAlignmentOptions.Top, _accessibilityModel.ModulesFontSizeScaler, 50.0f);
                    msgBox.ShowOkButton(true);
                    msgBox.ShowPopup(true);
                    msgBox.transform.SetAsLastSibling();
                    break;
            
                case PopupOverlayType.LoadingBar:
                    ShowPopup(signal.Active);
                    _popupOverlayView.ShowLoadingBar(true);
                    SetPopupData(signal.Message, signal.CultureCode, signal.LocalizationKey);
                    
                    if(signal.Active)
                        _popupOverlayView.transform.SetAsLastSibling();
                    break;
            
                case PopupOverlayType.TextBox:
                    if (DeviceInfo.IsPCInterface())
                    {
                        var txtBox = _popupOverlayPCFactory.Create() as PopupOverlayPC;
                        var canShowCancel = signal.CloseCallback != null;
                        
                        txtBox.ShowTextBox(signal.Message, canShowCancel);
                        txtBox.transform.SetAsLastSibling();
                        
                        if (signal.OkCallback != null)
                            txtBox.AddOkOnClickCallback(signal.OkCallback);

                        if (canShowCancel)
                        {
                            txtBox.AddCloseOnClickCallback(signal.CloseCallback);
                        }
                    }
                    else
                    {
                        var txtBoxMobile = _popupOverlayPCFactory.Create() as PopupOverlayMobile;
                        txtBoxMobile.InitUiComponents();
                        txtBoxMobile._canvasPanel.SetPanelName(signal.PanelName);
                        txtBoxMobile._canvasPanel.contentPanel.SetTextBox(signal.Message);
                        txtBoxMobile.ShowContentPanel();
                        txtBoxMobile.transform.SetAsLastSibling();
                        if (signal.OkCallback != null) 
                            txtBoxMobile.AddOkOnClickCallback(signal.OkCallback);
                    }
                    break;
            
                case PopupOverlayType.LanguageBox:
                    if (DeviceInfo.IsMobile())
                    {
                        _popupLanguageView.gameObject.SetActive(true);
                    }
                    break;
            
                case PopupOverlayType.RecordingFrame:
                    
                    if (!_recordingFramePopup)
                    {
                        _recordingFramePopup = _popupOverlayPCFactory.Create();
                        _recordingFramePopup.SetupRecordingFrame();
                    }
                
                    _recordingFramePopup.gameObject.SetActive(signal.Active);
                    if(signal.Active)
                        _recordingFramePopup.transform.SetAsLastSibling();
                    break;
            }
        }
        
        private void OnResetPopup()
        {
            _popupOverlayView.ResetProgressPanel();
        }

        private void SetPopupData(string title, string cultureCode = "", string localizationKey = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                if (!string.IsNullOrEmpty(cultureCode) && !string.IsNullOrEmpty(localizationKey))
                {
                    title = _localizationModel.GetSystemTranslations(cultureCode, localizationKey);
                    _popupOverlayView.SetText(title, _accessibilityModel.ModulesFontSizeScaler);
                }
            }
            else
            {
                _popupOverlayView.SetText(title, _accessibilityModel.ModulesFontSizeScaler);
            }
            
            if (DeviceInfo.IsPCInterface())
            {
                _popupOverlayView.SetCancelText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.CancelKey), _accessibilityModel.ModulesFontSizeScaler);
                _popupOverlayView.SetOKText(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.CloseKey), _accessibilityModel.ModulesFontSizeScaler);
            }
        }

        private void CreateMoreDropdownMobile(MoreDropdownClickViewSignal signal)
        {
            var txtBox = _popupOverlayPCFactory.Create() as PopupOverlayMobile;
            txtBox.InitUiComponents();

            //var currentSystemTranslations = _localizationModel.AllSystemTranslations[_localizationModel.CurrentLanguageCultureCode];

            txtBox._canvasPanel.SetPanelName(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MenuModulesKey));
            txtBox._canvasPanel.SetRightButtonName(_localizationModel.GetCurrentSystemTranslations(LocalizationConstants.CancelKey));
            
            var clickedAsset = _contentModel.GetAssetById(signal.Id);
            
            var quizActivityData = new ActivityData
            {
                ActivityCount = clickedAsset.Quiz.Count,
                CanvasPanel = txtBox._canvasPanel,
                ButtonName = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MoreQuizKey),
                SignalSource = new CreateDropdownMultipleQuizMobileCommandSignal(signal.Id, txtBox._canvasPanel.contentPanel),
                OnClickMethod = () => OnQuizClick(signal.Id)
            };
            
            var puzzleActivityData = new ActivityData
            {
                ActivityCount = clickedAsset.Puzzle.Count,
                CanvasPanel = txtBox._canvasPanel,
                ButtonName = _localizationModel.GetCurrentSystemTranslations(LocalizationConstants.MorePuzzleKey),
                SignalSource = new CreateDropdownMultiplePuzzleMobileCommandSignal(signal.Id, txtBox._canvasPanel.contentPanel),
                OnClickMethod = () => OnPuzzleClick(signal.Id)
            };
            
            ProcessActivities(quizActivityData);
            ProcessActivities(puzzleActivityData);

            txtBox.ShowContentPanel();
        }

        private class ActivityData
        {
            public int ActivityCount;
            public CanvasPanel CanvasPanel;
            public string ButtonName;
            public ISignal SignalSource;
            public UnityAction OnClickMethod;
        }
        
        private void ProcessActivities(ActivityData activityData)
        {
            if (activityData.ActivityCount > 0)
            {
                HideCanvasItems(activityData);
                
                if (activityData.ActivityCount == 1)
                {
                    CreateSingleActivity(activityData);
                }
                else
                {
                    CreateMultipleActivities(activityData);
                }
            }
        }

        private void HideCanvasItems(ActivityData activityData)
        {
            activityData.CanvasPanel.contentPanel.template.gameObject.SetActive(false);
            activityData.CanvasPanel.contentPanel.textBoxTemplate?.gameObject.SetActive(false);
        }

        private void CreateSingleActivity(ActivityData activityData)
        {
            var quizBtn = activityData.CanvasPanel.contentPanel.CreateButton(activityData.ButtonName) as Button;
            quizBtn.onClick.AddListener(activityData.CanvasPanel.onHide.RemoveAllListeners);
            quizBtn.onClick.AddListener(activityData.CanvasPanel.CloseCanvasPanel);
            quizBtn.onClick.AddListener(() => activityData.CanvasPanel.onHide.AddListener(activityData.OnClickMethod));
        }
        
        private void CreateMultipleActivities(ActivityData activityData)
        {
            _signal.Fire(activityData.SignalSource);
        }

        private void OnPuzzleClick(int id)
        {
            _contentModel.AssetDetailsSignalSource = new StartPuzzleCommandSignal(id, 0);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {id}));
        }

        private void OnQuizClick(int id)
        {
            _contentModel.AssetDetailsSignalSource = new StartQuizCommandSignal(id, 0);
            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {id}));
        }

        private void OnCancelProgressClick()
        {
            OnCloseProgressClick();

            if (IsMultipleActivitySelected())
            {
                _signal.Fire<CancelMultipleDownloadProgressCommandSignal>();
            }
            else
            {
                _signal.Fire(new CancelDownloadProgressCommandSignal(_contentModel.SelectedAsset.Asset.Id));
            }

            ResetSelectedActivities();
        }

        private void OnCloseProgressClick()
        {
            ShowPopup(false);
            _popupOverlayView.ShowProgressPanel(false);
        }

        private bool IsMultipleActivitySelected()
        {
            return _contentModel.SelectedAsset.IsMultipleQuizSelected
                   || _contentModel.SelectedAsset.IsMultiplePuzzleSelected
                   || _contentModel.SelectedAsset.IsClassificationSelected;
        }

        private void ShowPopup(bool status)
        {
            _homeModel.IsPopupProgressVisible = status;
            _popupOverlayView.ShowPopup(status);
        }

        private void ResetSelectedActivities()
        {
            if (_contentModel.SelectedAsset.IsPuzzleSelected)
            {
                _contentModel.SelectedAsset.IsPuzzleSelected = false;
            }

            if (_contentModel.SelectedAsset.IsQuizSelected)
            {
                _contentModel.SelectedAsset.IsQuizSelected = false;
            }

            if (_contentModel.SelectedAsset.IsMultipleQuizSelected)
            {
                _contentModel.SelectedAsset.IsMultipleQuizSelected = false;
            }

            if (_contentModel.SelectedAsset.IsMultiplePuzzleSelected)
            {
                _contentModel.SelectedAsset.IsMultiplePuzzleSelected = false;
            }

            if (_contentModel.SelectedAsset.IsClassificationSelected)
            {
                _contentModel.SelectedAsset.IsClassificationSelected = false;
            }
        }
    }
}