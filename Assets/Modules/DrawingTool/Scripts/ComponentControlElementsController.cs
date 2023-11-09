using Module.Core;
using Module.Core.Component;
using UnityEngine;

namespace DrawingTool.Components.ControlElements {
    public class ComponentControlElementsController : ComponentControllerBase<ComponentControlElementsView> {
        #region Override
        
        protected override void Initialize() {
            base.Initialize();
            Utilities.Component.SetActiveCanvasGroup(View.FrameCanvasGroup, false);
            HideAllPanel();
            SubscribeListeners();
            View.ColorPicker.SetColor(Color.white);
        }

        protected override void Release() {
            UnsubscribeListeners();
            base.Release();
        }
        
        #endregion

        public void SetRecordingFrame(bool isActive) {
            Utilities.Component.SetActiveCanvasGroup(View.FrameCanvasGroup, isActive);
        }

        public void HideAllPanel() {
            Utilities.Component.SetActiveCanvasGroup(View.BrushSizePanelCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(View.TextSizePanelCanvasGroup, false);
            Utilities.Component.SetActiveCanvasGroup(View.ColorPanelCanvasGroup, false);
        }

        private void SubscribeListeners() {
            View.CloseButton.AddListener(HideAllPanel);
            View.SaveButton.AddListener(HideAllPanel);
            View.CloudButton.AddListener(HideAllPanel);
            View.BrushSizeButton.AddListener(SwitchActiveBrushSizePanel);
            View.TextSizeButton.AddListener(SwitchActiveTextSizePanel);
            View.ColorButton.AddListener(SwitchActiveColorPanel);
            View.UndoButton.AddListener(HideAllPanel);
            View.ColorPicker.AddOnSetColorListener(ChangeColor);
        }
        
        private void UnsubscribeListeners() {
            View.CloseButton.RemoveListener(HideAllPanel);
            View.SaveButton.RemoveListener(HideAllPanel);
            View.CloudButton.RemoveListener(HideAllPanel);
            View.BrushSizeButton.RemoveListener(SwitchActiveBrushSizePanel);
            View.TextSizeButton.RemoveListener(SwitchActiveTextSizePanel);
            View.ColorButton.RemoveListener(SwitchActiveColorPanel);
            View.UndoButton.RemoveListener(HideAllPanel);
            View.ColorPicker.RemoveOnSetColorListener(ChangeColor);
        }

        private void SwitchActiveBrushSizePanel() {
            if (Utilities.Component.IsActiveCanvasGroup(View.BrushSizePanelCanvasGroup)) {
                HideAllPanel();
            } else {
                HideAllPanel();
                Utilities.Component.SetActiveCanvasGroup(View.BrushSizePanelCanvasGroup, true);
            }
        }
        
        private void SwitchActiveTextSizePanel() {
            if (Utilities.Component.IsActiveCanvasGroup(View.TextSizePanelCanvasGroup)) {
                HideAllPanel();
            } else {
                HideAllPanel();
                Utilities.Component.SetActiveCanvasGroup(View.TextSizePanelCanvasGroup, true);
            }
        }
        
        private void SwitchActiveColorPanel() {
            if (Utilities.Component.IsActiveCanvasGroup(View.ColorPanelCanvasGroup)) {
                HideAllPanel();
            } else {
                HideAllPanel();
                Utilities.Component.SetActiveCanvasGroup(View.ColorPanelCanvasGroup, true);
            }
        }

        private void ChangeColor(Color color) {
            View.ColorIconImage.color = color;
        }
    }
}