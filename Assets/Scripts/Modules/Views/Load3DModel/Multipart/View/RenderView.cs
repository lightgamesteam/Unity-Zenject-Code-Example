using TDL.Modules.Model3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RenderView : ViewBase
{
    public MultiView MultiView;
    public Camera RenderCamera;
    public Light Light;
    public SmoothOrbitCam SmoothOrbitCam;
    public Button ZoomPlusButton;
    public Button ZoomMinusButton;
    public Button ResetButton;
    public ColorPicker ColorPicker;
    public TextMeshProUGUI ScreenName;
    public Button CloseScreen;
    
    [Header("Multimodel")]
    public DropdownToggle LabelsListDropdownToggle;
    public DropdownToggle LanguageListDropdownToggle;
    public Button MultiModelPartButton;
    public Button StudentDescriptionButton;
    
    [Header("Multimodel Part Panel")]
    public Transform MultiModelPartPanel;

    public TextMeshProUGUI MultiModelPartSelectedText;
    public Button MultiModelPartBackButton;
    public Button MultiModelPartApplyButton;
    public Button MultiModelPartClearButton;
    
    public override void InitUiComponents()
    {

    }

    public void SetActiveUI(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public int GetRenderLayerIndex()
    {
        if (MultiView)
            return MultiView.renderLayerIndex;

        return Module3DModelConstants.MultiViewMainLayer;
    }
}
