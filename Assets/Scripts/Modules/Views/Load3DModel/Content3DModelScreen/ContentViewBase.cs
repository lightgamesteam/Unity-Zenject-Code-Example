using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;
using Zenject;

#if UNITY_IOS
using System.Collections;
using UnityEngine.Apple.ReplayKit;
#endif

public class ContentViewBase : ViewBase
{
	[Header("Screenshot / Video Capture")]
	public Toggle recorderToggle;
	public Button screenshotButton;

	//public ScreenshotView screenshotView;
	
	[Header("Camera")]
	public Camera model3DCamera;
	public SmoothOrbitCam smoothOrbitCam;
	public Transform cameraOrigin;
	public RawImage backgroundRawImage;
	public GameObject webCamera;

    [Inject] public Camera3DModelSettings cameraSettings;
    [Inject] public Load3DModelContainer load3DModelContainer;
    [Inject] private ExternalCallManager _externalCallManager;

    [Header("Top Left Menu")]
	public GameObject topLeftMenuPanel;
	public Button backButton;
	public Button returnButton;
	public Toggle fullscreenToggle;
	public Button helpVideoRecordingButton;

	[Header("Top Right Menu")]
	public GameObject topRightMenuPanel;
	public Toggle rightMenuToggle;
	public Button homeButton;

	[Header("Right Menu")] 
	public GameObject rightMenuPanel;
	public DropdownToggle labelsListDropdownToggle;
	public TextMeshProUGUI LabelsListDropdownSelectAllText { get; set; }
	public DropdownToggle languagesDropdownToggle;
	public DropdownToggle relatedContentsDropdownToggle;
	public Button DescriptionButton { get; set; }

	[Header("Bottom Left Menu")] 
	public GameObject bottomLeftMenu;
	public Button zoomPlusButton;
	public Button zoomMinusButton;
	public Toggle arCameraToggle;
	public Toggle colorPikerToggle;
	//public TextMeshProUGUI accessibilityText;
	public Button playAnimationButton;
	public Button pauseAnimationButton;
    public ColorPicker colorPicker;
    public ColorPickerControl colorPickerControl;
	public Button resetButton;

	[Header("Bottom Right Menu")]
	public TextMeshProUGUI modelNameLabel;
	
	// Student Note
	public Button studentNoteButton;
	
	// feedback
	public TextMeshProUGUI FeedbackTitle { get; set; }
	public TextMeshProUGUI FeedbackSendText { get; set; }
	public TextMeshProUGUI FeedbackCancelText { get; set; }
	public TextMeshProUGUI FeedbackPlaceholderText { get; set; }
	public TextMeshProUGUI FeedbackSentOkText { get; set; }
    
	private RectTransform _mainFeedbackPanel;
	private RectTransform _sendingFeedbackPanel;
	private RectTransform _sentFeedbackPanel;
	private Toggle _feedbackToggle;
	private TMP_InputField _feedbackMessage;
	private Button _feedbackSendButton;
	private Button _feedbackCancelButton;
	private Button _feedbackSentOkButton;
	
	// Search
	public Button SearchGoButton { get; set; }
	public TextMeshProUGUI NoSearchResultsFound { get; set; }
	
	// Multimodel Panel
	public DropdownToggle multimodelSearchButton;
	
	[Header("AR")]
	public ToggleAR _placeModelAR;
	public ToggleAR _placeModelAR_2;

	private void Awake()
	{
		GetReference();
	}

