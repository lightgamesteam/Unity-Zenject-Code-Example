using DrawingTool.Components.ControlElements;
using LylekGames;
using Module.Common;
using Module.Core;
using Module.Core.Attributes;
using UnityEngine;
using Zenject;

namespace DrawingTool.States {
    public class StateDrawing : StateControllerBase {
        [Inject] private SceneFrameManager _sceneFrameManager;
        [Inject] private ScreenRecorderManager _screenRecorderManager;
        [Inject] private FileManager _fileManager;
        [Inject] protected readonly SignalBus _signal;
        
        [SerializeField] protected ModuleEntryPoint ModuleEntryPoint;
        [ShowOnly][SerializeField] protected ComponentBackground CBackground;
        [ShowOnly][SerializeField] protected ComponentControlElements CControlElements;

        public override void EnterState() {
            base.EnterState();
            CBackground.ShowComponent();
            CBackground.Controller.SetBackground(_sceneFrameManager.GetFrameOfTexture2dFromCache());
            CControlElements.ShowComponent();
            CControlElements.View.CloseButton.AddListener(() => {
                _screenRecorderManager.StopRecording();
                ModuleEntryPoint.UnloadSceneModule();
            });
            CControlElements.View.SaveButton.AddListener(SaveTextureToFolder);
            CControlElements.View.CloudButton.AddListener(SaveTextureToCloud);
            CControlElements.View.RecorderButton.AddListener(OnRecorder);
            CControlElements.View.BrushSizeButton.AddListener(() => UseBrushToolAndSetBrushSize(Constants.Data.BRUSH_SIZE_1));
            CControlElements.View.BrushSize1Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.BRUSH_SIZE_1));
            CControlElements.View.BrushSize2Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.BRUSH_SIZE_2));
            CControlElements.View.BrushSize3Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.BRUSH_SIZE_3));
            CControlElements.View.BrushSize4Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.BRUSH_SIZE_4));
            CControlElements.View.BrushSize5Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.BRUSH_SIZE_5));
            CControlElements.View.TextSizeButton.AddListener(() => UseTextToolAndSetBrushSize(Constants.Data.TEXT_SIZE_1));
            CControlElements.View.TextSize1Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.TEXT_SIZE_1));
            CControlElements.View.TextSize2Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.TEXT_SIZE_2));
            CControlElements.View.TextSize3Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.TEXT_SIZE_3));
            CControlElements.View.TextSize4Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.TEXT_SIZE_4));
            CControlElements.View.TextSize5Button.AddListener(() => SetBrushSizeAndHidePanel(Constants.Data.TEXT_SIZE_5));
            CControlElements.View.ColorPicker.AddOnSetColorListener(DrawScript.Instance.SetBrushColor);
            CControlElements.View.UndoButton.AddListener(DrawScript.Instance.Undo);
            
            DrawScript.Instance.EventStartDrawing.AddListener(CControlElements.Controller.HideAllPanel);
            DrawScript.Instance.EventStartDrawing.AddListener(CControlElements.HideComponent);
            DrawScript.Instance.EventStopDrawing.AddListener(CControlElements.ShowComponent);
            _signal.Subscribe<VideoRecordingStateSignal>(OnChangeRecordingState);
            //_screenRecorderManager.AddStateListener(OnChangeRecordingState);
            _screenRecorderManager.AddRecordedVideoListener(MoveVideoToFolder);
        }

        public override void ExitState() {
            CBackground.SetStateComponent(false);
            CControlElements.SetStateComponent(false);
            CControlElements.View.CloseButton.RemoveListener(ModuleEntryPoint.UnloadSceneModule);
            DrawScript.Instance.EventStartDrawing.RemoveListener(CControlElements.Controller.HideAllPanel);
            DrawScript.Instance.EventStartDrawing.RemoveListener(CControlElements.HideComponent);
            DrawScript.Instance.EventStopDrawing.RemoveListener(CControlElements.ShowComponent);
            _signal.Unsubscribe<VideoRecordingStateSignal>(OnChangeRecordingState);
            //_screenRecorderManager.RemoveStateListener(OnChangeRecordingState);
            _screenRecorderManager.RemoveRecordedVideoListener(MoveVideoToFolder);
            base.ExitState();
        }

        protected override void Initialize() {
            base.Initialize();
            CBackground = ModuleControllerBase.Instance.Get<ComponentBackground>();
            CControlElements = ModuleControllerBase.Instance.Get<ComponentControlElements>();
        }

        private void SaveTextureToFolder() {
            _fileManager.SaveTextureToMyPictures(_sceneFrameManager.GetFrameOfTexture2d(true));
        }

        private void SaveTextureToCloud() {
        }

        private void UseBrushToolAndSetBrushSize(int value) {
            if (DrawScript.Instance.IsUseTextTool()) {
                DrawScript.Instance.SetBrushSize(value);
            }
            DrawScript.Instance.UseBrushTool();
        }
        
        private void UseTextToolAndSetBrushSize(int value) {
            if (!DrawScript.Instance.IsUseTextTool()) {
                DrawScript.Instance.SetBrushSize(value);
            }
            DrawScript.Instance.UseTextTool();
        }
        
        private void SetBrushSizeAndHidePanel(int value) {
            DrawScript.Instance.SetBrushSize(value);
            CControlElements.Controller.HideAllPanel();
        }

        private void OnRecorder() {
            Debug.Log("OnRecorder called in StateDrawing");
            switch (_screenRecorderManager.GetCurrentState()) {
                case RecordingState.StartRecording:
                    _screenRecorderManager.StopRecording();
                    break;
                case RecordingState.None:
                case RecordingState.StopRecording:
                default:
                    Debug.Log("OnRecorder called Start in StateDrawing");
                    _screenRecorderManager.StartRecording();
                    break;
            }
        }

        private void MoveVideoToFolder(string path) {
            _fileManager.MoveVideoToMyVideos(path);
        }

        private void OnChangeRecordingState(VideoRecordingStateSignal signal) 
        {
            CControlElements.Controller.SetRecordingFrame(signal.State == RecordingState.StartRecording);
        }
    }
}