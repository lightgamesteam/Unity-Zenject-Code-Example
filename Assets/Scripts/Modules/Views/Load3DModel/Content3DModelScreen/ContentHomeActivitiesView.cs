using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentHomeActivitiesView : ViewBase
{
	public Button backButton;

    public GameObject quizPanel;
    public TextMeshProUGUI quizPanelName;
    public TextTooltipEvents quizTemplate;
    
    public GameObject puzzlePanel;
    public TextMeshProUGUI puzzlePanelName;
    public TextTooltipEvents puzzleTemplate;
    
    public GameObject classificationPanel;
    public TextMeshProUGUI classificationPanelName;
    public TextTooltipEvents classificationTemplate;

    public void InitUiComponents()
    {
        backButton = transform.Get<Button>("Back_Button");

        quizPanel = transform.Find("Panels/Quiz_Panel").gameObject;
        quizPanelName = transform.Get<TextMeshProUGUI>("Panels/Quiz_Panel/Panel_Name");
        quizTemplate = transform.Get<TextTooltipEvents>("Panels/Quiz_Panel/Body/Scroll View/Viewport/Content/Template_Item");
        
        puzzlePanel = transform.Find("Panels/Puzzle_Panel").gameObject;
        puzzlePanelName = transform.Get<TextMeshProUGUI>("Panels/Puzzle_Panel/Panel_Name");
        puzzleTemplate = transform.Get<TextTooltipEvents>("Panels/Puzzle_Panel/Body/Scroll View/Viewport/Content/Template_Item");
        
        classificationPanel = transform.Find("Panels/Classification_Panel").gameObject;
        classificationPanelName = transform.Get<TextMeshProUGUI>("Panels/Classification_Panel/Panel_Name");
        classificationTemplate = transform.Get<TextTooltipEvents>("Panels/Classification_Panel/Body/Scroll View/Viewport/Content/Template_Item");
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
