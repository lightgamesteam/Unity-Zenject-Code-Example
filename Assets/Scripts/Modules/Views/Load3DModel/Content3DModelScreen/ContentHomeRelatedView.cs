using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentHomeRelatedView : ViewBase
{
	public Button backButton;

    public GameObject modelRelatedPanel;
    public TextMeshProUGUI modelRelatedPanelName;
    public TextTooltipEvents modelRelatedTemplate;
    
    public GameObject labelRelatedPanel;
    public TextMeshProUGUI labelRelatedPanelName;
    public TextTooltipEvents labelRelatedTemplate;
    
    public void InitUiComponents()
    {
        backButton = transform.Get<Button>("Back_Button");

        modelRelatedPanel = transform.Find("Panels/ModelRelated_Panel").gameObject;
        modelRelatedPanelName = transform.Get<TextMeshProUGUI>("Panels/ModelRelated_Panel/Panel_Name");
        modelRelatedTemplate = transform.Get<TextTooltipEvents>("Panels/ModelRelated_Panel/Body/Scroll View/Viewport/Content/Template_Item");
        
        labelRelatedPanel = transform.Find("Panels/LabelRelated_Panel").gameObject;
        labelRelatedPanelName = transform.Get<TextMeshProUGUI>("Panels/LabelRelated_Panel/Panel_Name");
        labelRelatedTemplate = transform.Get<TextTooltipEvents>("Panels/LabelRelated_Panel/Body/Scroll View/Viewport/Content/Template_Item");
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
