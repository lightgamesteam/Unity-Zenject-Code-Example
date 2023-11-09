using System;
using CI.TaskParallel;
using Commands;
using Commands.Common;
using Commands.Tools;
using Managers;
using Signals;
using Signals.Localization;
using Signals.Login;
using Signals.Tools;
using UnityEngine;
using Zenject;
using TDL.Models;
using TDL.Views;
using TDL.Commands;
using TDL.Modules.Model3D.View;
using TDL.Signals;
using TDL.Services;

namespace TDL.Core
{
    public class GameInstaller : MonoInstaller
    {
        [Inject] private DebugSettings _debugSettings;
        [Inject] private ScreenPrefabs _screenPrefabs;
        [Inject] private UIElementPrefabs _elementPrefabs;

        public override void InstallBindings()
        {
            SetDebugSettings();
            SetTargetFPSLimit();
            InitMultiThreading();
            InstallServices();
            InstallModels();
            InstallViews();
            InstallMediators();
            InstallViewSignals();
            InstallCommandSignals();
            InstallManagers();
	    }

        private void SetDebugSettings()
        {
            Debug.unityLogger.logEnabled = _debugSettings.Log;
        }

        private void SetTargetFPSLimit()
        {
            Application.targetFrameRate = 60;
        }

        private void InitMultiThreading()
        {
            UnityTask.InitialiseDispatcher();
        }

    #region Views
        
        private void InstallViews()
        {
            if (DeviceInfo.IsPCInterface())
            {
                PCViews();
            }
            else
            {
                MobileViews();
            }

            Container.BindFactory<GradeMenuItemView, GradeMenuItemView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.GradeMenuItem);
            
            Container.BindFactory<ModulesMenuItemView, ModulesMenuItemView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.ModulesMenuItem);
            
