using TDL.Commands;
using UnityEngine;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class Load3DModelInstaller : MonoInstaller
    {
        [Inject] private ScreenContainer _screenContainer;
        [Inject] private GameObject Label3DPrefab;
        [Inject] private GameObject MultiViewPrefab;
        
        public override void InstallBindings()
        {
            switch (DeviceInfo.GetUI())
            {
                case DeviceUI.PC:
                    InstallPC();
                    break;
                
                case DeviceUI.Mobile:
                    Container.BindInterfacesAndSelfTo<ContentViewMobileMediator>().AsSingle();
                    break;
            }
            
            Container.Bind<AssetBundleModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<Load3DModelMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<AugmentedRealityMediator>().AsSingle();

            Container.DeclareSignal<LoadThumbnailsCommandSignal>();
            Container.BindSignal<LoadThumbnailsCommandSignal>().ToMethod<LoadThumbnailsCommand>(signal => signal.Execute).FromNew();
            
            InstallServices();
            InstallViews();
            InstallSignals();
        }

        private void InstallPC()
        {
            Container.BindInterfacesAndSelfTo<ContentViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContentHomeViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContentLessonModeViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContentMultipartViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContentMultimodelViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ContentMultimodelPartViewMediator>().AsSingle();
        }
        
        private void InstallServices()
        {
            Container.Bind<IModule3dServerService>().To<Module3dServerService>().AsSingle().NonLazy();
        }

        private void InstallViews()
        {
            Container.BindFactory<string, Color, LabelData, LabelData.Factory>()
                .FromComponentInNewPrefab(Label3DPrefab)
                .UnderTransformGroup("3DModelContainer/3DLabel");
            
            switch (DeviceInfo.GetUI())
            {
                case DeviceUI.PC:
                    Container.BindFactory<ContentHomeView, ContentHomeView.Factory>()
                        .FromComponentInNewPrefab(_screenContainer.screenHome)
                        .WithGameObjectName("ContentHomeScreen")
                        .UnderTransformGroup("3DUIContainer");
                    
                    Container.BindFactory<ContentViewBase, ContentViewBase.Factory>()
                        .FromComponentInNewPrefab(_screenContainer.screenPC)
                        .WithGameObjectName("ContentScreenPC")
                        .UnderTransformGroup("3DUIContainer");
                    
                    Container.BindFactory<int, MultiViewType, MultiView, MultiView.Factory>()
                        .FromComponentInNewPrefab(_screenContainer.multiView)
                        .WithGameObjectName("MultiView");
                    
                    break;
                
                case DeviceUI.Mobile:
                    Container.BindFactory<ContentViewBase, ContentViewBase.Factory>()
                        .FromComponentInNewPrefab(_screenContainer.screenMobile)
                        .WithGameObjectName("ContentScreenMobile")
                        .UnderTransformGroup("3DUIContainer");
                    
                    break;
            }
        }

        private void InstallSignals()
        {
            Container.DeclareSignal<SubscribeOnContentViewSignal>().OptionalSubscriber();
            Container.DeclareSignal<UnsubscribeFromContentViewSignal>().OptionalSubscriber();
            
            Container.DeclareSignal<DownloadAssetCommandSignal>();
            Container.BindSignal<DownloadAssetCommandSignal>().ToMethod<DownloadAssetCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<LoadAssetCommandSignal>();
            Container.BindSignal<LoadAssetCommandSignal>().ToMethod<LoadAssetCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<DownloadBackgroundCommandSignal>();
            Container.BindSignal<DownloadBackgroundCommandSignal>().ToMethod<DownloadBackgroundCommand>(signal => signal.Execute).FromNew();
            
            // asset details
            Container.DeclareSignal<Module3dStartAssetDetailsCommandSignal>();
            Container.BindSignal<Module3dStartAssetDetailsCommandSignal>().ToMethod<Module3dStartAssetDetailsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<Module3dDownloadAssetDetailsCommandSignal>();
            Container.BindSignal<Module3dDownloadAssetDetailsCommandSignal>().ToMethod<Module3dDownloadAssetDetailsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<Module3dCreateAssetDetailsCommandSignal>();
            Container.BindSignal<Module3dCreateAssetDetailsCommandSignal>().ToMethod<Module3dCreateAssetDetailsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<Module3dProcessAssetDetailsCommandSignal>();
            Container.BindSignal<Module3dProcessAssetDetailsCommandSignal>().ToMethod<Module3dProcessAssetDetailsCommand>(signal => signal.Execute).FromNew();
            
            // search
            Container.DeclareSignal<Module3dGetSearchAssetsCommandSignal>();
            Container.BindSignal<Module3dGetSearchAssetsCommandSignal>().ToMethod<Module3dGetSearchAssetsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SearchAssetCommandSignal>();
            Container.BindSignal<SearchAssetCommandSignal>().ToMethod<SearchAssetCommand>(signal => signal.Execute).FromNew();
            
            // AR
            Container.DeclareSignal<StartAugmentedRealitySignal>();
        }
    }
}