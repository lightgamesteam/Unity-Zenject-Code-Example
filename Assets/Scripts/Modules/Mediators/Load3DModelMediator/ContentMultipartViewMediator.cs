using System;
using System.Collections.Generic;
using DG.Tweening;
using TDL.Constants;
using TDL.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class ContentMultipartViewMediator  : ContentViewMediatorBase, IInitializable, ITickable, IDisposable
    {
        [Inject] private ContentViewMediator _contentViewMediator;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private Load3DModelContainer _load3DModelContainer;
        [Inject] private ContentViewModel _contentViewModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;
        [Inject] private Camera3DModelSettings cameraSettings;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        [Inject] private MultiView.Factory  _multiViewFactory;
        private List<MultiView>  _multiView;

        public ContentViewPC contentViewPC;

        private float _transitionDuration = 0.5f;
        private int _maxSelectedPart = 3;
        private ContentMultipartState _contentMultipartState = ContentMultipartState.None;
        
        public List<string> selectedPart = new List<string>();
        private const int startRenderLayerIndex = 10;

        public void Initialize()
        {
            _multiView = _contentViewMediator.MultiView;

            SetMultipartState(ContentMultipartState.InitializeMode);
        }

        private void AddListenersMainView()
        {
            //Fullscreen Mode
            contentViewPC.fullscreenToggle.onValueChanged
                .AddListener(value => contentViewPC.multipartButton.gameObject.SetActive(value));
            
            //Back To Select State
            contentViewPC.showMultipartCloseButton.onClick
                .AddListener(() => SetMultipartState(ContentMultipartState.StartSelectPart));
            
            //Turn On Multipart Mode
            contentViewPC.multipartButton.onClick
                .AddListener(() => SetActiveMultipartMode(true));
          
            //Turn Off Multipart Mode
            contentViewPC.multipartCloseButton.onClick
                .AddListener(() => SetActiveMultipartMode(false));
            
            //Clear All Selected Part
            contentViewPC.clearAllSelectedPart.onClick.AddListener(DeselectLabels);
            
            //Start Multipart View 
            contentViewPC.startViewSelectedPart.onClick.AddListener(StartMultipartView);
        }
        
        private void AddListenersMultipartView(MultiView multiView, string partName)
        {
            // Close View
            multiView.MultipartView.CloseScreen.onClick.AddListener(() => CloseView(multiView, partName));
            
            // Change background color
            multiView.MultipartView.ColorPicker.onValueChanged.AddListener(value => {multiView.MultipartView.RenderCamera.backgroundColor = value; });
            
            //Zoom Plus
            multiView.MultipartView.ZoomPlusButton.onClick.AddListener(
                () => multiView.MultipartView.SmoothOrbitCam.Zoom(0.5f));
            
            //Zoom Minus
            multiView.MultipartView.ZoomMinusButton.onClick.AddListener(
                () => multiView.MultipartView.SmoothOrbitCam.Zoom(-0.5f));
            
            //Reset
            multiView.MultipartView.ResetButton.onClick.AddListener(multiView.MultipartView.SmoothOrbitCam.ResetMainValues);
        }

        private void SetActiveMultipartMode(bool isActive)
        {
            DeselectLabels();

            if (isActive)
            {
                _contentViewMediator.ClosePaintMode();
                SetMultipartState(ContentMultipartState.StartMode);
            }
            else
            {
                SetMultipartState(ContentMultipartState.DisposeMode);
            }
        }

        private void DeselectLabels()
        {
            foreach (var lt in _contentViewMediator._labelToggles)
            {
                _contentViewMediator._contentView.labelsListDropdownToggle.Template.GetComponent<Toggle>().SetValue(false, true);
                lt.Value.SetValue(false, true);
            }
        }

        public void StartMultipartView()
        {
            if(selectedPart.Count < 1)
                return;
           
            SetMultipartState(ContentMultipartState.StartShowPart);
        }
        
        public void Tick()
        {
            switch (_contentMultipartState)
            {
                case ContentMultipartState.SelectPart:
                    ToolTip();
                    break;
                
                case ContentMultipartState.StartShowPart:

                    break;
                
                case ContentMultipartState.ShowPart:
                    break;
            }
        }

        private void UpdateSelectedPart(string modelPartName, bool isActive)
        {
            if (isActive)
            {
                if(!selectedPart.Contains(modelPartName))
                    selectedPart.Add(modelPartName);

                if (selectedPart.Count > _maxSelectedPart)
                {
                    _contentViewMediator._labelToggles[selectedPart[0]].isOn = false;
                }
            }
            else
            {
                if(selectedPart.Contains(modelPartName))
                    selectedPart.Remove(modelPartName);
            }
            
            if(selectedPart.Count > 0)
                contentViewPC.selectedPartText.SetText($"{selectedPart.Count}/{_maxSelectedPart} {_contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.PartsSelectedKey)}");
            else
                contentViewPC.selectedPartText.SetText(_contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.SelectPartsKey));
        }

        string toolTipText = string.Empty;
        private void ToolTip()
        {
            Ray ray = contentViewPC.model3DCamera.ScreenPointToRay (Input.mousePosition);
            int layerMask = LayerMask.GetMask("UI");
            
            if (Physics.Raycast (ray,out RaycastHit hit, layerMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);

                if (hit.collider.gameObject.HasComponent(out ObjectHighlighter oh) && !EventSystem.current.IsPointerOverGameObject())
                {
                    toolTipText = GetTranslationsForItem(_contentViewModel.mainAssetID, oh.gameObject.name, _contentViewModel.CurrentContentViewCultureCode);

                    if (!string.IsNullOrEmpty(toolTipText))
                    {
                       TooltipPanel.Instance?.SetEnableTooltip(toolTipText);
                        return;
                    }
                }
            }
            
            TooltipPanel.Instance?.SetSafeDisableTooltip(toolTipText);
        }

        private void CreateParts()
        {
            for (int i = 0; i < selectedPart.Count; i++)
            {
                MultiView mv = _multiViewFactory.Create(startRenderLayerIndex + i, MultiViewType.Multipart);
                mv.ScaleFontSize(_accessibilityModel.ModulesFontSizeScaler);

                _multiView.Add(mv);
                    
                CreatePart(mv.RenderLayer, mv.MultipartView, _contentViewMediator._models[selectedPart[i]].gameObject);
                    
                AddListenersMultipartView(mv, _contentViewMediator._models[selectedPart[i]].name);
                
                _asyncProcessor.Wait(0, () => { 
                    mv.MultipartView.SmoothOrbitCam.enabled = true;
                    mv.MultipartView.SmoothOrbitCam.AutoZoomOnTarget(true);});
                
                mv.MultipartView.SmoothOrbitCam.xSpeed =
                    mv.MultipartView.SmoothOrbitCam.ySpeed = cameraSettings.RotationSpeed;
            }
            
            switch (_multiView.Count)
            {
                case 1 :
                   _multiView[0].MultipartView.RenderCamera.rect = ViewportRectPresets.FullScreen;
                    
                    break;
                
                case 2:
                    _multiView[0].MultipartView.RenderCamera.rect = ViewportRectPresets.LeftOfTwo;
                    _multiView[1].MultipartView.RenderCamera.rect = ViewportRectPresets.RightOfTwo;

                    break;
                
                case 3:
                    _multiView[0].MultipartView.RenderCamera.rect = ViewportRectPresets.LeftOfThree;
                    _multiView[1].MultipartView.RenderCamera.rect = ViewportRectPresets.MiddleOfThree;
                    _multiView[2].MultipartView.RenderCamera.rect = ViewportRectPresets.RightOfThree;

                    break;
            }
        }
        
        private void CloseView(MultiView multiView, string partName)
        {
            _contentViewMediator._labelToggles[partName].isOn = false;
            _contentViewMediator._models[partName].GetComponent<ObjectHighlighter>().interactable = true;

            _multiView.Remove(multiView);
            MonoBehaviour.Destroy(multiView.gameObject);

            List<Rect> viewRects = new List<Rect>();
            
            switch (_multiView.Count)
            {
                case 0 :
                    SetMultipartState(ContentMultipartState.StartSelectPart);
                    break;
                
                case 1 :
                    viewRects.Add(ViewportRectPresets.FullScreen);

                    break;
                
                case 2:               
                    viewRects.Add(ViewportRectPresets.LeftOfTwo);
                    viewRects.Add(ViewportRectPresets.RightOfTwo);
                   
                    break;
            }
            
            if(_multiView.Count > 0)
                for (int i = 0; i < _multiView.Count; i++)
                {
                    _multiView[i].MultipartView.RenderCamera.DORect(viewRects[i], _transitionDuration).SetEase(Ease.OutQuad);
                }
        }

        private void CreatePart(GameObject renderLayer, RenderView renderView, GameObject part)
        {
            // Localization
//        Model3DLabelItem item = _contentModel.GetAssetById(_contentViewMediator._assetID)
//            .Asset.AssetContents[0].Metadata.Model3DLabels
//            .Items.Find(labelItem => labelItem.LocalizedText[_localizationModel.FallbackCultureCode] == part.name);
//        
//        string name = GetCurrentTranslationsForItem(item.LocalizedText);
        
            string name = GetCurrentTranslationsForItem(_contentViewModel.mainAssetID, part.name);

            renderView.ScreenName.text = name;
        
            renderLayer.GetAllInSceneOnLayer<TooltipEvents>().ForEach(t =>
            {
                t.SetHint(_contentViewModel.GetCurrentSystemTranslations(t.GetKey()));
            });
        
            GameObject go = MonoBehaviour.Instantiate(part, renderLayer.transform, false);
        
            renderLayer.transform.rotation = _load3DModelContainer.transform.rotation;
            go.transform.localRotation = Quaternion.identity;
        
            ObjectHighlighter oh = go.GetComponent<ObjectHighlighter>();
            _asyncProcessor.Wait(0f, () => oh.OffHighlightMaterial(true));
                
            // Set Pivot Point to Center Of Mass
            SetPivotPoint(go, _contentViewModel.mainAssetID);
            
            go.transform.SetLayer(renderLayer.layer);
            
            _contentViewModel.SetLanguageOnLayer(_contentViewMediator._layerIndex, _contentViewModel.CurrentContentViewCultureCode);
        }
        
        private void SetSelectedPartClickable(bool _isClickable)
        {
            foreach (string s in selectedPart)
            {
                _contentViewMediator._models[s].GetComponent<ObjectHighlighter>().interactable = _isClickable;
            }
        }

        private void SetMultipartState(ContentMultipartState newState)
        {
            if(_contentMultipartState == newState)
                return;
            
            _contentMultipartState = newState;
            
            switch (newState)
            {
                case ContentMultipartState.None:
                    break;
                
                case ContentMultipartState.InitializeMode:
                    contentViewPC = _contentViewMediator._contentView;
                    contentViewPC.GetReferenceUI();
                    AddListenersMainView();
                    break;
                
                case ContentMultipartState.StartMode:               
                    _contentViewMediator.modelPartSelectedAction += UpdateSelectedPart;
                    SetMultipartState(ContentMultipartState.StartSelectPart);

                    break;
                
                case ContentMultipartState.StartSelectPart:
                    SetSelectedPartClickable(true);
                    
                    if(selectedPart.Count > 0)
                        contentViewPC.selectedPartText.SetText($"{selectedPart.Count}/{_maxSelectedPart} {_contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.PartsSelectedKey)}");
                    else
                        contentViewPC.selectedPartText.SetText(_contentViewModel.GetCurrentSystemTranslations(LocalizationConstants.SelectPartsKey));

                    _contentViewMediator._contentView.model3DCamera.DORect(ViewportRectPresets.FullScreen, _transitionDuration)
                        .SetEase(Ease.OutQuad)
                        .onComplete += () => SetMultipartState(ContentMultipartState.SelectPart);

                    contentViewPC.multipartCameraViewPanel.SetActive(false);
                    
                    //Turn Off Show Multipart Panel
                    contentViewPC.showMultipartPanel.SetActive(false);
                    
                    //Turn Off Main Mode Panel
                    contentViewPC.rightMenuPanel.SetActive(false);
                    contentViewPC.topLeftMenuPanel.SetActive(false);
                    contentViewPC.topRightMenuPanel.SetActive(false);
            
                    //Turn On Multipart Top Panel
                    contentViewPC.multipartPanel.gameObject.SetActive(true);
                    
                    //Turn On Bot Panel
                    contentViewPC.bottomLeftMenu.gameObject.SetActive(true);
                    
                    //Turn On Model Name
                    contentViewPC.modelNameLabel.transform.parent.parent.gameObject.SetActive(true);
                    break;
                
                case ContentMultipartState.SelectPart:
                    ClearMultiview();
                    break;
                
                case ContentMultipartState.StartShowPart:
                    SetSelectedPartClickable(false);
                    CreateParts();
                    
                    _contentViewMediator._contentView.model3DCamera.DORect(ViewportRectPresets.TopLeftZero, _transitionDuration)
                        .SetEase(Ease.OutQuad)
                        .onComplete += () => SetMultipartState(ContentMultipartState.ShowPart);

                    //Turn On Show Multipart Panel
                    contentViewPC.showMultipartPanel.SetActive(true);

                    //Turn Off Multipart Top Panel
                    contentViewPC.multipartPanel.gameObject.SetActive(false);

                    //Turn Off Bot Panel
                    contentViewPC.bottomLeftMenu.gameObject.SetActive(false);
                    
                    //Turn Off Model Name
                    contentViewPC.modelNameLabel.transform.parent.parent.gameObject.SetActive(false);
                    
                    break;
                
                case ContentMultipartState.ShowPart:
                    contentViewPC.multipartCameraViewPanel.SetActive(true);
                    
                    break;
                
                case ContentMultipartState.DisposeMode:
                    ClearMultiview();
                    SetSelectedPartClickable(true);
                    SetActiveMultipartMode(false);
                    _contentViewMediator.modelPartSelectedAction -= UpdateSelectedPart;
                    
                    //Turn On Main Mode Panel
                    contentViewPC.rightMenuPanel.SetActive(contentViewPC.rightMenuToggle.isOn);
                    contentViewPC.topLeftMenuPanel.SetActive(true);
                    contentViewPC.topRightMenuPanel.SetActive(true);
            
                    //Turn Off Multipart Top Panel
                    contentViewPC.multipartPanel.gameObject.SetActive(false);
                    
                    break;
            }
        }

        private void ClearMultiview()
        {
            foreach (MultiView view in _multiView)
            {
                MonoBehaviour.Destroy(view.gameObject);
            }
            
            _multiView.Clear();
        }

        public void Dispose()
        {
        }
    }

    public enum ContentMultipartState
    {
        None,
        InitializeMode,
        StartMode,
        StartSelectPart,
        SelectPart,
        StartShowPart,
        ShowPart,
        DisposeMode
    }
}