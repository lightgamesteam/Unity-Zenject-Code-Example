using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using TDL.Constants;
using TDL.Models;
using UnityEngine.EventSystems;

namespace TDL.Modules.Model3D
{
    public class ContentMultimodelPartViewMediator : ContentViewMediatorBase, IInitializable, ITickable, IDisposable
    {
        [Inject] private MultiView.Factory  _multiViewFactory;
        [Inject] private readonly AsyncProcessorService _asyncProcessor;
        [Inject] private Camera3DModelSettings cameraSettings;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private ContentViewModel _contentViewModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private ContentViewMediator _contentViewMediator;
        [Inject] private Load3DModelContainer _load3DModelContainer;
        [Inject] private readonly AccessibilityModel _accessibilityModel;
        
        private Dictionary<int, ObjectHighlighter> selectedParts = new Dictionary<int, ObjectHighlighter>();
        private Dictionary<int, RenderView> multiModelViews = new Dictionary<int, RenderView>();
        private Dictionary<int, MultiView> multiPartViews = new Dictionary<int, MultiView>();
        private List<int> selectPartEnabled = new List<int>();
        
        private float _transitionDuration = 0.5f;
        private const int startRenderLayerIndex = 2;
        
        private TooltipPanel toolTip => TooltipPanel.Instance;

        public void Initialize()
        {
            ObjectHighlighter.SelectModelPart += SelectModelPart;
        }
        
        public void Dispose()
        {
            ObjectHighlighter.SelectModelPart -= SelectModelPart;
        }
        
        public void Tick()
        {
            if(selectPartEnabled.Count > 0)
                MultimodelToolTip();
        }
        
        string toolTipText = string.Empty;
        private void MultimodelToolTip()
        {
            Camera camera = MouseExtension.GetDepthCameraForMousePosition(5);
            
            if(camera == null)
                return;
            
            Ray ray = camera.ScreenPointToRay (Input.mousePosition);
            //Debug.DrawRay(ray.origin, ray.direction, Color.red);

            int layerMask = LayerMask.GetMask("UI");

            List<RaycastHit> hits = new List<RaycastHit>();
            hits.AddRange(Physics.RaycastAll(ray, layerMask));
            hits.RemoveAll(h => h.collider.gameObject.layer != camera.gameObject.layer || !h.collider.gameObject.HasComponent<ObjectHighlighter>());
            hits.Sort((a, b) => a.distance.CompareTo(b.distance));
            
            if (hits.Count > 0 && !EventSystem.current.IsPointerOverGameObject())
            {
                GameObject gh = hits[0].collider.gameObject;
                
                if (gh.HasComponent(out ObjectHighlighter oh) && selectPartEnabled.Contains(gh.layer))
                {
                    toolTipText = GetTranslationsForItem(oh.ID, oh.gameObject.name, GetLanguageOnLayer(oh.gameObject.layer));

                    if (!string.IsNullOrEmpty(toolTipText))
                    {
                        toolTip.SetEnableTooltip(toolTipText);
                        return;
                    }
                }
            }
            
            toolTip.SetSafeDisableTooltip(toolTipText);
        }
        
        private void SelectModelPart(bool status, ObjectHighlighter objHighlighter)
        {
            int layer = objHighlighter.gameObject.layer;

            if (!selectPartEnabled.Contains(layer))
                return;
            
            if (selectedParts.ContainsKey(layer))
            {
                ObjectHighlighter curInList = selectedParts[layer];
                
                if (!curInList.Equals(objHighlighter))
                {
                    curInList.GetMyToggle().InvokeValue(false, true);
                }
            }

            if (status)
            {
                selectedParts[layer] = objHighlighter;
                if( multiModelViews.ContainsKey(layer))
                    multiModelViews[layer].MultiModelPartSelectedText.text = GetTranslationsForItem(objHighlighter.ID, objHighlighter.gameObject.name, GetLanguageOnLayer(layer));
            }
            else
            {
                selectedParts.Remove(layer);
                if( multiModelViews.ContainsKey(layer))
                    ClearMultiModelPartSelectedText(multiModelViews[layer]);
            }
        }