	public virtual void GetReference()
	{
		model3DCamera = gameObject.GetInScene<Camera>("3DModelCamera");
		backgroundRawImage = model3DCamera.transform.Get<RawImage>("Canvas/X_BackgroundImage");
		webCamera =  model3DCamera.transform.Find("Canvas/X_WebCamera").gameObject;
		cameraOrigin = model3DCamera.transform.parent;
		smoothOrbitCam = cameraOrigin.GetComponent<SmoothOrbitCam>();
		
		topLeftMenuPanel = transform.Find("Content.Main/Panel_Top/Panel_TopLeft").gameObject;
		backButton = transform.Get<Button>("Content.Main/Panel_Top/Panel_TopLeft/btn_Back");
		fullscreenToggle = transform.Get<Toggle>("Content.Main/Panel_Top/Panel_TopLeft/tgl_Fullscreen");

		rightMenuPanel = transform.Find("Content.Main/Panel_Right").gameObject;
		
		labelsListDropdownToggle = transform.Get<DropdownToggle>("Content.Main/Panel_Right/Toggle_LabelList");
		LabelsListDropdownSelectAllText = labelsListDropdownToggle.Template.GetComponentInChildren<TextMeshProUGUI>();
		languagesDropdownToggle = transform.Get<DropdownToggle>("Content.Main/Panel_Right/Toggle_Languages");
		relatedContentsDropdownToggle  = transform.Get<DropdownToggle>("Content.Main/Panel_Right/Toggle_RelatedContents");
        //accessibilityText = transform.Get<TextMeshProUGUI>("Content.Main/Panel_Right/Toggle_Accessibility/X_Panel_Accessibility/Label_bg/Text");
        DescriptionButton = rightMenuPanel.Get<Button>("Button_Description");

        bottomLeftMenu = transform.Find("Content.Main/Panel_Bot/Panel_BotLeft").gameObject;
		zoomPlusButton = transform.Get<Button>("Content.Main/Panel_Bot/Panel_BotLeft/btn_ZoomPlus");
		zoomMinusButton = transform.Get<Button>("Content.Main/Panel_Bot/Panel_BotLeft/btn_ZoomMinus");
		arCameraToggle = transform.Get<Toggle>("Content.Main/Panel_Bot/Panel_BotLeft/tgl_ARcamera");
		arCameraToggle.gameObject.SetActive(!_externalCallManager.IsTeams);
		colorPikerToggle = transform.Get<Toggle>("Content.Main/Panel_Bot/Panel_BotLeft/tgl_ColorPiker");
		resetButton = transform.Get<Button>("Content.Main/Panel_Bot/Panel_BotLeft/btn_Reset");
		
		modelNameLabel = transform.Get<TextMeshProUGUI>("Content.Main/Panel_Bot/Panel_BotRight/panel_Main/Text");
		SetLabel("");
		
		colorPicker = transform.Get<ColorPicker>("Content.Main/Panel_Bot/Panel_BotLeft/X_UIColorPicker");
		
		// feedback main panel
		var panelBotRight = transform.Get<RectTransform>("Content.Main/Panel_Bot/Panel_BotRight");
		_feedbackToggle = panelBotRight.Get<Toggle>("panel_Main/tgl_Feedback");
		_mainFeedbackPanel = panelBotRight.Get<RectTransform>("panel_Main_Feedback");
		_feedbackMessage = _mainFeedbackPanel.Get<TMP_InputField>("Content_Panel/InputField");
		_feedbackSendButton = _mainFeedbackPanel.Get<Button>("Bottom_Panel/SendButton");
		_feedbackCancelButton = _mainFeedbackPanel.Get<Button>("Bottom_Panel/CancelButton");
		FeedbackTitle = _mainFeedbackPanel.Get<TextMeshProUGUI>("Title");
		FeedbackSendText = _feedbackSendButton.GetComponentInChildren<TextMeshProUGUI>();
		FeedbackCancelText = _feedbackCancelButton.GetComponentInChildren<TextMeshProUGUI>();
		FeedbackPlaceholderText = _feedbackMessage.placeholder.GetComponent<TextMeshProUGUI>();
        
		// feedback sent panel
		_sendingFeedbackPanel = panelBotRight.Get<RectTransform>("panel_Sending_Feedback");
		_sentFeedbackPanel = panelBotRight.Get<RectTransform>("panel_Sent_Feedback");
		_feedbackSentOkButton = _sentFeedbackPanel.GetComponentInChildren<Button>();
		FeedbackSentOkText = _sentFeedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
	}

