using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDL.Modules.Ultimate.GuiControlElements {
    [Serializable]
    public class MonoControlElementsMobileView : Gui.Core.ViewBase {
        [SerializeField] public CanvasGroup PanelTopCanvasGroup;
        [SerializeField] public CanvasGroup PanelCenterCanvasGroup;
        [SerializeField] public TextMeshProUGUI InformationText;
        [SerializeField] public TextMeshProUGUI LanguagesText;
        [SerializeField] public Button CloseButton;
        [SerializeField] public Button ResetButton;
        [SerializeField] public Toggle FullscreenToggle;
        [SerializeField] public Toggle LanguagesToggle;
        [SerializeField] public Toggle ColorPickerToggle;
        [SerializeField] public Toggle LabelsToggle;
        [SerializeField] public Toggle LayersToggle;
        [Header("Localization")]
        [SerializeField] public TextMeshProUGUI FullscreenHideInterfaceText;
        [SerializeField] public TextMeshProUGUI FullscreenShowInterfaceText;
        [SerializeField] public TextMeshProUGUI ResetText;
        [SerializeField] public TextMeshProUGUI ColorPickerText;
        [SerializeField] public TextMeshProUGUI LabelsText;
        [SerializeField] public TextMeshProUGUI LayersText;
    }
}