        public void InitMultimodelPart(RenderView renderView)
        {
            ClearMultiModelPartSelectedText(renderView);
            
            selectPartEnabled.Add(renderView.GetRenderLayerIndex());
            
            Toggle selectAll = renderView.LabelsListDropdownToggle.Template.GetComponent<Toggle>();
            selectAll.InvokeValue(false, true);
            
            bool isCloseButtonEnabled = renderView.CloseScreen.gameObject.activeSelf;

            if (isCloseButtonEnabled)
            {
                renderView.CloseScreen.gameObject.SetActive(false);
            }
            
            renderView.MultiModelPartButton.transform.parent.gameObject.SetActive(false);
            renderView.MultiModelPartPanel.gameObject.SetActive(true);
            
            // Back Button Events
            renderView.MultiModelPartBackButton.onClick.RemoveAllListeners();
            renderView.MultiModelPartBackButton.onClick.AddListener(() => { DisposeMultimodelPart(renderView, selectAll); });
            
            if (isCloseButtonEnabled)
            {
                renderView.MultiModelPartBackButton.onClick.AddListener(() =>
                {
                    renderView.CloseScreen.gameObject.SetActive(true);
                });
            }
            
            // Apply Button Events
            renderView.MultiModelPartApplyButton.onClick.RemoveAllListeners();
            renderView.MultiModelPartApplyButton.onClick.AddListener(() => { ApplyPart(renderView); });
            
            // Clear Button Events
            renderView.MultiModelPartClearButton.onClick.RemoveAllListeners();
            renderView.MultiModelPartClearButton.onClick.AddListener(() => { ClearSelectedModelPart(renderView); });
            
            multiModelViews[renderView.GetRenderLayerIndex()] = renderView;
        }

        private void DisposeMultimodelPart(RenderView renderView, Toggle selectAll)
        {
            selectPartEnabled.Remove(renderView.GetRenderLayerIndex());
        
            selectAll.InvokeValue(false, true);
            renderView.MultiModelPartButton.transform.parent.gameObject.SetActive(true);
            renderView.MultiModelPartPanel.gameObject.SetActive(false);
            selectedParts.Remove(renderView.GetRenderLayerIndex());
            multiModelViews.Remove(renderView.GetRenderLayerIndex());
        }

        public void DestroyMultipartView(int layer)
        {
            if (multiPartViews.ContainsKey(layer))
            {
                ClearSelectedModelPart(multiModelViews[layer]);
                
                Rect swipePosition = multiPartViews[layer].MultipartView.RenderCamera.rect.GetDownSwipe();
                multiPartViews[layer].MultipartView.RenderCamera.DORect(swipePosition, _transitionDuration).SetEase(Ease.OutQuad)
                    .onComplete += () =>
                {
                    MonoBehaviour.Destroy(multiPartViews[layer].gameObject);
                    multiPartViews.Remove(layer);
                };
            }
        }

        public void DestroyMode()
        {
            foreach (var view in multiPartViews)
            {
                Rect swipePosition = view.Value.MultipartView.RenderCamera.rect.GetDownSwipe();
                view.Value.MultipartView.RenderCamera.DORect(swipePosition, _transitionDuration).SetEase(Ease.OutQuad)
                    .onComplete += () =>
                {
                    MonoBehaviour.Destroy(view.Value.gameObject);
                };
            }
            
            multiModelViews.Clear();
            multiModelViews.Clear();
            selectedParts.Clear();
            selectPartEnabled.Clear();
        }

