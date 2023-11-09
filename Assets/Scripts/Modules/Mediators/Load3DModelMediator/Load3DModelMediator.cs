using System;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class Load3DModelMediator : IInitializable, IDisposable
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private Load3DModelContainer load3DModelContainer;
        [Inject] private readonly SignalBus _signal;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;
        [Inject] private Content3DModelHistory _content3DModelHistory;
        [Inject] private AssetBundleModel _assetBundleModel;
        [Inject] private UserLoginModel _loginModel;

        [InjectOptional] private ContentViewMediator _contentPCViewMediator;
        [InjectOptional] private ContentViewMobileMediator _contentMobileViewMediator;

        private AssetBundle _assetBundle;
	
        public void Initialize()
        {
            SubscribeOnListeners();

            _signal.Fire(new LoadAssetCommandSignal(GetAssetId(), ModelLoaded));
        }

        private void ModelLoaded(bool isLoaded, int id, GameObject model, string msg)
        {
            if (isLoaded)
            {
                ShowView(model);
            }
            else
            {
                _signal.Fire(new SendCrashAnalyticsCommandSignal($"Load Asset Error /: {msg}"));
                
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new PopupOverlaySignal(true, 
                    //You will be redirected to the home screen.
                    $"Load Asset Error:" + $" {msg}\nAborted.", 
                    type: PopupOverlayType.TextBox, okCallback: () =>
                    {
                        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount-1));
                        Dispose();

                        if (!_loginModel.IsLoggedAsUser)
                            return;
                        
                        _signal.Fire(new SetHomeTabsVisibilityViewSignal(true));
                        _signal.Fire(new HomeTabRecentClickViewSignal());
                    }));
            }
        }

        private void ShowView(GameObject go)
        {
            Debug.Log("Load3dModelMediator => ShowView");
            _signal.Fire(new PopupOverlaySignal(false));
            _signal.Fire<ShowModuleScreenCommandSignal>();

            if (DeviceInfo.IsPCInterface())
            {
                _contentPCViewMediator.InitializeModule(go);
            }
            else
            {
                _contentMobileViewMediator.InitializeModule(go);
            }
        }

        private void SubscribeOnListeners()
        {
            if (DeviceInfo.IsPCInterface())
            {
                _contentPCViewMediator._contentView.backButton.onClick.AddListener(load3DModelContainer.CloseModule);
            }
            else
            {
                _contentMobileViewMediator._contentView.backButton.onClick.AddListener(load3DModelContainer.CloseModule);
            }
        }
	
        private int GetAssetId()
        {
            return GetSelectedAsset().Asset.Id;
        }

        private ClientAssetModel GetSelectedAsset()
        {
            return _contentModel.SelectedAsset;
        }

        public void Dispose()
        {
            _assetBundleModel.UnloadAllAssetBundles();
        }
    }
}