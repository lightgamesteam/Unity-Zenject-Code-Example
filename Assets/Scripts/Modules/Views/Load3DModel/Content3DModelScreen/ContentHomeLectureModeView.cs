using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentHomeLectureModeView : ViewBase
{
    public GameObject loadingPanel;
    
	public Button homeButton;
    public Button previousButton;
    public Button nextButton;
    public Button playButton;
    public Button pauseButton;

    public TextMeshProUGUI labelName;
    public TextMeshProUGUI description;
    
    public void InitUiComponents()
    {
        loadingPanel = transform.Find("X_Loading_panel").gameObject;
        
        homeButton = transform.Get<Button>("Right_Panel/Home_Button");
        previousButton = transform.Get<Button>("Right_Panel/Previous_Button");
        nextButton = transform.Get<Button>("Right_Panel/Next_Button");
        playButton = transform.Get<Button>("Right_Panel/Play_Button");
        pauseButton = transform.Get<Button>("Right_Panel/Pause_Button");
        
        labelName = transform.Get<TextMeshProUGUI>("Main_Panel/Label_Panel/Label_Text");
        description = transform.Get<TextMeshProUGUI>("Main_Panel/Scroll View/Viewport/Content/Description_Text");
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