        private void ApplyPart(RenderView multimodelView)
        {
            multimodelView.MultiModelPartApplyButton.interactable = false;
            _asyncProcessor.Wait(1f, () => { multimodelView.MultiModelPartApplyButton.interactable = true;});
            
            int layer = multimodelView.GetRenderLayerIndex();
            
            if(!selectedParts.ContainsKey(layer))
                return;
            
            MultiView mv = _multiViewFactory.Create(multimodelView.GetRenderLayerIndex() + startRenderLayerIndex, MultiViewType.Multipart);
            mv.ScaleFontSize(_accessibilityModel.ModulesFontSizeScaler);

            multiPartViews[multimodelView.GetRenderLayerIndex()] = mv;
            
            // Screen name
            mv.MultipartView.ScreenName.text = selectedParts[layer].name;
            
            mv.MultipartView.RenderCamera.depth = multimodelView.RenderCamera.depth + 1;
            mv.MultipartView.SmoothOrbitCam.xSpeed = mv.MultipartView.SmoothOrbitCam.ySpeed = cameraSettings.RotationSpeed;
            
            Rect rectFrom = new Rect(multimodelView.RenderCamera.rect.center.x, multimodelView.RenderCamera.rect.center.y, 0, 0);
            mv.MultipartView.RenderCamera.rect = rectFrom;
            mv.MultipartView.RenderCamera.DORect(multimodelView.RenderCamera.rect, _transitionDuration).SetEase(Ease.OutQuad);
                
            CreatePart(mv.MultipartView, multimodelView, selectedParts[layer].gameObject);
            
            _asyncProcessor.Wait(0, () => { 
                mv.MultipartView.SmoothOrbitCam.enabled = true;
                mv.MultipartView.SmoothOrbitCam.AutoZoomOnTarget(true);});
            
            // Close screen
            mv.MultipartView.CloseScreen.onClick.AddListener(() => DestroyMultipartView(multimodelView.GetRenderLayerIndex()));
            
            // Change background color
            mv.MultipartView.ColorPicker.onValueChanged.AddListener(value => {mv.MultipartView.RenderCamera.backgroundColor = value; });
            
            //Zoom Plus
            mv.MultipartView.ZoomPlusButton.onClick.AddListener(() => mv.MultipartView.SmoothOrbitCam.Zoom(0.5f));
            
            //Zoom Minus
            mv.MultipartView.ZoomMinusButton.onClick.AddListener(() => mv.MultipartView.SmoothOrbitCam.Zoom(-0.5f));
            
            //Reset
            mv.MultipartView.ResetButton.onClick.AddListener(mv.MultipartView.SmoothOrbitCam.ResetMainValues);
        }
        
        private void CreatePart(RenderView multipartView, RenderView multimodelView, GameObject part)
        {
            // Localization
            int _assetID = part.GetComponent<ObjectHighlighter>().ID;
            string cultureCode = GetLanguageOnLayer(multipartView.MultiView.RenderLayer.layer - startRenderLayerIndex);
            
            multipartView.ScreenName.text = GetTranslationsForItem(_assetID, part.name, cultureCode);
            
            multipartView.MultiView.RenderLayer.GetAllInSceneOnLayer<TooltipEvents>().ForEach(t =>
            {
                t.SetHint(GetSystemTranslations(cultureCode, t.GetKey()));
            });
            
            GameObject go = MonoBehaviour.Instantiate(part, multipartView.MultiView.RenderLayer.transform, false);
            
            if(multimodelView.MultiView)
                multipartView.MultiView.RenderLayer.transform.rotation = multimodelView.MultiView.RenderLayer.transform.rotation;
            else
                multipartView.MultiView.RenderLayer.transform.rotation = _load3DModelContainer.transform.rotation;

            go.transform.localRotation = Quaternion.identity;
            
            ObjectHighlighter oh = go.GetComponent<ObjectHighlighter>();
            oh.OffHighlightMaterial();

            // Set Pivot Point to Center Of Mass
            SetPivotPoint(go, _assetID);
            
            //part.GetComponent<ObjectHighlighter>().OnHighlightMaterial();

            go.transform.SetLayer(multipartView.MultiView.RenderLayer.layer);
        }
        
        private void ClearSelectedModelPart(RenderView renderView)
        {
            renderView.LabelsListDropdownToggle.Template.GetComponent<Toggle>().InvokeValue(false, true);
            ClearMultiModelPartSelectedText(renderView);
        }

        private void ClearMultiModelPartSelectedText(RenderView renderView)
        {
            renderView.MultiModelPartSelectedText.text =
                GetSystemTranslations(GetLanguageOnLayer(renderView.GetRenderLayerIndex()),
                    LocalizationConstants.SelectPartsKey);
        }
    }
}