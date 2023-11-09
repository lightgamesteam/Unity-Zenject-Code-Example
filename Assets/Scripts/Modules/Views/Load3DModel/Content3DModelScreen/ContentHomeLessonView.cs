using UnityEngine.UI;

public class ContentHomeLessonView : ViewBase
{
	public Button backButton;

    public Button lectureModeButton;
    
    public Button interactiveModeButton;

    public ContentHomeLectureModeView contentHomeLectureModeView;

    public void InitUiComponents()
    {
        backButton = transform.Get<Button>("Back_Button");
        lectureModeButton = transform.Get<Button>("Radial_Panel/LectureMode_Button");
        interactiveModeButton = transform.Get<Button>("Radial_Panel/InteractiveMode_Button");

        contentHomeLectureModeView = transform.parent.Get<ContentHomeLectureModeView>("X_LectureMode_Panel");
        contentHomeLectureModeView.InitUiComponents();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
