using System;
using Module.Core.UIComponent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorPicker = Module.ColorPicker.ColorPicker;

namespace DrawingTool.Components.ControlElements {
    [Serializable]
    public class ComponentControlElementsView {
        [Header("Content top")]
        [SerializeField] public ComponentButton CloseButton;
        [SerializeField] public ComponentButton SaveButton;
        [SerializeField] public ComponentButton CloudButton;
        [SerializeField] public ComponentButton RecorderButton;
        
        [Header("Content right")]
        [SerializeField] public ComponentButton BrushSizeButton;
        [SerializeField] public ComponentButton TextSizeButton;
        [SerializeField] public ComponentButton ColorButton;
        [SerializeField] public ComponentButton UndoButton;
        
        [Header("Content frame")]
        [SerializeField] public CanvasGroup FrameCanvasGroup;
        
        [Header("Content brush size")]
        [SerializeField] public CanvasGroup BrushSizePanelCanvasGroup;
        [SerializeField] public TextMeshProUGUI BrushSizeTitleText;
        [SerializeField] public ComponentButton BrushSize1Button;
        [SerializeField] public ComponentButton BrushSize2Button;
        [SerializeField] public ComponentButton BrushSize3Button;
        [SerializeField] public ComponentButton BrushSize4Button;
        [SerializeField] public ComponentButton BrushSize5Button;
        
        [Header("Content text size")]
        [SerializeField] public CanvasGroup TextSizePanelCanvasGroup;
        [SerializeField] public TextMeshProUGUI TextSizeTitleText;
        [SerializeField] public ComponentButton TextSize1Button;
        [SerializeField] public ComponentButton TextSize2Button;
        [SerializeField] public ComponentButton TextSize3Button;
        [SerializeField] public ComponentButton TextSize4Button;
        [SerializeField] public ComponentButton TextSize5Button;
        
        [Header("Content color")]
        [SerializeField] public CanvasGroup ColorPanelCanvasGroup;
        [SerializeField] public Image ColorIconImage;
        [SerializeField] public TextMeshProUGUI ColorTitleText;
        [SerializeField] public ColorPicker ColorPicker;
    }
}
