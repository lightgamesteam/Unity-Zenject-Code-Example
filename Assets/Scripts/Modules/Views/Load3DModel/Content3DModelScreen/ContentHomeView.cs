using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ContentHomeView : ViewBase
{
	public AudioSource audioSource;
	public Image rayCastBlock;
	public GameObject homePanel;
	
	public Button closeButton;
	
	public Button exploreButton;
	public Button lessonButton;
	public Button explainButton;
	public Button activitiesButton;
	public Button relatedButton;

	public ContentHomeActivitiesView contentHomeActivitiesPanel;
	public ContentHomeRelatedView contentHomeRelatedPanel;
	public ContentHomeLectureModeView contenentHomeLecturePanel;
	
	public override void InitUiComponents()
	{
		rayCastBlock = GetComponent<Image>();
		audioSource = GetComponent<AudioSource>();
		homePanel = transform.Find("Home_Panel").gameObject;
		
		closeButton = transform.Get<Button>("Home_Panel/Close_Button");
	
		exploreButton = transform.Get<Button>("Home_Panel/Radial_Panel/Explore_Button");
		lessonButton = transform.Get<Button>("Home_Panel/Radial_Panel/Lesson_Button");
		explainButton = transform.Get<Button>("Home_Panel/Radial_Panel/Explain_Button");
		activitiesButton = transform.Get<Button>("Home_Panel/Radial_Panel/Activities_Button");
		relatedButton = transform.Get<Button>("Home_Panel/Radial_Panel/Related_Button");

		contentHomeActivitiesPanel = transform.Get<ContentHomeActivitiesView>("X_Activities_Panel");
		contentHomeActivitiesPanel.InitUiComponents();
		
		contentHomeRelatedPanel = transform.Get<ContentHomeRelatedView>("X_Related_Panel");
		contentHomeRelatedPanel.InitUiComponents();

		contenentHomeLecturePanel = transform.Get<ContentHomeLectureModeView>("X_LectureMode_Panel");
		contenentHomeLecturePanel.InitUiComponents();
	}
	
   public class Factory : PlaceholderFactory<ContentHomeView>
   {
   
   }
}
