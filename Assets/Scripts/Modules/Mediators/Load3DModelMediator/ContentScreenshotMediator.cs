using System.Linq;
using UnityEngine;
using Zenject;
using DG.Tweening;
using TDL.Constants;
using TDL.Models;
using TDL.Modules.Model3D.View;
using TDL.Signals;
using TDL.Views;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ContentScreenshotMediator : IInitializable, IMediator
{
    //[Inject] private ContentViewModel _contentViewModel;
    [Inject] private AsyncProcessorService _asyncProcessor;
    [Inject] private readonly SignalBus _signal;
    [Inject] private FileManager _fileManager;
    [Inject] private LocalizationModel _localization;
    //[Inject] private ContentModel _contentModel;
    [Inject] private ScreenshotView.Factory _factory;

    //private ContentViewPC _contentView => _contentViewModel.contentViewPC;
    
    private ScreenshotView _view;
    private string _cultureCode;
    private int _assetID;
    
    private Texture2D screenshot;
    private RectTransform screenshotRect;
    private bool needUpdateUserContent;
    private const string IconName = "Image_ico"; 
    
    public void Initialize()
    {
        _view = _factory.Create();
        _view.gameObject.SetActive(false);
        
        _view.shotCloseButton.onClick.AddListener(CloseScreenshot);
        _view.shotSaveButton.onClick.AddListener(SaveScreenshot);
        _view.shotSendButton.onClick.AddListener(SendScreenshot);
        screenshotRect = _view.shotImage.GetComponent<RectTransform>();
        _signal.Fire(new AddObjectToMemoryManagerSignal(SceneNameConstants.Module3DModel, screenshot));
        _signal.Subscribe<TakeScreenshotSignal>(TakeScreenshotSignal);
    }

    private void SaveUserContentServerResponse(SaveUserContentServerResponseSignal signal)
    {
        if (signal.IsUploaded == false)
        {
            _signal.Fire(new PopupOverlaySignal(true, $"{signal.Progress} %"));

            return;
        }
        
        _signal.TryUnsubscribe<SaveUserContentServerResponseSignal>(SaveUserContentServerResponse);

        _signal.Fire(new PopupOverlaySignal(false));

        if (signal.Response.Success)
        {
            SetButtonInteractable(_view.shotSendButton, false);
            _signal.Fire(new PopupOverlaySignal(true, _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.ServerFileSavedKey), type: PopupOverlayType.MessageBox));
        }
        else
        {
            SetButtonInteractable(_view.shotSendButton, true);
            _signal.Fire(new PopupOverlaySignal(true, signal.Response.ErrorMessage, type: PopupOverlayType.TextBox));
        }
    }

    private void TakeScreenshotSignal(TakeScreenshotSignal signal)
    {
        _cultureCode = signal.CultureCore == "" ? _localization.CurrentLanguageCultureCode : signal.CultureCore;;
         _assetID = signal.AssetID;
        TakeScreenshot();
    }
    
    private void TakeScreenshot()
    {
        UpdateScreenshotLocalization();

        TooltipPanel.Instance?.SetDisableTooltip();
        
        SetButtonInteractable(_view.shotSaveButton, true);
        SetButtonInteractable(_view.shotSendButton, true);
        
        _asyncProcessor.Wait(0, Take);
        
        void Take()
        {
            if(screenshot != null)
                Object.DestroyImmediate(screenshot);
            
            screenshot = ScreenCapture.CaptureScreenshotAsTexture();
            _view.shotImage.sprite = Sprite.Create(screenshot,
                new Rect(0.0f, 0.0f, Screen.width, Screen.height), new Vector2(0.5f, 0.5f));
            
             Show();
        }

        void Show()
        {
            _signal.Fire(new FocusKeyboardNavigationSignal(_view.canvasGroup, true));

            screenshotRect.offsetMax = new Vector2(screenshotRect.offsetMax.x, 0);
            screenshotRect.offsetMin = new Vector2(screenshotRect.offsetMin.x, 0);
            
            _view.gameObject.SetActive(true);

            _asyncProcessor.Wait(0, () =>
            {
                DOTween.To(() => screenshotRect.offsetMax, value => screenshotRect.offsetMax = value, new Vector2(screenshotRect.offsetMax.x,-100), 0.3f);
                DOTween.To(() => screenshotRect.offsetMin, value => screenshotRect.offsetMin = value, new Vector2(screenshotRect.offsetMin.x,180), 0.3f);
            });
        }
    }
    
    private void UpdateScreenshotLocalization()
    {
        _view.GetComponentsInChildren<TooltipEvents>(true).ToList().ForEach(te =>
        {
            te.SetHint(_localization.GetSystemTranslations(_cultureCode, te.GetKey()));
        });
    }
    
    private void CloseScreenshot()
    {
        _signal.Fire(new FocusKeyboardNavigationSignal(_view.canvasGroup, false));

        _view.gameObject.SetActive(false);
        Object.Destroy(screenshot);
    }
    
    private void SaveScreenshot()
    {
        _signal.Fire(new PopupInputViewSignal(_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.SaveKey) + "?", 
            _cultureCode, Save));
        
        void Save(bool isSubmit, string name)
        {
            if (isSubmit)
            {
                var pngTexture = screenshot.EncodeToPNG();
                string saveTo = _fileManager.SaveFileToMyContentFolder(pngTexture, name, FileExtension.DefaultScreenshotExtension);
                SetButtonInteractable(_view.shotSaveButton, false);
                
                _signal.Fire(new PopupOverlaySignal(true, $"{_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.LocalFileSavedKey)} {saveTo}", type: PopupOverlayType.TextBox));
            }
        }
    }
    
    private void SendScreenshot()
    {
        var fileBytes = screenshot.EncodeToPNG();
        _signal.Fire(new PopupInputViewSignal(_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloudKey) + "?", 
            _cultureCode, Send));

        void Send(bool isSubmit, string name)
        {
            if(isSubmit)
            {
                needUpdateUserContent = true;
                _signal.Subscribe<SaveUserContentServerResponseSignal>(SaveUserContentServerResponse);
                _signal.Fire(new PopupOverlaySignal(true, $"{_localization.GetSystemTranslations(_cultureCode, LocalizationConstants.CloudKey)}..."));
                SetButtonInteractable(_view.shotSendButton, false);
                _signal.Fire(new SaveUserContentServerCommandSignal(UserContentTypeIDConstants.Image, _assetID, name,
                    name + FileExtension.DefaultScreenshotExtension, fileBytes));
            }
        }
    }

    private void SetButtonInteractable(Button btn, bool isInteractable)
    {
        btn.interactable = isInteractable;

        btn.transform.Get<Image>(IconName).color =
            isInteractable ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    public void OnViewEnable(){}

    public void OnViewDisable(){}
}
