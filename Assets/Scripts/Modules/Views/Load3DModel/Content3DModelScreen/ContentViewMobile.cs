using System.Collections.Generic;
using TDL.Modules.Model3D.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public class ContentViewMobile : ContentViewBase 
{
	private GameObject uiPanel;

	public List<TextMeshProUGUI> labelsListText = new List<TextMeshProUGUI>();
	public GameObject screenLabel;

	public Button relatedContentButton;
	public CanvasPanel relatedCanvasPanel;
	
	public Button languageButton;
	public CanvasPanel languageCanvasPanel;
	public ExecutionEvents executionEvents;
	public Button cancelLoading;
	
	private void Awake()
	{
		GetReference();
	}

	public override void GetReference()
	{
		//AR
		_placeModelAR = transform.Get<ToggleAR>("UI/Panel_Bot/Toggle_ARPlaceModel");
		_placeModelAR_2 = transform.Get<ToggleAR>("UI/Panel_Bot/Toggle_ARPlaceModel_2");
		
		// Top Left
		returnButton = transform.Get<Button>("UI/Panel_Top/Button_Return");
		recorderToggle = transform.Get<Toggle>("tgl_VideoRecording");
		//screenshotButton = transform.Get<Button>("Content.Main/Panel_Top/Panel_TopLeft/Button_ScreenshotTool");
		//screenshotView = transform.Get<ScreenshotView>("Overlay/X_ScreenshotView");
		
		model3DCamera = gameObject.GetInScene<Camera>("3DModelCamera");
		cameraOrigin = model3DCamera.transform.parent;
		smoothOrbitCam = cameraOrigin.GetComponent<SmoothOrbitCam>();

		webCamera = model3DCamera.transform.Find("Canvas/X_WebCamera").gameObject;
		backgroundRawImage = model3DCamera.transform.Get<RawImage>("Canvas/X_BackgroundImage");
		executionEvents = gameObject.GetComponent<ExecutionEvents>();

		//UI
		uiPanel = transform.Find("UI").gameObject;
		screenLabel = transform.Find("Screen_Label").gameObject;

		backButton = transform.Get<Button>("UI/Panel_Top/btn_Back");
		fullscreenToggle = transform.Get<Toggle>("tgl_Fullscreen");

		labelsListDropdownToggle = transform.Get<DropdownToggle>("UI/Panel_Bot/Toggle_LabelList");
		
		// Search
		multimodelSearchButton = transform.Get<DropdownToggle>("UI/Panel_Bot/Toggle_Multimodel");
		SearchGoButton = transform.Get<Button>("UI/X_Multimodel_Panel/Bot_Panel/Button_Find");
		NoSearchResultsFound = transform.Get<TextMeshProUGUI>("UI/X_Multimodel_Panel/Scroll View/NoSearchResultsPanel/X_NoSearchResultsText");

		relatedContentButton = transform.Get<Button>("UI/Panel_Top/Button_RelatedContent");
		languageButton = transform.Get<Button>("UI/Panel_Top/Button_Language");
		//accessibilityText = transform.Get<TextMeshProUGUI>("UI/Panel_Bot/Toggle_Accessibility/Text");

		arCameraToggle = transform.Get<Toggle>("UI/Panel_Bot/tgl_ARcamera");
		colorPikerToggle = transform.Get<Toggle>("UI/Panel_Bot/tgl_ColorPiker");
		playAnimationButton = transform.Get<Button>("UI/Panel_Bot/btn_PlayAnimation");
        pauseAnimationButton = transform.Get<Button>("UI/Panel_Bot/btn_PauseAnimation");
		resetButton = transform.Get<Button>("UI/Panel_Bot/btn_Reset");
		cancelLoading = transform.Get<Button>("UI/Panel_Bot/btn_CancelLoading");

		labelsListText.Add(transform.Get<TextMeshProUGUI>("UI/Panel_Bot/Toggle_LabelList/Text"));
		labelsListText.Add(transform.Get<TextMeshProUGUI>("UI/Label_Panel/Bot_Panel/Text"));

		modelNameLabel = transform.Get<TextMeshProUGUI>("UI/Panel_Top/ModelName_Text");
		SetLabel("");

		colorPickerControl = transform.Get<ColorPickerControl>("UI/UIColorPicker/ColorPicker 2.0");

		// student notes
		studentNoteButton = transform.Get<Button>("UI/Panel_Top/btn_Notes");
	}

	public override void SubscribeOnListeners()
	{
		// Change background color
		colorPickerControl.onValueChanged.AddListener(OnColorPickerChanged);
		colorPickerControl.transform.parent.GetComponentInChildren<Toggle>().onValueChanged.AddListener(OnColorPickerActivationChanged);
		
        // Play animation
        playAnimationButton.onClick.AddListener(() => {
            playAnimationButton.gameObject.SetActive(false);
            pauseAnimationButton.gameObject.SetActive(true);
            var model = load3DModelContainer.transform.Find("model");
            var animator = model?.GetComponent<Animator>();
            if (animator)
            {
                animator.StopPlayback();
            }
        });

        // Pause animation
        pauseAnimationButton.onClick.AddListener(() => {
            playAnimationButton.gameObject.SetActive(true);
            pauseAnimationButton.gameObject.SetActive(false);
            var model = load3DModelContainer.transform.Find("model");
            var animator = model?.GetComponent<Animator>();
            if (animator)
            {
                animator.StartPlayback();
            }
        });

		//backButton.onClick.AddListener(CloseView);

		fullscreenToggle.onValueChanged.AddListener(SetFullScreenMode);
		
		//Label Line
		//labelLine.onValueChanged.AddListener(SetIsEnableLineRendererMobile);
	}

//	public void SetIsEnableLineRendererMobile(bool value)
//	{
//		cameraOrigin.gameObject.GetAllInSceneOnLayer<LabelLine>().ForEach(l =>
//		{
//			l.SetLabelLineRendererActive(value);
//			l.SetLabelLineGameObjectActive(value);
//		});
//	}

	private void SetFullScreenMode(bool value)
	{
		uiPanel.SetActive(value);
	}
	
	#region Color Picker
	
	private void OnColorPickerActivationChanged(bool isActivated)
	{
		backgroundRawImage.gameObject.SetActive(!isActivated);
	}

	private void OnColorPickerChanged(Color color)
	{
		model3DCamera.backgroundColor = color;
	}
	
	#endregion
}
