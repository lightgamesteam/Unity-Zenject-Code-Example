using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TDL.Constants;
using Zenject;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;

namespace TDL.Modules.UserContentViewer
{
    public class UserContentViewerMediator : IInitializable, IDisposable
    {
        [Inject] private UserContentAppModel _userContentAppModel;
        [Inject] private readonly SignalBus _signal;
        [Inject] private ICacheService _cacheService;
        [Inject] private ServerService _serverService;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private UserContentView _userContentView;
        [Inject] private AsyncProcessorService _asyncProcessor;

        private UserContent _currentUserContent;

        public void Initialize()
        {
            _signal.Fire(new PopupOverlaySignal(false));
            
            LoadUserContent();
            UpdateToolTip();
        }

        private void LoadUserContent()
        {
            _currentUserContent = _userContentAppModel.GetUserContentById(_userContentAppModel.CurrentUserContentId);
            _userContentView.GetPanelTitleText().text = _currentUserContent.Name;
            _userContentView.GetImageViewPanel().SetActive(false);
            
            if(_userContentView.GetVideoViewPanel(out GameObject videoPlayerPanel))
                videoPlayerPanel.SetActive(false);

            switch (_currentUserContent.ContentTypeId)
            {
                case UserContentTypeIDConstants.Image:
                    ShowImage();
                    break;
                
                case UserContentTypeIDConstants.Video:
                    ShowVideo();
                    break;
            }
        }

        private void ShowImage()
        {
            #if UNITY_WEBGL
            //Create caching here
            // if (_cachedTexturesDict.TryGetValue(_currentUserContent.FileUrl, out var texture))
            // {
            //     _signal.Fire(new AddObjectToMemoryManagerSignal(SceneNameConstants.ModuleUserContentViewer, texture));
            //         
            //     _userContentView.GetImageView().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            //     _userContentView.GetImageViewPanel().SetActive(true);
            //     _userContentView.GetProgressText().transform.parent.gameObject.SetActive(false);
            // }
            // else
            // {
            // }
            _serverService.GetUserContentFile(_currentUserContent.FileUrl, IsDownloadedWebGL);

            return;
#endif
            if (File.Exists(_currentUserContent.FilePath))
            {
                IsLoaded(true, File.ReadAllBytes(_currentUserContent.FilePath));
            }
            else
            {
                _serverService.GetUserContentFile(_currentUserContent.FileUrl, IsDownloaded);
            }
            void IsDownloadedWebGL(bool isLoaded, int progress, byte[] data)
            {
                if (!isLoaded)
                {
                    _userContentView.GetProgressText().text = progress + " %";
                    return;
                }
                
                _userContentView.GetProgressText().text = string.Empty;
                IsLoaded(isLoaded, data);
            }

            void IsDownloaded(bool isLoaded, int progress, byte[] data)
            {
                if (!isLoaded)
                {
                    _userContentView.GetProgressText().text = progress + " %";
                    return;
                }
                
                _userContentView.GetProgressText().text = string.Empty;

                var contentFolder = _userContentAppModel.IsTeacherContent 
                    ? _cacheService.GetPathToTeacherContentFolder(_currentUserContent.ContentTypeName) 
                    : _cacheService.GetPathToUserContentFolder(_currentUserContent.ContentTypeName);
                
                if (!Directory.Exists(Path.Combine(contentFolder)))
                    Directory.CreateDirectory(contentFolder);   

                File.WriteAllBytes(_currentUserContent.FilePath, data);
                IsLoaded(isLoaded, data);
            }
            
            void IsLoaded(bool isLoaded, byte[] data)
            {
                if (isLoaded)
                {
                    var texture = new Texture2D(1, 1);
                    texture.LoadImage(data);
                    _signal.Fire(new AddObjectToMemoryManagerSignal(SceneNameConstants.ModuleUserContentViewer, texture));
                    
                    _userContentView.GetImageView().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);;
                    _userContentView.GetImageViewPanel().SetActive(true);
                    _userContentView.GetProgressText().transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _userContentView.GetImageViewPanel().SetActive(false);
                    _signal.Fire(new PopupOverlaySignal(true, "Error", type: PopupOverlayType.MessageBox));
                }
            }
        }

        private void ShowVideo()
        {
#if UNITY_WEBGL
            
            _serverService.GetUserContentFileWebGL(_currentUserContent.FileUrl, IsDownloadedWebGL);
            return;
#endif
            if (File.Exists(_currentUserContent.FilePath))
            {
                IsLoaded();
            }
            else
            {
                Debug.Log($"Downloading {_currentUserContent.FileUrl}");
                _serverService.GetUserContentFile(_currentUserContent.FileUrl, IsDownloaded);
            }

            void IsDownloaded(bool isLoaded, int progress, byte[] data)
            {
                if (!isLoaded)
                {
                    _userContentView.GetProgressText().text = progress + " %";
                    return;
                }
                
                _userContentView.GetProgressText().text = string.Empty;

                var contentFolder = _userContentAppModel.IsTeacherContent 
                    ? _cacheService.GetPathToTeacherContentFolder(_currentUserContent.ContentTypeName) 
                    : _cacheService.GetPathToUserContentFolder(_currentUserContent.ContentTypeName);
                
                if (!Directory.Exists(Path.Combine(contentFolder)))
                    Directory.CreateDirectory(contentFolder);   
                    
                File.WriteAllBytes(_currentUserContent.FilePath, data);
                FileExist();

                void FileExist()
                {
                    _asyncProcessor.Wait(0.1f, () =>
                    {
                        if (File.Exists(_currentUserContent.FilePath))
                            IsLoaded();
                        else
                            FileExist();
                    });
                }
            }

            void IsDownloadedWebGL(bool isLoaded, int progress, string url)
            {
                if (!isLoaded)
                {
                    _userContentView.GetProgressText().text = progress + " %";
                    return;
                }
                
                _userContentView.GetProgressText().text = string.Empty;

                IsLoadedWebGL(url);
            }
            
            void IsLoaded()
            {
                if (_userContentView.GetVideoViewPanel(out GameObject videoPlayerPanel))
                {
                    videoPlayerPanel.SetActive(true);
                    _userContentView.GetVideoView().InitializeModule(_currentUserContent.FilePath);
                    _userContentView.GetProgressText().transform.parent.gameObject.SetActive(false);
                }
            }
            
            void IsLoadedWebGL(string url)
            {
                if (_userContentView.GetVideoViewPanel(out GameObject videoPlayerPanel))
                {
                    videoPlayerPanel.SetActive(true);
                    _userContentView.GetVideoView().InitializeModule(url);
                    _userContentView.GetProgressText().transform.parent.gameObject.SetActive(false);
                }
            }
        }
        
        private void UpdateToolTip()
        {
            FindComponentExtension.GetAllInScene<TooltipEvents>(SceneNameConstants.ModuleUserContentViewer).ForEach(t =>
            {
                t.SetHint(_localizationModel.GetCurrentSystemTranslations(t.GetKey()));
            });
            
            FindComponentExtension.GetAllInScene<ToggleTooltipEvents>(SceneNameConstants.ModuleUserContentViewer).ForEach(tte =>
            {
                tte.SetTrueHint(_localizationModel.GetCurrentSystemTranslations(tte.GetTrueKey()));
                tte.SetFalseHint(_localizationModel.GetCurrentSystemTranslations(tte.GetFalseKey()));
            });
        }
        
        public void Dispose()
        {
        }
    }
}