	public virtual void SubscribeOnListeners()
	{
		Signal.Fire<SubscribeOnContentViewSignal>();

		// Change background color
		colorPicker.ActivationChanged.AddListener(OnColorPickerActivationChanged);
		colorPicker.onValueChanged.AddListener(OnColorPickerChanged);
		colorPicker.gameObject.SetActive(false);

		// Toggle Web Camera
		arCameraToggle.onValueChanged.AddListener((value) => { webCamera.SetActive(value); });

		// Camera zoom
		zoomPlusButton.onClick.AddListener(() => smoothOrbitCam.Zoom(0.5f));
		zoomMinusButton.onClick.AddListener(() => smoothOrbitCam.Zoom(-0.5f));
		
        fullscreenToggle.onValueChanged.AddListener(SetFullScreenMode);
        
        //Label Line
        //labelLine.onValueChanged.AddListener(value => SetIsEnableLineRenderer(value));

        if (DeviceInfo.GetUI() == DeviceUI.PC)
        {
	        SubscribeOnFeedback();
        }
	}

	private void SubscribeOnFeedback()
	{
		_feedbackToggle.onValueChanged.AddListener(OnFeedbackToggleChanged);
		_feedbackMessage.onValueChanged.AddListener(SetSendButtonStatus);
		_feedbackSendButton.onClick.AddListener(OnFeedbackSendClick);
		_feedbackCancelButton.onClick.AddListener(OnFeedbackCancelClick);
		_feedbackSentOkButton.onClick.AddListener(OnFeedbackSentOkClick);
	}
	
	private void UnsubscribeFromFeedback()
	{
		if (_feedbackToggle.isOn)
		{
			CloseFeedbackWindow();
		}
		
		_feedbackToggle.onValueChanged.RemoveAllListeners();
		_feedbackMessage.onValueChanged.RemoveAllListeners();
		_feedbackSendButton.onClick.RemoveAllListeners();
		_feedbackCancelButton.onClick.RemoveAllListeners();
		_feedbackSentOkButton.onClick.RemoveAllListeners();
	}

	public void CloseView()
	{
		if (DeviceInfo.GetUI() == DeviceUI.PC)
		{
			UnsubscribeFromFeedback();
		}
        
		Signal.Fire<UnsubscribeFromContentViewSignal>();
		
		SetLabel("");

		if (colorPicker != null)
		{
			colorPicker.ResetColor(Color.white);
		}
		
		arCameraToggle.isOn = false;
		colorPikerToggle.isOn = false;
        pauseAnimationButton.gameObject.SetActive(false);
        playAnimationButton.gameObject.SetActive(false);
		fullscreenToggle.isOn = false;
		gameObject.SetActive(false);
	}

	public void ClearView(Transform tr)
	{
		SetLabel("");
		
		if (colorPicker != null)
		{
			colorPicker.ResetColor(Color.white);
		}
		
		arCameraToggle.isOn = false;
		colorPikerToggle.isOn = false;
		fullscreenToggle.isOn = true;

		languagesDropdownToggle.DropdownListToggle.isOn = false;
		labelsListDropdownToggle.DropdownListToggle.isOn = false;
		relatedContentsDropdownToggle.DropdownListToggle.isOn = false;
		
		tr.localRotation = Quaternion.identity;
		
		foreach (Transform t in tr)
		{
			if (t.name != "3DLabel")
			{
				Destroy(t.gameObject);
			}
			else
			{
				foreach (Transform trLabel in t)
				{
					trLabel.localRotation = Quaternion.identity;
					Destroy(trLabel.gameObject);
				}
			}
		}
	}

	public void SetLabel(string str)
	{
		modelNameLabel.text = str;
	}
	
