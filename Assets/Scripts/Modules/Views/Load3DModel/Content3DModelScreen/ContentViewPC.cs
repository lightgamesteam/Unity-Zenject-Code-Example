using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentViewPC : ContentViewBase
{
    [Header("Content -> CanvasGroup")]
    [SerializeField] public CanvasGroup ContentMainCanvasGroup;
    [SerializeField] public CanvasGroup ContentSearchModelCanvasGroup;
    
    [Header("Multipart Panel")]
    public Button multipartButton;

    public GameObject multipartPanel;
    public GameObject multipartCameraViewPanel;
    public Button multipartCloseButton;
    //public TooltipPanel toolTip;
    public TextMeshProUGUI selectedPartText;
    
    public GameObject showMultipartPanel;
    public Button showMultipartCloseButton;
    
    public Button clearAllSelectedPart;
    public Button startViewSelectedPart;

    [Header("Multimodel Panel")]
    public PopupProgress popupProgressFactory;
    public RenderView multimodelRenderView;
    //public TMP_InputField multimodelSearch;

    [Header("Paint 3D")] 
    public PaintView paintView;

    public void GetReferenceUI()
    {
        //AR
        _placeModelAR = transform.Get<ToggleAR>("Content.Main/Panel_Bot/Panel_BotLeft/tgl_ARPlaceModel");
        _placeModelAR_2 = transform.Get<ToggleAR>("Content.Main/Panel_Bot/Panel_BotLeft/tgl_ARPlaceModel_2");
        
        // ---
        popupProgressFactory = transform.Get<PopupProgress>("Overlay/X_PopupProgress");
        helpVideoRecordingButton = transform.Get<Button>("Content.Main/Panel_Top/Panel_TopLeft/Button_Help");
        
        // Top Left
        returnButton = transform.Get<Button>("Content.Main/Panel_Top/Panel_TopLeft/Button_Return");
        recorderToggle = transform.Get<Toggle>("Content.Main/Panel_Top/Panel_TopLeft/Toggle_Recorder");
        screenshotButton = transform.Get<Button>("Content.Main/Panel_Top/Panel_TopLeft/Button_ScreenshotTool");
        //screenshotView = transform.Get<ScreenshotView>("Overlay/X_ScreenshotView");

        // Top Right
        topRightMenuPanel = transform.Find("Content.Main/Panel_Top/Panel_TopRight").gameObject;
        rightMenuToggle  = transform.Get<Toggle>("Content.Main/Panel_Top/Panel_TopRight/Toggle_RightMenu");
        homeButton = transform.Get<Button>("Content.Main/Panel_Top/Panel_TopRight/Button_Home");
        
        // Search
        var searchPanel = transform.Get<Transform>("Content.SearchModel/X_SearchModel_Panel/Panel");
        SearchGoButton = searchPanel.Get<Button>("SearchPanel/btn_SearchModel_Go");
        NoSearchResultsFound = searchPanel.Get<TextMeshProUGUI>("NoSearchResultsPanel/NoSearchResultsText");
            
        //Multipart
        multipartPanel = transform.Find("Content.Main/X_MultipartPanel_Top").gameObject;
        multipartButton = transform.Get<Button>("Content.Main/Panel_Right/Button_Multipart");
        multipartCloseButton = transform.Get<Button>("Content.Main/X_MultipartPanel_Top/Panel_TopLeft/btn_MultipartBack");

        multipartCameraViewPanel = model3DCamera.transform.Find("Canvas/X_Multipart_Panel").gameObject;
        
        //toolTip = transform.Get<TooltipPanel>("Overlay/Panel_ToolTip/X_ToolTip_Image");
        selectedPartText = transform.Get<TextMeshProUGUI>("Content.Main/X_MultipartPanel_Top/Panel_Middle/SelectedPart Text");
        clearAllSelectedPart = transform.Get<Button>("Content.Main/X_MultipartPanel_Top/Panel_Middle/btn_ClearMultipart");
        startViewSelectedPart = transform.Get<Button>("Content.Main/X_MultipartPanel_Top/Panel_Middle/btn_ApplyMultipart");

        showMultipartPanel = transform.Find("Content.Main/X_MultipartPanel_ShowPart").gameObject;
        showMultipartCloseButton = transform.Get<Button>("Content.Main/X_MultipartPanel_ShowPart/Panel_TopLeft/btn_BackToSelectPart");

        //Multimodel
        multimodelRenderView = model3DCamera.transform.Get<RenderView>("Canvas/X_Multimodel_Panel");
        //multimodelSearch = transform.Get<TMP_InputField>("Content.SearchModel/X_SearchModel_Panel/Panel/SearchPanel/SearchInputField");
        multimodelSearchButton = transform.Get<DropdownToggle>("Content.Main/Panel_Right/Button_Multimodel");
        
        //Animation
        playAnimationButton = transform.Get<Button>("Content.Main/Panel_Bot/Panel_BotLeft/btn_PlayAnimation");
        pauseAnimationButton = transform.Get<Button>("Content.Main/Panel_Bot/Panel_BotLeft/btn_PauseAnimation");
        
        //Student Note
        studentNoteButton = transform.Get<Button>("Content.Main/Panel_Bot/Panel_BotRight/panel_Main/btn_Note");
        
        // Paint 3D
        paintView = transform.Get<PaintView>("Content.Main/Panel_Right/PaintView");

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
    }
}