            Container.BindFactory<SubjectMenuItemView, SubjectMenuItemView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.SubjectMenuItem);
            
            Container.BindFactory<PopupWarningARView, PopupWarningARView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupWarningAR)
                .WithGameObjectName("PopupWarningAR")
                .UnderTransformGroup("UIOverlayContainer");
        }
        
        private void PCViews()
        {
            Container.BindFactory<LoginViewBase, LoginViewBase.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.LoginScreenPC)
                .UnderTransformGroup("UIContainer");
            
            Container.BindFactory<HomeViewBase, HomeViewBase.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.HomeScreenPC)
                .UnderTransformGroup("UIContainer");
            
            Container.BindFactory<PopupOverlayBase, PopupOverlayBase.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupOverlayPC)
                .WithGameObjectName("PopupOverlayPC")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<PopupInputView, PopupInputView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupInputPC)
                .WithGameObjectName("PopupInputPC")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<PopupInfoView, PopupInfoView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupInfoPC)
                .WithGameObjectName("PopupInfoPC")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<PopupHotSpotListView, PopupHotSpotListView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupHotSpotListPC)
                .WithGameObjectName("PopupHotSpotListPC")
                .UnderTransformGroup("UIOverlayContainer");

            Container.BindFactory<DescriptionView, DescriptionView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupDescriptionPC)
                .WithGameObjectName("PopupDescriptionPC")
                .UnderTransformGroup("UIOverlayContainer");

            Container.BindFactory<PopupInternetConnectionView, PopupInternetConnectionView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupInternetConnectionPC)
                .WithGameObjectName("PopupInternetConnectionPC")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<StudentNotesView, StudentNotesView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.StudentNotesPC)
                .WithGameObjectName("StudentNotesView")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<VideoView, VideoView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.VideoViewPC)
                .WithGameObjectName("VideoViewPC")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<ScreenshotView, ScreenshotView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.ScreenshotViewPC)
                .WithGameObjectName("ScreenshotViewPC")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindMemoryPool<NoteItemView, NoteItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.NoteItemPC);
            
            //Activity
            Container.BindMemoryPool<ActivityItemView, ActivityItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityItem);
            
            Container.BindMemoryPool<ActivityQuizView, ActivityQuizView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityQuizzes);
            
            Container.BindMemoryPool<ActivityPuzzleView, ActivityPuzzleView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityPuzzles);
                
            Container.BindMemoryPool<ActivityMultipleQuizView, ActivityMultipleQuizView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityMultipleQuizzes);
            
            Container.BindMemoryPool<ActivityMultiplePuzzlesView, ActivityMultiplePuzzlesView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityMultiplePuzzles);

            Container.BindFactory<DropdownActivityItemView, DropdownActivityItemView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.DropdownActivityItem);
            
            // activity -> classification
            Container.BindMemoryPool<ActivityClassificationView, ActivityClassificationView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityClassification);
            
            Container.BindMemoryPool<ClassificationAssetItemView, ClassificationAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ClassificationAsset);
            //

            Container.BindMemoryPool<QuizAssetView, QuizAssetView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.QuizAsset);
            
            Container.BindMemoryPool<PuzzleAssetItemView, PuzzleAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.PuzzleAsset);
            
            Container.BindMemoryPool<MultipleQuizAssetItemView, MultipleQuizAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.MultipleQuizAsset);
            
            Container.BindMemoryPool<MultiplePuzzleAssetItemView, MultiplePuzzleAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.MultiplePuzzleAsset);
            
            Container.BindMemoryPool<TopicItemView, TopicItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.TopicItem);
            
            Container.BindMemoryPool<SubtopicItemView, SubtopicItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.SubtopicItem);

            Container.BindMemoryPool<AssetItemView, AssetItemView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_elementPrefabs.AssetItem)
                .UnderTransformGroup("TempAssetsContainer");


            Container.BindMemoryPool<WebAssetItemView, WebAssetItemView.Pool>()
            .WithInitialSize(10)
            .FromComponentInNewPrefab(_elementPrefabs.AssetItemWeb)
            .UnderTransformGroup("TempAssetsContainer");


            Container.BindMemoryPool<UserContentItemView, UserContentItemView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_elementPrefabs.UserContentItem)
                .UnderTransformGroup("TempAssetsContainer");
            
            // feedback
            Container.BindFactory<FeedbackPopupView, FeedbackPopupView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.FeedbackHomePCPopup)
                .WithGameObjectName("FeedbackPopup")
                .UnderTransformGroup("UIOverlayContainer");
        }

        private void MobileViews()
        {
            Container.BindFactory<LoginViewBase, LoginViewBase.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.LoginScreenMobile)
                .UnderTransformGroup("UIContainerMobile");

            Container.BindFactory<HomeViewBase, HomeViewBase.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.HomeScreenMobile)
                .UnderTransformGroup("UIContainerMobile");

            Container.BindFactory<PopupOverlayBase, PopupOverlayBase.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupOverlayMobile)
                .WithGameObjectName("PopupOverlayMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<PopupInputView, PopupInputView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupInputMobile)
                .WithGameObjectName("PopupInputMobile")
                .UnderTransformGroup("UIOverlayContainer");
             
            Container.BindFactory<PopupInfoView, PopupInfoView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupInfoMobile)
                .WithGameObjectName("PopupInfoMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<PopupHotSpotListView, PopupHotSpotListView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupHotSpotListMobile)
                .WithGameObjectName("PopupHotSpotListMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<PopupInternetConnectionView, PopupInternetConnectionView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupInternetConnectionMobile)
                .WithGameObjectName("PopupInternetConnectionMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<CanvasPanel, CanvasPanel.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.PopupLanguageMobile)
                .WithGameObjectName("PopupLanguageMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<StudentNotesView, StudentNotesView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.StudentNotesMobile)
                .WithGameObjectName("StudentNotesView")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<VideoView, VideoView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.VideoViewMobile)
                .WithGameObjectName("VideoViewMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindFactory<ScreenshotView, ScreenshotView.Factory>()
                .FromComponentInNewPrefab(_screenPrefabs.ScreenshotViewMobile)
                .WithGameObjectName("ScreenshotViewMobile")
                .UnderTransformGroup("UIOverlayContainer");
            
            Container.BindMemoryPool<NoteItemView, NoteItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.NoteItemMobile);

            Container.BindMemoryPool<ActivityItemView, ActivityItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityItemMobile);
            
            Container.BindMemoryPool<ActivityQuizView, ActivityQuizView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityQuizzesMobile);
            
            Container.BindMemoryPool<ActivityPuzzleView, ActivityPuzzleView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityPuzzlesMobile);
                
            Container.BindMemoryPool<ActivityMultipleQuizView, ActivityMultipleQuizView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityMultipleQuizzesMobile);
            
            Container.BindMemoryPool<ActivityMultiplePuzzlesView, ActivityMultiplePuzzlesView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityMultiplePuzzlesMobile);

            Container.BindMemoryPool<QuizAssetView, QuizAssetView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.QuizAssetMobile);
            
            Container.BindMemoryPool<PuzzleAssetItemView, PuzzleAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.PuzzleAssetMobile);
            
            Container.BindMemoryPool<MultipleQuizAssetItemView, MultipleQuizAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.MultipleQuizAssetMobile);
            
            Container.BindMemoryPool<MultiplePuzzleAssetItemView, MultiplePuzzleAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.MultiplePuzzleAssetMobile);

            Container.BindMemoryPool<GradeItemView, GradeItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.GradeMenuItemMobile);
            
            Container.BindMemoryPool<SubjectItemView, SubjectItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.SubjectItemMobile);
            
            Container.BindMemoryPool<TopicItemView, TopicItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.TopicItemMobile);
            
            Container.BindMemoryPool<SubtopicItemView, SubtopicItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.SubtopicItemMobile);
            
            Container.BindMemoryPool<AssetItemView, AssetItemView.Pool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(_elementPrefabs.AssetItemMobile)
                .UnderTransformGroup("TempAssetsContainer");
            
            Container.BindMemoryPool<UserContentItemView, UserContentItemView.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(_elementPrefabs.UserContentItemMobile)
                .UnderTransformGroup("TempAssetsContainer");
            
            // activity -> classification
            Container.BindMemoryPool<ActivityClassificationView, ActivityClassificationView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ActivityClassificationMobile);
            
            Container.BindMemoryPool<ClassificationAssetItemView, ClassificationAssetItemView.Pool>()
                .FromComponentInNewPrefab(_elementPrefabs.ClassificationAssetMobile);
            
            // dropdown multiple activities
            Container.BindFactory<DropdownActivityHeaderMobileView, DropdownActivityHeaderMobileView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.DropdownActivityHeaderMobile);
            
            Container.BindFactory<DropdownActivityContainerMobileView, DropdownActivityContainerMobileView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.DropdownActivityContainerMobile);
            
            Container.BindFactory<DropdownActivityItemMobileView, DropdownActivityItemMobileView.Factory>()
                .FromComponentInNewPrefab(_elementPrefabs.DropdownActivityItemMobile);
        }
        
    #endregion

    #region Mediators

        private void InstallMediators()
        {
            if (DeviceInfo.IsPCInterface())
            {
                PCMediators();
            }
            else
            {
                MobileMediators();
            }
            
            // video recording
            Container.BindInterfacesAndSelfTo<ContentVideoRecorderMediator>().AsSingle();
            
            // screenshot
            Container.BindInterfacesAndSelfTo<ContentScreenshotMediator>().AsSingle();
            
            // splash screen
            Container.BindInterfacesAndSelfTo<SplashScreenMediator>().AsSingle();

            // overlay
            Container.BindInterfacesAndSelfTo<PopupOverlayMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<PopupInternetConnectionMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<StudentNotesMediator>().AsSingle();

            // login
            //Container.BindInterfacesAndSelfTo<WebLoginViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<WebLoginViewMediator>().AsSingle();
            
            // home
            Container.BindInterfacesAndSelfTo<HomeRightMenuViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<GradeMenuItemViewsMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<SubjectMenuItemViewsMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<TopicItemViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<SubtopicItemViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<AssetItemViewsMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<AssetItemViewsWebMediator>().AsSingle();

            // home -> activities
            Container.BindInterfacesAndSelfTo<ActivitiesItemViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ActivityQuizViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ActivityPuzzleViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ActivityMultipleQuizViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ActivityMultiplePuzzlesViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<QuizAssetViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<PuzzleAssetViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<MultipleQuizAssetViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<MultiplePuzzleAssetViewMediator>().AsSingle();
            
            // home -> activities -> classification
            Container.BindInterfacesAndSelfTo<ActivityClassificationViewMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ClassificationAssetViewMediator>().AsSingle();
        }
        
        private void PCMediators()
        {
            Container.BindInterfacesAndSelfTo<KeyboardNavigationMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<SelectableTextToSpeechMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<UserContentItemViewsMediator>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<HomeViewMediator>().AsSingle();
            
            // feedback
            Container.BindInterfacesAndSelfTo<FeedbackPopupMediator>().AsSingle();
            
            // Paint 
            Container.BindInterfacesAndSelfTo<ContentPaint3DMediator>().AsSingle();
        }

        private void MobileMediators()
        {
            //for module quiz
            Container.BindInterfacesAndSelfTo<KeyboardNavigationMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<SelectableTextToSpeechMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<UserContentItemViewsMediator>().AsSingle();
        
            Container.BindInterfacesAndSelfTo<ScreenOrientationMediator>().AsSingle();
            Container.BindInterfacesAndSelfTo<HomeViewMobileMediator>().AsSingle();
        }
        
    #endregion

    #region Services

        private void InstallServices()
        {
            Container.Bind<ServerService>().AsSingle().NonLazy();
            Container.Bind<ICacheService>().To<CacheService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WindowService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DebugLogConsoleService>().AsSingle().NonLazy();
            Container.Bind<AsyncProcessorService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }

    #endregion

    #region Models

        private void InstallModels()
        {   
            Container.Bind<UserLoginModel>().AsSingle();
            Container.Bind<DiscoveryDocumentModel>().AsSingle();
            Container.Bind<LocalizationModel>().AsSingle();
            Container.Bind<ContentModel>().AsSingle();
            Container.Bind<MetaDataModel>().AsSingle();
            Container.Bind<HomeModel>().AsSingle();
            Container.Bind<Content3DModelHistory>().AsSingle();
            Container.Bind<FeedbackModel>().AsSingle();
            Container.Bind<ClassificationDetailsModel>().AsSingle();
            Container.Bind<AccessibilityModel>().AsSingle();
            Container.Bind<ContentViewModel>().AsSingle();
            Container.Bind<PopupModel>().AsSingle();
            Container.Bind<UserContentAppModel>().AsSingle();

            Container.Bind<MainScreenModel>().AsSingle();
        }
        
    #endregion

    #region ViewSignals

        private void InstallViewSignals()
        {
            SignalBusInstaller.Install(Container);
            
            // paint
            Container.DeclareSignal<InitializePaintSignal>();
            
            // common
            Container.DeclareSignal<CreateScreensSignal>();
            Container.DeclareSignal<OnChangeLanguageClickViewSignal>();
            Container.DeclareSignal<ImageRecognitionResponseSignal>();
            Container.DeclareSignal<FocusKeyboardNavigationSignal>();
            Container.DeclareSignal<TakeScreenshotSignal>();
            Container.DeclareSignal<StartVideoRecordingSignal>();
            Container.DeclareSignal<VideoRecordingStateSignal>();

            // splash screen
            Container.DeclareSignal<SplashScreenRequiredTimeEndedViewSignal>();

            // login
            Container.DeclareSignal<LoginStateSignal>();
            Container.DeclareSignal<LoginClickViewSignal>();
            
            // home -> top menu
            Container.DeclareSignal<LeftMenuClickViewSignal>();
            Container.DeclareSignal<RightMenuClickViewSignal>();
            Container.DeclareSignal<HideSearchInputViewSignal>().OptionalSubscriber();
            Container.DeclareSignal<CloseAppClickViewSignal>();
            Container.DeclareSignal<SearchChangedViewSignal>();
            
            // home -> right menu
            Container.DeclareSignal<RecentlyViewedClickViewSignal>();
            Container.DeclareSignal<SaveRightMenuItemViewSignal>();
            Container.DeclareSignal<FavoritesClickViewSignal>();
            Container.DeclareSignal<MyContentClickViewSignal>();
            Container.DeclareSignal<RightMenuMyTeacherClickViewSignal>();
            Container.DeclareSignal<MetaDataClickViewSignal>();
            Container.DeclareSignal<SignOutClickViewSignal>();
            
            // home -> accessibility
            Container.DeclareSignal<AccessibilityTextToAudioClickViewSignal>();
            Container.DeclareSignal<AccessibilityGrayscaleClickViewSignal>();
            Container.DeclareSignal<AccessibilityLabelLinesClickViewSignal>();
            Container.DeclareSignal<AccessibilityMenuInitializedViewSignal>();
            Container.DeclareSignal<AccessibilityTTSPlayOnHoverViewSignal>();
            Container.DeclareSignal<AccessibilityFontSizeClickViewSignal>();

            Container.DeclareSignal<GradeMenuItemClickViewSignal>();
            Container.DeclareSignal<SubjectMenuItemClickViewSignal>();
            Container.DeclareSignal<TopicItemClickViewSignal>();
            Container.DeclareSignal<SubtopicItemClickViewSignal>();
            
            // asset item
            Container.DeclareSignal<AssetItemClickViewSignal>();
            Container.DeclareSignal<PuzzleClickViewSignal>();
            Container.DeclareSignal<QuizClickViewSignal>();
            Container.DeclareSignal<DescriptionClickViewSignal>();
            Container.DeclareSignal<FavouriteToggleClickViewSignal>();
            Container.DeclareSignal<DownloadToggleClickViewSignal>();
            Container.DeclareSignal<MoreDropdownClickViewSignal>();
            
            // user content item
            Container.DeclareSignal<UserContentItemClickViewSignal>();
            Container.DeclareSignal<UserContentItemDeleteClickViewSignal>();
            
            // home -> activities
            Container.DeclareSignal<ActivityItemClickViewSignal>();
            Container.DeclareSignal<ActivityQuizClickViewSignal>();
            Container.DeclareSignal<ActivityPuzzleClickViewSignal>();
            Container.DeclareSignal<ActivityMultipleQuizItemClickViewSignal>();
            Container.DeclareSignal<ActivityMultiplePuzzlesItemClickViewSignal>();
            Container.DeclareSignal<QuizAssetItemClickViewSignal>();
            Container.DeclareSignal<OnDropdownMultipleQuizClickViewSignal>();
            Container.DeclareSignal<OnDropdownMultiplePuzzleClickViewSignal>();
            Container.DeclareSignal<PuzzleAssetItemClickViewSignal>();
            Container.DeclareSignal<MultipleQuizAssetItemClickViewSignal>();
            Container.DeclareSignal<MultiplePuzzleAssetItemClickViewSignal>();
            
            // home -> activities -> classification
            Container.DeclareSignal<ActivityClassificationItemClickViewSignal>();
            Container.DeclareSignal<ClassificationAssetItemClickViewSignal>();
            
            // home tabs
            Container.DeclareSignal<SetHomeTabsVisibilityViewSignal>();
            Container.DeclareSignal<UpdateHomeTabsStatusViewSignal>();
            Container.DeclareSignal<HomeTabFavouritesClickViewSignal>();
            Container.DeclareSignal<HomeTabRecentClickViewSignal>();
            Container.DeclareSignal<HomeTabMyContentClickViewSignal>();
            Container.DeclareSignal<HomeTabMyTeacherClickViewSignal>();
            Container.DeclareSignal<FavoritesClickFromHomeTabsViewSignal>();
            Container.DeclareSignal<RecentlyViewedClickFromHomeTabsViewSignal>();
            Container.DeclareSignal<MyContentViewedClickFromHomeTabsViewSignal>();
            Container.DeclareSignal<MyTeacherViewedClickFromHomeTabsViewSignal>();
            
            // breadcrumbs
            Container.DeclareSignal<ResetAllBreadcrumbsViewSignal>();
            Container.DeclareSignal<SubjectHyperlinkClickViewSignal>();
            Container.DeclareSignal<TopicHyperlinkClickViewSignal>();
            Container.DeclareSignal<SubtopicHyperlinkClickViewSignal>();
            Container.DeclareSignal<ActivityHyperlinkClickViewSignal>();
            
            // popup overlay
            Container.DeclareSignal<OnCancelProgressClickViewSignal>();
            Container.DeclareSignal<OnCloseProgressClickViewSignal>();
            
            // popup overlay -> internet connection
            Container.DeclareSignal<PopupInternetConnectionRetryViewSignal>();
            Container.DeclareSignal<PopupInternetConnectionExitViewSignal>();

            // localization
            Container.DeclareSignal<OnLoginActivatedViewSignal>();
            Container.DeclareSignal<OnLoginDeactivatedViewSignal>();
            Container.DeclareSignal<OnHomeActivatedViewSignal>();
            Container.DeclareSignal<OnHomeDeactivatedViewSignal>();
            
            // feedback
            Container.DeclareSignal<ShowFeedbackPopupFromAssetViewSignal>();
            Container.DeclareSignal<ShowFeedbackPopupFromQuizViewSignal>();
            Container.DeclareSignal<ShowFeedbackPopupFromPuzzleViewSignal>();
            Container.DeclareSignal<ShowMainFeedbackPanelViewSignal>();
            Container.DeclareSignal<SendFeedbackViewSignal>();
            Container.DeclareSignal<CancelFeedbackViewSignal>();
            Container.DeclareSignal<FeedbackSentOkClickViewSignal>();
            Container.DeclareSignal<SubscribeOnFeedbackPopupViewSignal>();
            Container.DeclareSignal<UnsubscribeFromFeedbackPopupViewSignal>();
            
            Container.DeclareSignal<OnDescriptionCloseClickViewSignal>();
            Container.DeclareSignal<CloseDescriptionViewSignal>();
            Container.DeclareSignal<LoadAndPlayDescriptionViewSignal>();
            Container.DeclareSignal<PauseAllExceptActiveDescriptionViewSignal>();
            Container.DeclareSignal<OnDescriptionBlockModelMovementsViewSignal>();
        }
        
    #endregion

    #region CommandSignals

        private void InstallCommandSignals()
        {
            if (DeviceInfo.IsPCInterface())
            {
                PCCommandSignals();
            }
            else
            {
                MobileCommandSignals();
            }
            
            // common
            
            Container.DeclareSignal<SetFullscreenWebglCommandSignal>();
            Container.BindSignal<SetFullscreenWebglCommandSignal>().ToMethod<SetFullscreenWebglCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CheckCameraPermissionCommandSignal>();
            Container.BindSignal<CheckCameraPermissionCommandSignal>().ToMethod<CheckCameraPermissionCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<RequestWebGLMicrophoneSignal>();
            Container.BindSignal<RequestWebGLMicrophoneSignal>().ToMethod<RequestWebGLMicrophoneSignalCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ReloadSceneCommandSignal>();
            Container.BindSignal<ReloadSceneCommandSignal>().ToMethod<ReloadSceneCommand>(signal => signal.Execute).FromNew();
            Container.DeclareSignal<AddObjectToMemoryManagerSignal>();

            Container.DeclareSignal<RegisterScreenCommandSignal>();
            Container.BindSignal<RegisterScreenCommandSignal>().ToMethod<RegisterScreenCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ChangeLanguageCommandSignal>();
            Container.BindSignal<ChangeLanguageCommandSignal>().ToMethod<ChangeLanguageCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SaveCurrentLanguageToCacheSignal>();
            Container.BindSignal<SaveCurrentLanguageToCacheSignal>().ToMethod<SaveCurrentLanguageToCacheCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<OpenUrlCommandSignal>();
            Container.BindSignal<OpenUrlCommandSignal>().ToMethod<OpenUrlCommand>(signal => signal.Execute).FromNew();
            
            // Send Image / Video To Server
            Container.DeclareSignal<SaveUserContentServerCommandSignal>();
            Container.BindSignal<SaveUserContentServerCommandSignal>().ToMethod<SaveUserContentServerCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<SaveUserContentServerResponseCommandSignal>();
            Container.BindSignal<SaveUserContentServerResponseCommandSignal>().ToMethod<SaveUserContentServerResponseCommand>(signal => signal.Execute).FromNew();
            Container.DeclareSignal<SaveUserContentServerResponseSignal>();
            
            // cache
            Container.DeclareSignal<SaveAssetInCacheCommandSignal>();
            Container.BindSignal<SaveAssetInCacheCommandSignal>().ToMethod<SaveAssetInCacheCommand>(signal => signal.Execute).FromNew();
            
            // tools
            Container.DeclareSignal<ShowDebugLogCommandSignal>();
            Container.BindSignal<ShowDebugLogCommandSignal>().ToMethod<ShowDebugLogCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SendCrashAnalyticsCommandSignal>();
            Container.BindSignal<SendCrashAnalyticsCommandSignal>().ToMethod<SendCrashAnalyticsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CheckInternetConnectionCommandSignal>();
            Container.BindSignal<CheckInternetConnectionCommandSignal>().ToMethod<CheckInternetConnectionCommand>(signal => signal.Execute).FromNew();
            
            // thumbnails
            Container.DeclareSignal<CreateThumbnailsForAllPreviewSignal>();
            Container.BindSignal<CreateThumbnailsForAllPreviewSignal>().ToMethod<CreateThumbnailsForAllPreviewCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<DownloadThumbnailByIdSignal>();
            Container.BindSignal<DownloadThumbnailByIdSignal>().ToMethod<CreateThumbnailByIdCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<SaveThumbnailInCacheCommandSignal>();
            Container.BindSignal<SaveThumbnailInCacheCommandSignal>().ToMethod<SaveThumbnailInCacheCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SaveThumbnailInCacheCommandSignal>().ToMethod<SetThumbnailForViewCommand>(signal => signal.Execute).FromNew();
            
            // background
            Container.DeclareSignal<SaveBackgroundInCacheCommandSignal>();
            Container.BindSignal<SaveBackgroundInCacheCommandSignal>().ToMethod<SaveBackgroundInCacheCommand>(signal => signal.Execute).FromNew();
            
            // guest login
            Container.DeclareSignal<LoginAsGuestCommandSignal>();
            Container.BindSignal<LoginAsGuestCommandSignal>().ToMethod<LoginAsGuestCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<CheckLoginAsGuestCommandSignal>();
            Container.BindSignal<CheckLoginAsGuestCommandSignal>().ToMethod<CheckLoginAsGuestCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<LoadAssetsAsGuestLoginScreenSignal>();
            Container.BindSignal<LoadAssetsAsGuestLoginScreenSignal>().ToMethod<LoadAssetsAsGuestLoginScreenCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<MainScreenCreateAssetsCommandSignal>();
            Container.BindSignal<MainScreenCreateAssetsCommandSignal>().ToMethod<MainScreenOsaCreateAssetsCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<MainScreenOsaInitializeAssetSignal>();
            Container.BindSignal<MainScreenOsaInitializeAssetSignal>().ToMethod<MainScreenOsaInitializeAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<MainScreenOsaInitializeAssetSignal>().ToMethod<MainScreenOsaDownloadPreviewAssetCommand>(signal => signal.Execute).FromNew();

            // login
            //Container.DeclareSignal<SetCurrentHostCommandSignal>();
            Container.DeclareSignal<CreateDiscoveryDocumentModelCommandSignal>();
#if UNITY_WEBGL && !UNITY_EDITOR
            //Container.BindSignal<SetCurrentHostCommandSignal>().ToMethod<SetCurrentHostCommand>(signal => signal.Execute).FromNew();
#endif
            Container.BindSignal<CreateDiscoveryDocumentModelCommandSignal>().ToMethod<CreateDiscoveryDocumentModelCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<HandleRequestErrorSignal>();
            Container.BindSignal<HandleRequestErrorSignal>().ToMethod<HandleRequestErrorCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<TryTeamsSSOSignal>();
            Container.BindSignal<TryTeamsSSOSignal>().ToMethod<TryTeamsSSOCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<GetAvailableResourcesCommandSignal>();
            Container.BindSignal<GetAvailableResourcesCommandSignal>().ToMethod<GetAvailableResourcesCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<DownloadAvailableResourcesCommandSignal>();
            Container.BindSignal<DownloadAvailableResourcesCommandSignal>().ToMethod<DownloadAvailableResourcesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SaveAvailableResourcesCommandSignal>();
            Container.BindSignal<SaveAvailableResourcesCommandSignal>().ToMethod<SaveAvailableResourcesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetAvailableLanguagesCommandSignal>();
            Container.BindSignal<GetAvailableLanguagesCommandSignal>().ToMethod<GetAvailableLanguagesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SaveAvailableLanguagesCommandSignal>();
            Container.BindSignal<SaveAvailableLanguagesCommandSignal>().ToMethod<SaveAvailableLanguagesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<HandleApplicationLanguageSignal>();
#if  UNITY_WEBGL && !UNITY_EDITOR
            Container.BindSignal<HandleApplicationLanguageSignal>().ToMethod<HandleBrowserQueryLanguageCommand>(signal => signal.Execute).FromNew();
#endif
            Container.BindSignal<HandleApplicationLanguageSignal>().ToMethod<SetChosenLanguageCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowLoginScreenCommandSignal>();
            Container.BindSignal<ShowLoginScreenCommandSignal>().ToMethod<ShowLoginScreenCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowLoginScreenCommandSignal>().ToMethod<SetUserInfoIfRememberedCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowLoginScreenCommandSignal>().ToMethod<DeleteNotFullyDownloadedAssetsFromCacheCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowLoginScreenCommandSignal>().ToMethod<AccessibilityGrayscaleApplySettingCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowLoginScreenCommandSignal>().ToMethod<AccessibilityFontSizeApplySettingCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<LoginClickCommandSignal>();
            Container.BindSignal<LoginClickCommandSignal>().ToMethod<LoginClickCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetTermTextCommandSignal>();
            Container.BindSignal<GetTermTextCommandSignal>().ToMethod<GetTermTextCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<OpenTermCommandSignal>();
            Container.BindSignal<OpenTermCommandSignal>().ToMethod<OpenTermCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ValidateTermCommandSignal>();
            Container.BindSignal<ValidateTermCommandSignal>().ToMethod<ValidateTermCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AcceptTermCommandSignal>();
            Container.BindSignal<AcceptTermCommandSignal>().ToMethod<AcceptTermCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<LoginTeamsClickCommandSignal>();
            Container.BindSignal<LoginTeamsClickCommandSignal>().ToMethod<LoginTeamsClickCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<LoginFeideClickCommandSignal>();
            Container.BindSignal<LoginFeideClickCommandSignal>().ToMethod<LoginFeideClickCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<LoginSkolonClickCommandSignal>();
            Container.BindSignal<LoginSkolonClickCommandSignal>().ToMethod<LoginSkolonClickCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateUserLoginModelSignal>();
            Container.BindSignal<CreateUserLoginModelSignal>().ToMethod<CreateUserLoginModelCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<OnLoginSuccessCommandSignal>();
            Container.BindSignal<OnLoginSuccessCommandSignal>().ToMethod<GetContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<OnLoginSuccessCommandSignal>().ToMethod<GetMetaDataCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<OnLoginSuccessCommandSignal>().ToMethod<RememberUserCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetUserContentCommandSignal>();
            Container.BindSignal<GetUserContentCommandSignal>().ToMethod<GetUserContentCommand>(signal => signal.Execute).FromNew();
            
            // content creation
            Container.DeclareSignal<CreateContentModelSignal>();
            Container.BindSignal<CreateContentModelSignal>().ToMethod<CreateContentModelCommand>(signal => signal.Execute).FromNew();
            
            // user content creation
            Container.DeclareSignal<CreateUserContentModelCommandSignal>();
            Container.BindSignal<CreateUserContentModelCommandSignal>().ToMethod<CreateUserContentModelCommand>(signal => signal.Execute).FromNew();
            
            // teacher content creation
            Container.DeclareSignal<CreateTeacherContentModelCommandSignal>();
            Container.BindSignal<CreateTeacherContentModelCommandSignal>().ToMethod<CreateTeacherContentModelCommand>(signal => signal.Execute).FromNew();
            
            // teacher content creation
            Container.DeclareSignal<GetTeacherContentCommandSignal>();
            Container.BindSignal<GetTeacherContentCommandSignal>().ToMethod<GetTeacherContentCommand>(signal => signal.Execute).FromNew();
            //generate internal asset link
            
            Container.DeclareSignal<GenerateInternalAssetLinkCommandSignal>();
            Container.BindSignal<GenerateInternalAssetLinkCommandSignal>().ToMethod<GenerateInternalAssetLinkCommand>(signal => signal.Execute).FromNew();
            Container.DeclareSignal<CopyInternalAssetLinkCommandSignal>();
            Container.BindSignal<CopyInternalAssetLinkCommandSignal>().ToMethod<CopyInternalAssetLinkCommand>(signal => signal.Execute).FromNew();

            
            Container.DeclareSignal<CreateMetaDataCommandSignal>();
            Container.BindSignal<CreateMetaDataCommandSignal>().ToMethod<CreateMetaDataCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetActivityContentCommandSignal>();
            Container.BindSignal<GetActivityContentCommandSignal>().ToMethod<GetActivityContentCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<AddToFavouritesCommandSignal>();
            Container.BindSignal<AddToFavouritesCommandSignal>().ToMethod<AddToFavoritesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<RemoveFromFavouritesCommandSignal>();
            Container.BindSignal<RemoveFromFavouritesCommandSignal>().ToMethod<RemoveFromFavoritesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<RemovedFromFavouritesCommandSignal>();
            Container.BindSignal<RemovedFromFavouritesCommandSignal>().ToMethod<RemovedFromFavoritesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SaveAddedFavouriteAssetCommandSignal>();
            Container.BindSignal<SaveAddedFavouriteAssetCommandSignal>().ToMethod<SaveAddedFavoriteAssetCommand>(signal => signal.Execute).FromNew();
            
            // paint
            Container.DeclareSignal<ContentPaint3DStateChangedSignal>();
            // notes
            Container.DeclareSignal<ShowStudentNotesPanelViewSignal>();
            Container.DeclareSignal<AddNoteSpawnSignal>();
            Container.DeclareSignal<GetAllNotesSpawnSignal>();
            
            Container.DeclareSignal<GetAllNotesCommandSignal>();
            Container.BindSignal<GetAllNotesCommandSignal>().ToMethod<GetAllNotesCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetAllNotesResponseCommandSignal>();
            Container.BindSignal<GetAllNotesResponseCommandSignal>().ToMethod<GetAllNotesResponseCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AddNoteCommandSignal>();
            Container.BindSignal<AddNoteCommandSignal>().ToMethod<AddNoteCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<UpdateNoteCommandSignal>();
            Container.BindSignal<UpdateNoteCommandSignal>().ToMethod<UpdateNoteCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<DeleteNoteCommandSignal>();
            Container.BindSignal<DeleteNoteCommandSignal>().ToMethod<DeleteNoteCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AddNoteResponseCommandSignal>();
            Container.BindSignal<AddNoteResponseCommandSignal>().ToMethod<AddNoteResponseCommand>(signal => signal.Execute).FromNew();
            
            // home
            Container.DeclareSignal<ChangePasswordClickViewSignal>();
            Container.BindSignal<ChangePasswordClickViewSignal>().ToMethod<ChangePasswordClickCommand>(signal => signal.Execute).FromNew();
            Container.DeclareSignal<CreateLeftMenuCommandSignal>();
            Container.BindSignal<CreateLeftMenuCommandSignal>().ToMethod<CreateLeftMenuGradesSubjectsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateLeftMenuCommandSignal>().ToMethod<LeftMenuUpdateFontSizeCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowHomeScreenCommandSignal>();
            Container.BindSignal<ShowHomeScreenCommandSignal>().ToMethod<ShowHomeScreenCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowHomeScreenCommandSignal>().ToMethod<ChangeVideoAssetsExtensionsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowLeftMenuCommandSignal>();
            Container.BindSignal<ShowLeftMenuCommandSignal>().ToMethod<ShowLeftMenuCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowRightMenuCommandSignal>();
            Container.BindSignal<ShowRightMenuCommandSignal>().ToMethod<ShowRightMenuCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowLastShownCategoryCommandSignal>();
            Container.BindSignal<ShowLastShownCategoryCommandSignal>().ToMethod<ShowLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetSearchAssetsCommandSignal>();
            Container.BindSignal<GetSearchAssetsCommandSignal>().ToMethod<GetSearchAssetsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ParseSearchResultsCommandSignal>();
            Container.BindSignal<ParseSearchResultsCommandSignal>().ToMethod<ParseSearchResultsCommand>(signal => signal.Execute).FromNew();


            #region New MainScreen
            Container.DeclareSignal<ShowMainScreenAssetsCommandSignal>();
            Container.BindSignal<ShowMainScreenAssetsCommandSignal>().ToMethod<MainScreenCreateAssetsCommand>(signal => signal.Execute).FromNew();


            Container.DeclareSignal<SaveMainScreenContentPanelsCommandSignal>();
            Container.BindSignal<SaveMainScreenContentPanelsCommandSignal>().ToMethod<SaveMainScreenContentPanelsCommand>(signal => signal.Execute).FromNew();


            #endregion



            Container.DeclareSignal<ShowSearchAssetsCommandSignal>();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<RightMenuUnselectAllItemsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<CreateAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowSearchAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowFavouritesCommandSignal>();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<ClearBreadcrumbsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<ClearSelectedCategoriesCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<CreateAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<ActivateHomeTabFavouritesCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowFavouritesCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<ShowRecentlyViewedCommandSignal>();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<ClearBreadcrumbsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<ClearSelectedCategoriesCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<CreateAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<ActivateHomeTabRecentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowRecentlyViewedCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<ShowMyContentViewedCommandSignal>();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<ClearBreadcrumbsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<ClearSelectedCategoriesCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<CreateUserContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<ActivateHomeTabContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ShowMyContentViewedCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<CloseAppCommandSignal>();
            Container.BindSignal<CloseAppCommandSignal>().ToMethod<CloseAppCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SignOutClickCommandSignal>();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<CancelAllDownloadsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<ClearModelsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<ResetPopupOverlayCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<RememberUserCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<ShowLoginScreenCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<CheckLoginAsGuestCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<GradeMenuItemClickCommandSignal>();
            Container.BindSignal<GradeMenuItemClickCommandSignal>().ToMethod<SelectGradeCommand>(signal => signal.Execute).FromNew();
            //Container.BindSignal<GradeMenuItemClickCommandSignal>().ToMethod<SelectSubjectCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SubjectMenuItemClickCommandSignal>();
            // TODO : Only PC
            Container.BindSignal<SubjectMenuItemClickCommandSignal>().ToMethod<SelectGradeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<SubjectMenuItemClickCommandSignal>().ToMethod<SelectSubjectCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<TopicItemClickCommandSignal>();
            Container.BindSignal<TopicItemClickCommandSignal>().ToMethod<SelectTopicCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SubtopicItemClickCommandSignal>();
            Container.BindSignal<SubtopicItemClickCommandSignal>().ToMethod<SelectSubtopicCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AssetItemClickCommandSignal>();
            Container.BindSignal<AssetItemClickCommandSignal>().ToMethod<SelectAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<AssetItemClickCommandSignal>().ToMethod<StartModuleCommand>(signal => signal.Execute).FromNew();


            Container.DeclareSignal<MainScreenAssetItemClickCommandSignal>();
            Container.BindSignal<MainScreenAssetItemClickCommandSignal>().ToMethod<MainScreenSelectAssetCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<StartAssetDetailsCommandSignal>();
            Container.BindSignal<StartAssetDetailsCommandSignal>().ToMethod<StartAssetDetailsCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<DownloadAssetDetailsCommandSignal>();
            Container.BindSignal<DownloadAssetDetailsCommandSignal>().ToMethod<DownloadAssetDetailsCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<CreateAssetDetailsCommandSignal>();
            Container.BindSignal<CreateAssetDetailsCommandSignal>().ToMethod<CreateAssetDetailsCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<ProcessAssetDetailsCommandSignal>();
            Container.BindSignal<ProcessAssetDetailsCommandSignal>().ToMethod<ProcessAssetDetailsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateTopicsContentCommandSignal>();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<CreateTopicsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<CreateAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateTopicsContentCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<CreateSubtopicsContentCommandSignal>();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<CreateSubtopicsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<CreateAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubtopicsContentCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            
            // home -> activities
            Container.DeclareSignal<CreateActivityItemCommandSignal>();
            Container.BindSignal<CreateActivityItemCommandSignal>().ToMethod<CreateMainActivityAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivityItemCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivityItemCommandSignal>().ToMethod<SaveLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateActivitiesScreenWithHeadersCommandSignal>();
            Container.BindSignal<CreateActivitiesScreenWithHeadersCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenWithHeadersCommandSignal>().ToMethod<CreateScreenWithActivityAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenWithHeadersCommandSignal>().ToMethod<ShowCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenWithHeadersCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenWithHeadersCommandSignal>().ToMethod<SaveLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateActivitiesScreenCommandSignal>();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<DeselectChosenActivityCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<SelectActivityItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<CreateScreenWithActivityAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateActivitiesScreenCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateQuizAssetsCommandSignal>();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<SelectActivityItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<SelectActivityQuizAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<CreateQuizAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateQuizAssetsCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreatePuzzleAssetsCommandSignal>();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<SelectActivityItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<SelectActivityPuzzleAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<CreatePuzzleAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreatePuzzleAssetsCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateMultipleQuizAssetsCommandSignal>();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<SelectActivityItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<SelectActivityMultipleQuizAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<CreateMultipleQuizAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultipleQuizAssetsCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateMultiplePuzzleAssetsCommandSignal>();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<SelectActivityItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<SelectActivityMultiplePuzzleAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<CreateMultiplePuzzleAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateMultiplePuzzleAssetsCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateClassificationAssetsCommandSignal>();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<HideCategoryWithHeaderCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<SelectActivityItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<SelectActivityClassificationCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<CreateMultipleClassificationAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateClassificationAssetsCommandSignal>().ToMethod<ClearLastShownActivityCommand>(signal => signal.Execute).FromNew();

            // home tabs
            Container.DeclareSignal<ActivateHomeTabFavouritesCommandSignal>();
            Container.BindSignal<ActivateHomeTabFavouritesCommandSignal>().ToMethod<DeactivateAllHomeTabsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ActivateHomeTabFavouritesCommandSignal>().ToMethod<ActivateHomeTabFavouritesCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<ActivateHomeTabRecentCommandSignal>();
            Container.BindSignal<ActivateHomeTabRecentCommandSignal>().ToMethod<DeactivateAllHomeTabsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ActivateHomeTabRecentCommandSignal>().ToMethod<ActivateHomeTabRecentCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ActivateHomeTabMyContentCommandSignal>();
            Container.BindSignal<ActivateHomeTabMyContentCommandSignal>().ToMethod<DeactivateAllHomeTabsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ActivateHomeTabMyContentCommandSignal>().ToMethod<ActivateHomeTabMyContentCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ActivateHomeTabMyTeacherCommandSignal>();
            Container.BindSignal<ActivateHomeTabMyTeacherCommandSignal>().ToMethod<DeactivateAllHomeTabsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ActivateHomeTabMyTeacherCommandSignal>().ToMethod<ActivateHomeTabMyTeacherCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SetMyTeacherVisibilityCommandSignal>();
            Container.BindSignal<SetMyTeacherVisibilityCommandSignal>().ToMethod<SetMyTeacherVisibilityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<DeactivateHomeTabsCommandSignal>();
            Container.BindSignal<DeactivateHomeTabsCommandSignal>().ToMethod<DeactivateAllHomeTabsCommand>(signal => signal.Execute).FromNew();
            
            // asset item
            Container.DeclareSignal<CreateAssetsCommandSignal>();
            Container.BindSignal<CreateAssetsCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateAssetsCommandSignal>().ToMethod<CreateAssetsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateAssetsCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateAssetsCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateAssetsCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateAssetsCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartDownloadAssetCommandSignal>();
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<StartDownloadAssetCommand>(signal => signal.Execute).FromNew();        
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<ShowProgressSliderOnAssetViewCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<ShowProgressSliderOnQuizAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<ShowProgressSliderOnPuzzleAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<ShowProgressSliderOnMultipleQuizAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<ShowProgressSliderOnMultiplePuzzleAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartDownloadAssetCommandSignal>().ToMethod<ShowProgressSliderOnClassificationAssetCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<UpdateDownloadProgressCommandSignal>();
            Container.BindSignal<UpdateDownloadProgressCommandSignal>().ToMethod<UpdateAssetDownloadProgressCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CancelDownloadProgressCommandSignal>();
            Container.BindSignal<CancelDownloadProgressCommandSignal>().ToMethod<CancelDownloadProgressCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelDownloadProgressCommandSignal>().ToMethod<HideProgressSliderOnAssetItemCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelDownloadProgressCommandSignal>().ToMethod<HideProgressSliderOnQuizAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelDownloadProgressCommandSignal>().ToMethod<HideProgressSliderOnPuzzleAssetCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CancelMultipleDownloadProgressCommandSignal>();
            Container.BindSignal<CancelMultipleDownloadProgressCommandSignal>().ToMethod<CancelMultipleQuizDownloadProgressCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelMultipleDownloadProgressCommandSignal>().ToMethod<CancelMultiplePuzzleDownloadProgressCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelMultipleDownloadProgressCommandSignal>().ToMethod<CancelClassificationDownloadProgressCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelMultipleDownloadProgressCommandSignal>().ToMethod<HideProgressSliderOnMultipleQuizAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelMultipleDownloadProgressCommandSignal>().ToMethod<HideProgressSliderOnMultiplePuzzleAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CancelMultipleDownloadProgressCommandSignal>().ToMethod<HideProgressSliderOnClassificationAssetCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SaveContentPanelsCommandSignal>();
            Container.BindSignal<SaveContentPanelsCommandSignal>().ToMethod<SaveContentPanelsCommand>(signal => signal.Execute).FromNew();
            
            // modules
            Container.DeclareSignal<ShowModuleScreenCommandSignal>();
            Container.BindSignal<ShowModuleScreenCommandSignal>().ToMethod<ShowContent3DModelScreenCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartModuleAssetContentCommandSignal>();
            Container.BindSignal<StartModuleAssetContentCommandSignal>().ToMethod<StartModuleAssetContentCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<MainScreenStartModuleAssetContentCommandSignal>();
            Container.BindSignal<MainScreenStartModuleAssetContentCommandSignal>().ToMethod<MainScreenStartModuleAssetContentCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<StartMultipleModuleAssetContentCommandSignal>();
            Container.BindSignal<StartMultipleModuleAssetContentCommandSignal>().ToMethod<StartMultipleModuleAssetContentCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartImageRecognitionModuleCommandSignal>();
            Container.BindSignal<StartImageRecognitionModuleCommandSignal>().ToMethod<StartImageRecognitionModuleCommand>(signal => signal.Execute).FromNew();
        
            Container.DeclareSignal<StartDrawingToolModuleCommandSignal>();
            Container.BindSignal<StartDrawingToolModuleCommandSignal>().ToMethod<StartDrawingToolModuleCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartUserContentViewerModuleCommandSignal>();
            Container.BindSignal<StartUserContentViewerModuleCommandSignal>().ToMethod<StartUserContentViewerModuleCommand>(signal => signal.Execute).FromNew();

            // recent viewed
            Container.DeclareSignal<AddRecentAssetCommandSignal>();
            Container.BindSignal<AddRecentAssetCommandSignal>().ToMethod<AddRecentAssetCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<UpdateRecentAssetCommandSignal>();
            Container.BindSignal<UpdateRecentAssetCommandSignal>().ToMethod<UpdateRecentAssetCommand>(signal => signal.Execute).FromNew();
            //
            
            Container.DeclareSignal<StartPuzzleCommandSignal>();
            Container.BindSignal<StartPuzzleCommandSignal>().ToMethod<SelectPuzzleAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartPuzzleCommandSignal>().ToMethod<StartModuleCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartQuizCommandSignal>();
            Container.BindSignal<StartQuizCommandSignal>().ToMethod<SelectQuizAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartQuizCommandSignal>().ToMethod<StartModuleCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartMultipleQuizCommandSignal>();
            Container.BindSignal<StartMultipleQuizCommandSignal>().ToMethod<SelectMultipleQuizAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartMultipleQuizCommandSignal>().ToMethod<StartMultipleModuleCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartMultiplePuzzleCommandSignal>();
            Container.BindSignal<StartMultiplePuzzleCommandSignal>().ToMethod<SelectMultiplePuzzleAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartMultiplePuzzleCommandSignal>().ToMethod<StartMultipleModuleCommand>(signal => signal.Execute).FromNew();
            
            // activity -> classification
            Container.DeclareSignal<GetClassificationDetailsCommandSignal>();
            Container.BindSignal<GetClassificationDetailsCommandSignal>().ToMethod<GetClassificationDetailsCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateClassificationDetailsModelCommandSignal>();
            Container.BindSignal<CreateClassificationDetailsModelCommandSignal>().ToMethod<CreateClassificationDetailsModelCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<StartClassificationCommandSignal>();
            Container.BindSignal<StartClassificationCommandSignal>().ToMethod<SelectClassificationAssetCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<StartClassificationCommandSignal>().ToMethod<StartMultipleModuleCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<RunModuleCommandSignal>();
            Container.BindSignal<RunModuleCommandSignal>().ToMethod<RunModuleCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<StartModuleAgainAfterContentDownloadedCommandSignal>();
            Container.BindSignal<StartModuleAgainAfterContentDownloadedCommandSignal>().ToMethod<StartModuleCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<StartMultipleModuleAgainAfterContentDownloadedCommandSignal>();
            Container.BindSignal<StartMultipleModuleAgainAfterContentDownloadedCommandSignal>().ToMethod<StartMultipleModuleCommand>(signal => signal.Execute).FromNew();
             
            // popup overlay
            Container.DeclareSignal<PopupOverlaySignal>();
            Container.DeclareSignal<PopupInputViewSignal>();
            Container.DeclareSignal<PopupInfoViewSignal>();
            Container.DeclareSignal<PopupWarningARViewSignal>();
            Container.DeclareSignal<PopupHotSpotListViewSignal>();
            
            // popup overlay -> internet connection
            Container.DeclareSignal<ShowPopupInternetConnectionCommandSignal>();
            Container.BindSignal<ShowPopupInternetConnectionCommandSignal>().ToMethod<ShowPopupInternetConnectionCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<HidePopupInternetConnectionCommandSignal>();
            Container.BindSignal<HidePopupInternetConnectionCommandSignal>().ToMethod<HidePopupInternetConnectionCommand>(signal => signal.Execute).FromNew();
            
            // feedback
            Container.DeclareSignal<ShowMainFeedbackPanelCommandSignal>();
            Container.BindSignal<ShowMainFeedbackPanelCommandSignal>().ToMethod<ShowMainFeedbackPanelCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ShowSentFeedbackPanelCommandSignal>();
            Container.BindSignal<ShowSentFeedbackPanelCommandSignal>().ToMethod<ShowSentFeedbackPanelCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<HideFeedbackPopupCommandSignal>();
            Container.BindSignal<HideFeedbackPopupCommandSignal>().ToMethod<HideFeedbackPopupCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<SendFeedbackCommandSignal>();
            Container.BindSignal<SendFeedbackCommandSignal>().ToMethod<SendFeedbackCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<FeedbackSentOkResponseCommandSignal>();
            Container.BindSignal<FeedbackSentOkResponseCommandSignal>().ToMethod<HideMainFeedbackPanelCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<FeedbackSentOkResponseCommandSignal>().ToMethod<ShowSentFeedbackPanelCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<FeedbackSentFailResponseCommandSignal>();
            Container.BindSignal<FeedbackSentFailResponseCommandSignal>().ToMethod<HideFeedbackPopupCommand>(signal => signal.Execute).FromNew();
            
            // home -> right menu -> accessibility
            Container.DeclareSignal<AccessibilityFontSizeClickCommandSignal>();
            Container.BindSignal<AccessibilityFontSizeClickCommandSignal>().ToMethod<AccessibilityFontSizeSaveSettingCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<AccessibilityFontSizeClickCommandSignal>().ToMethod<AccessibilityFontSizeApplySettingCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<AccessibilityFontSizeClickCommandSignal>().ToMethod<LeftMenuUpdateFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<AccessibilityFontSizeClickCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AccessibilityFontSizeUpdateScreenCommandSignal>();
            Container.BindSignal<AccessibilityFontSizeUpdateScreenCommandSignal>().ToMethod<AccessibilityFontSizeApplySettingCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AccessibilityTextToAudioClickCommandSignal>();
            Container.BindSignal<AccessibilityTextToAudioClickCommandSignal>().ToMethod<AccessibilityTextToAudioSaveSettingCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AccessibilityGrayscaleClickCommandSignal>();
            Container.BindSignal<AccessibilityGrayscaleClickCommandSignal>().ToMethod<AccessibilityGrayscaleSaveSettingCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<AccessibilityGrayscaleClickCommandSignal>().ToMethod<AccessibilityGrayscaleApplySettingCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AccessibilityLabelLinesClickCommandSignal>();
            Container.BindSignal<AccessibilityLabelLinesClickCommandSignal>().ToMethod<AccessibilityLabelLinesSaveSettingCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ResetAccessibilityCommandSignal>();
            Container.BindSignal<ResetAccessibilityCommandSignal>().ToMethod<ResetAccessibilityCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<AccessibilityTTSPlayCommandSignal>();
            Container.BindSignal<AccessibilityTTSPlayCommandSignal>().ToMethod<AccessibilityTTSPlayCommand>(signal => signal.Execute).FromNew();
        }

        private void PCCommandSignals()
        {
            Container.BindSignal<SignOutClickCommandSignal>().ToMethod<ClearLeftMenuCommand>(signal => signal.Execute).FromNew();
            
            // dropdown multiple activities
            Container.DeclareSignal<CreateDropdownMultipleQuizCommandSignal>();
            Container.BindSignal<CreateDropdownMultipleQuizCommandSignal>().ToMethod<CreateDropdownMultipleQuizCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateDropdownMultiplePuzzleCommandSignal>();
            Container.BindSignal<CreateDropdownMultiplePuzzleCommandSignal>().ToMethod<CreateDropdownMultiplePuzzleCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<DestroyDropdownActivitiesCommandSignal>();
            Container.BindSignal<DestroyDropdownActivitiesCommandSignal>().ToMethod<DestroyDropdownActivitiesCommand>(signal => signal.Execute).FromNew();
            
            // description
            Container.DeclareSignal<GetLabelDescriptionCommandSignal>();
            Container.BindSignal<GetLabelDescriptionCommandSignal>().ToMethod<GetLabelDescriptionCommand>(signal => signal.Execute).FromNew();
            Container.DeclareSignal<GetDescriptionCommandSignal>();
            Container.BindSignal<GetDescriptionCommandSignal>().ToMethod<GetDescriptionCommand>(signal => signal.Execute).FromNew();
            Container.DeclareSignal<GetAudioFileCommandSignal>();
            Container.BindSignal<GetAudioFileCommandSignal>().ToMethod<GetAudioFileCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateDescriptionViewCommandSignal>();
            Container.BindSignal<CreateDescriptionViewCommandSignal>().ToMethod<PauseAllDescriptionsAudioCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateDescriptionViewCommandSignal>().ToMethod<CreateDescriptionViewCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<GetLanguageChangedDescriptionViewCommandSignal>();
            Container.BindSignal<GetLanguageChangedDescriptionViewCommandSignal>().ToMethod<GetLanguageChangedDescriptionViewCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<ChangeLanguageDescriptionViewCommandSignal>();
            Container.BindSignal<ChangeLanguageDescriptionViewCommandSignal>().ToMethod<ChangeLanguageDescriptionViewCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ChangeLanguageDescriptionViewCommandSignal>().ToMethod<PauseAllDescriptionsAudioCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<ChangeLanguageDescriptionViewCommandSignal>().ToMethod<ClearAllDescriptionsAudioCommand>(signal => signal.Execute).FromNew();

            Container.DeclareSignal<UpdateDescriptionFontSizeCommandSignal>();
            Container.BindSignal<UpdateDescriptionFontSizeCommandSignal>().ToMethod<UpdateDescriptionFontSizeCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<LoadAndPlayDescriptionCommandSignal>();
            Container.BindSignal<LoadAndPlayDescriptionCommandSignal>().ToMethod<LoadAndPlayDescriptionCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<PauseAllExceptActiveDescriptionCommandSignal>();
            Container.BindSignal<PauseAllExceptActiveDescriptionCommandSignal>().ToMethod<PauseAllExceptActiveDescriptionCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<PauseAllDescriptionsAudioCommandSignal>();
            Container.BindSignal<PauseAllDescriptionsAudioCommandSignal>().ToMethod<PauseAllDescriptionsAudioCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<RemoveDescriptionFromArrayCommandSignal>();
            Container.BindSignal<RemoveDescriptionFromArrayCommandSignal>().ToMethod<RemoveDescriptionFromArrayCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CloseAllOpenedDescriptionsOnSecondMultiViewCommandSignal>();
            Container.BindSignal<CloseAllOpenedDescriptionsOnSecondMultiViewCommandSignal>().ToMethod<CloseAllOpenedDescriptionsOnSecondMultiViewCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CloseAllOpenedDescriptionsCommandSignal>();
            Container.BindSignal<CloseAllOpenedDescriptionsCommandSignal>().ToMethod<CloseAllOpenedDescriptionsCommand>(signal => signal.Execute).FromNew();
        }

        private void MobileCommandSignals()
        {
            Container.DeclareSignal<CreateGradesContentCommandSignal>();
            Container.BindSignal<CreateGradesContentCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateGradesContentCommandSignal>().ToMethod<CreateGradesCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateGradesContentCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateGradesContentCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateGradesContentCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateThumbnailsForGradesCommandSignal>();
            Container.BindSignal<CreateThumbnailsForGradesCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.CreateThumbnailsForGrades).FromNew();

            //Subject
            Container.DeclareSignal<CreateSubjectsContentCommandSignal>();
            Container.BindSignal<CreateSubjectsContentCommandSignal>().ToMethod<ClearAllContentCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubjectsContentCommandSignal>().ToMethod<CreateSubjectsCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubjectsContentCommandSignal>().ToMethod<RefreshContentGridCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubjectsContentCommandSignal>().ToMethod<UpdateAssetsFontSizeCommand>(signal => signal.Execute).FromNew();
            Container.BindSignal<CreateSubjectsContentCommandSignal>().ToMethod<SaveLastShownCategoryCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateThumbnailsForSubjectsCommandSignal>();
            Container.BindSignal<CreateThumbnailsForSubjectsCommandSignal>().ToMethod<CreateThumbnailsCommand>(signal => signal.CreateThumbnailsForSubjects).FromNew();
            
            // dropdown multiple activities
            Container.DeclareSignal<CreateDropdownMultipleQuizMobileCommandSignal>();
            Container.BindSignal<CreateDropdownMultipleQuizMobileCommandSignal>().ToMethod<CreateDropdownMultipleQuizMobileCommand>(signal => signal.Execute).FromNew();
            
            Container.DeclareSignal<CreateDropdownMultiplePuzzleMobileCommandSignal>();
            Container.BindSignal<CreateDropdownMultiplePuzzleMobileCommandSignal>().ToMethod<CreateDropdownMultiplePuzzleMobileCommand>(signal => signal.Execute).FromNew();
        }
        
    #endregion
    
    #region Managers

    private void InstallManagers()
    {
        Container.Bind<SceneFrameManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ScreenRecorderManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ExternalCallManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<FileManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MemoryManager>().AsSingle().NonLazy();
        Container.Bind<WebGlFileBrowserManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        Container.Bind<IExecutor>().To<CoroutineExecutor>().AsSingle();
    }

    #endregion

        [Serializable]
        public class ScreenPrefabs
        {
            [Header("Main Screens")]
            public GameObject LoginScreenPC;
            public GameObject LoginScreenMobile;
            
            public GameObject HomeScreenPC;
            public GameObject HomeScreenMobile;
            
            [Header("Popups")]
            public GameObject PopupOverlayPC;
            public GameObject PopupOverlayMobile;
            public GameObject PopupInputPC;
            public GameObject PopupInputMobile;
            public GameObject PopupInfoPC;
            public GameObject PopupInfoMobile;
            public GameObject PopupWarningAR;
            public GameObject PopupHotSpotListPC;
            public GameObject PopupHotSpotListMobile;
            public GameObject PopupDescriptionPC;
            public GameObject PopupInternetConnectionPC;
            public GameObject PopupInternetConnectionMobile;

            public GameObject PopupLanguageMobile;
            
            [Header("Others")]
            public GameObject StudentNotesPC;
            public GameObject StudentNotesMobile;
            
            public GameObject VideoViewPC;
            public GameObject VideoViewMobile;
            
            public GameObject ScreenshotViewPC;
            public GameObject ScreenshotViewMobile;
        }
        
        [Serializable]
        public class UIElementPrefabs
        {
            public GameObject GradeMenuItem;
            public GameObject GradeMenuItemMobile;
            
            public GameObject ModulesMenuItem;
            public GameObject SubjectMenuItem;
            
            public GameObject SubjectItemMobile;
            
            public GameObject TopicItem;
            public GameObject TopicItemMobile;
            
            public GameObject SubtopicItem;
            public GameObject SubtopicItemMobile;

            public GameObject AssetItem;
            public GameObject AssetItemMobile;
            public GameObject AssetItemWeb;

            public GameObject UserContentItem;
            public GameObject UserContentItemMobile;
            
            public GameObject FeedbackHomePCPopup;
            
            public GameObject NoteItemPC;
            public GameObject NoteItemMobile;
            
            [Header("PaintCanvas3D")] 
            public GameObject PaintCanvas3D;

            
            [Header("Activity PC")]
            public GameObject ActivityItem;
            public GameObject ActivityQuizzes;
            public GameObject ActivityPuzzles;
            public GameObject ActivityMultipleQuizzes;
            public GameObject ActivityMultiplePuzzles;
            public GameObject ActivityClassification;
            public GameObject QuizAsset;
            public GameObject PuzzleAsset;
            public GameObject MultipleQuizAsset;
            public GameObject MultiplePuzzleAsset;
            public GameObject ClassificationAsset;
            public GameObject DropdownActivityItem;
            
            [Header("Activity Mobile")]
            public GameObject ActivityItemMobile;
            public GameObject ActivityQuizzesMobile;
            public GameObject ActivityPuzzlesMobile;
            public GameObject ActivityMultipleQuizzesMobile;
            public GameObject ActivityMultiplePuzzlesMobile;
            public GameObject ActivityClassificationMobile;
            public GameObject QuizAssetMobile;
            public GameObject PuzzleAssetMobile;
            public GameObject MultipleQuizAssetMobile;
            public GameObject MultiplePuzzleAssetMobile;
            public GameObject ClassificationAssetMobile;
            public GameObject DropdownActivityHeaderMobile;
            public GameObject DropdownActivityContainerMobile;
            public GameObject DropdownActivityItemMobile;
        }

        [Serializable]
        public class DebugSettings
        {
            [SerializeField] private bool log = true;

            public bool Log { get => log; }
        }
    }
}