	private void SetFullScreenMode(bool isFullScreen)
	{
		if (DeviceInfo.GetUI() == DeviceUI.Mobile)
		{
			rightMenuPanel.SetActive(isFullScreen);
		}
		else
		{
			rightMenuPanel.SetActive(isFullScreen && rightMenuToggle.isOn);
			topRightMenuPanel.SetActive(isFullScreen);
		}
		
		bottomLeftMenu.SetActive(isFullScreen);
	}
	
//	public void SetIsEnableLineRenderer(bool value)
//	{
//		cameraOrigin.gameObject.GetAllInSceneOnLayer<LabelLine>().ForEach(l =>
//		{
//			l.SetLabelLineRendererActive(value);
//			l.SetLabelLineGameObjectActive(value);
//		});
//	}
		
	#region FeedbackAndNote
	
	public void SetStudentNoteVisibility(bool status)
	{
		studentNoteButton.gameObject.SetActive(status);
	}

	public void SetFeedbackAvailability(bool status)
	{
		_feedbackToggle.gameObject.SetActive(status);
	}
    
	public void CloseFeedbackWindow()
	{
		_feedbackMessage.text = string.Empty;
		_feedbackSendButton.interactable = false;
		_feedbackToggle.isOn = false;
	}
    
	public void ShowMainFeedbackPanel(bool status)
	{
		_mainFeedbackPanel.gameObject.SetActive(status);
	}
    
	public void ShowSentFeedbackPanel(bool status)
	{
		_sendingFeedbackPanel.gameObject.SetActive(false);        
		_sentFeedbackPanel.gameObject.SetActive(status);        
	}

	private void OnFeedbackToggleChanged(bool isOpened)
	{
		smoothOrbitCam.BlockUserKeyInput(isOpened);

		if (isOpened)
		{
			Signal.Fire<ShowMainFeedbackPanelViewSignal>();
		}
		else
		{
			Signal.Fire<CancelFeedbackViewSignal>();
		}
	}
    
	private void SetSendButtonStatus(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			if (_feedbackSendButton.interactable)
			{
				_feedbackSendButton.interactable = false;
			}
            
			return;
		}
        
		if (string.IsNullOrEmpty(value) && _feedbackSendButton.interactable)
		{
			_feedbackSendButton.interactable = false;
		}
		else if (!string.IsNullOrEmpty(value) && !_feedbackSendButton.interactable)
		{
			_feedbackSendButton.interactable = true;
		}
	}

	private void OnFeedbackSendClick()
	{
		_sendingFeedbackPanel.gameObject.SetActive(true);        
		Signal.Fire(new SendFeedbackViewSignal(string.Empty, _feedbackMessage.text));
	}

	private void OnFeedbackCancelClick()
	{
		Signal.Fire<CancelFeedbackViewSignal>();
	}
    
	private void OnFeedbackSentOkClick()
	{
		Signal.Fire<FeedbackSentOkClickViewSignal>();
	}

	#endregion
	
	#region Color Picker
	
	private void OnColorPickerActivationChanged(bool isActivated)
	{
		if (!isActivated)
		{
			model3DCamera.backgroundColor = colorPicker.awakeColor;
		}
		
		backgroundRawImage.gameObject.SetActive(!isActivated);
	}

	private void OnColorPickerChanged(Color color)
	{
		model3DCamera.backgroundColor = color;
	}
	
	#endregion
	
	#region iOS recorder

	#if UNITY_IOS

	private bool _isReadyForPreview = true;
	private bool _isRecording;
	
	private void Update()
	{
		if (_isReadyForPreview && ReplayKit.recordingAvailable)
		{
			_isReadyForPreview = false;
			_isRecording = false;
			
			ReplayKit.Preview();
			StartCoroutine(DiscardVideo());
		}

		if (!_isRecording && ReplayKit.isRecording)
		{
			Signal.Fire(new VideoRecordingStateSignal(RecordingState.StartRecording));
			_isRecording = true;
		}
	}

	private IEnumerator DiscardVideo()
	{
		yield return new WaitForSeconds(0.5f);
        
		_isReadyForPreview = true;
		ReplayKit.Discard();
	}

	#endif
    
	#endregion
	
	public class Factory : PlaceholderFactory<ContentViewBase>
	{

	}
}