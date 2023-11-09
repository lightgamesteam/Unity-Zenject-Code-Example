using System;
using System.Linq;
using TDL.Constants;
using Zenject;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDL.Modules.Model3D
{
    public class VideoPlayerMediator : IInitializable, IDisposable
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private VideoPlayerBase _videoPlayerBase;
        [Inject] private ICacheService _cacheService;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private ScreenRecorderManager _screenRecorder;

        private CanvasGroup _uiContainerCanvasGroup;
        public bool IsLoadedVideo { get; set; }

        public void Initialize()
        {
            _uiContainerCanvasGroup = FindComponentExtension.GetInScene<CanvasGroup>("UIContainer", SceneNameConstants.Main);
            var selectedAsset = GetSelectedAsset();
            IsLoadedVideo = _cacheService.IsAssetExists(selectedAsset.Id, selectedAsset.Version);
            
            _signal.Subscribe<VideoRecordingStateSignal>(_videoPlayerBase.OnChangeRecordingState);

            _videoPlayerBase.SetAssetID(selectedAsset.Id);
            _videoPlayerBase.ClearTexture();
            PlayVideo();
            HidePopupOverlay();
            SetInteractableMainMenu(false);
            SetStudentNoteVisibility();
            SetVideoRecordingToggleVisibility();
            UpdateToolTip();
        }

        private void UpdateToolTip()
        {
            _videoPlayerBase.GetComponentsInChildren<TooltipEvents>(true).ToList().ForEach(te =>
            {
                te.SetHint(_localizationModel.GetCurrentSystemTranslations(te.GetKey()));
            });
            
            _videoPlayerBase.GetComponentsInChildren<ToggleTooltipEvents>(true).ToList().ForEach(tte =>
            {
                tte.SetTrueHint(_localizationModel.GetCurrentSystemTranslations(tte.GetTrueKey()));
                tte.SetFalseHint(_localizationModel.GetCurrentSystemTranslations(tte.GetFalseKey()));
            });
        }

        private void SetStudentNoteVisibility()
        {
            _videoPlayerBase.SetStudentNoteVisibility(!_userLoginModel.IsTeacher);
        }
        
        private void SetVideoRecordingToggleVisibility()
        {
//            bool isSceneLoaded = SceneManager.GetAllScenes().ToList().Contains(SceneManager.GetSceneByName(SceneNameConstants.Module3DModel));
            
            _videoPlayerBase.SetVideoRecordingToggleVisibility(!_screenRecorder.IsRecording());
        }
        
        private void PlayVideo()
        {
            var videoPath = string.Empty;
            Debug.Log($"Is loaded {IsLoadedVideo} {GetSelectedAsset().Name} {GetSelectedAsset().VimeoUrl}");
            if (IsLoadedVideo)
            {
                var selectedAsset = GetSelectedAsset();
                videoPath = _cacheService.GetPathToAsset(selectedAsset.Id, selectedAsset.Version);
                videoPath = _cacheService.DecryptVideoAsset(videoPath);
            }
            else
            {
                videoPath = GetSelectedAsset().VimeoUrl;
            }

            _videoPlayerBase.InitializeModule(videoPath);
        }

        private void HidePopupOverlay()
        {
            _signal.Fire(new PopupOverlaySignal(false));
        }

        private Asset GetSelectedAsset()
        {
            return _contentModel.SelectedAsset.Asset;
        }

        private void EncryptVideoAsset()
        {
            var selectedAsset = GetSelectedAsset();
            var videoPath = _cacheService.GetPathToAsset(selectedAsset.Id, selectedAsset.Version);
            _cacheService.EncryptVideoAsset(videoPath);
        }

        void SetInteractableMainMenu(bool status)
        {
            if(!status)
                EventSystem.current.SetSelectedGameObject(null);
        
            _uiContainerCanvasGroup.interactable = status;
            _uiContainerCanvasGroup.blocksRaycasts = status;
        }

        public void Dispose()
        {
            _signal.Unsubscribe<VideoRecordingStateSignal>(_videoPlayerBase.OnChangeRecordingState);

            SetInteractableMainMenu(true);

            if (IsLoadedVideo)
            {
                EncryptVideoAsset();
            }
        }
    }
}