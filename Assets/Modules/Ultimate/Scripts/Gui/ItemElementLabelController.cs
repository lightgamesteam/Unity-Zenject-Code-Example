using Gui.Core;
using TDL.Constants;
using TDL.Modules.Ultimate.Core.ActivityData;
using TDL.Modules.Ultimate.Core.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace TDL.Modules.Ultimate.Core {
    public class ItemElementLabelController : MonoViewControllerBase<ItemElementLabelView> {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        [Inject] private readonly ILanguageHandler _managerLanguageHandler = default;
        [Inject] private readonly ILabelListeners _managerLabelListeners = default;
        [Inject] private readonly IModelHandler _managerModelHandler = default;
        [Inject(Id = "Background")] 
        private readonly Camera _backgroundCamera = default;
        private static readonly int ZTestXRay = Shader.PropertyToID("_ZTestXRay");
        private ActivityData.LabelData _data;
        private bool _isActiveLineRender;
        private bool _isOnLabelLine;
        private bool _isCalculatedLabelLinePosition;

        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += UpdateModelAndRefreshView;
            _managerLabelListeners.OnSwitchGroupWithModelEvent += UpdateModelAndRefreshView;
        }
        
        public void Initialize(ActivityData.LabelData data) {
            _data = data;
            _isOnLabelLine = !DeviceInfo.IsMobile() && PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines);
            //View.SelectButton.GetComponent<GuiButtonHandlerScrollbarLabelsItem>().Initialize(Model.Guid);
            SetLabelLineRendererActive(_isOnLabelLine);
            SetLabelLineGameObjectActive(true);
            SetLabelLineXRayActive(false);
            RefreshView();
        }
        
        #region Unity methods
        
        private void Update() {
            if (_isActiveLineRender && _isOnLabelLine) {
                SetLabelLineGameObjectActive(IsCameraLookOnLabelFace(0.2f), false);
            }
        }
        
        private void OnMouseEnter() {
            if (EventSystem.current.IsPointerOverGameObject() || Input.touchCount > 0) { return; }
            if (View.IsMouseEnter) { return; }
            
            SetLabelLineXRayActive(true);
            View.IsMouseEnter = true;
        }

        private void OnMouseExit() {
            if (Input.touchCount > 0) { return; }
            
            View.IsMouseEnter = false;
            SetLabelLineXRayActive(false);
        }
        
        #endregion
        
        protected virtual void RefreshView() {
            View.DisplayText.text = _managerLanguageHandler.GetCurrentTranslations(_data.LocalNames);
            View.DisplayText.enableCulling = true;
            View.DisplayText.ForceMeshUpdate();
            View.HeaderTransform.GetComponent<Renderer>().material.color = _data.HighlightColor;
            View.LineRenderer.material.color = _data.HighlightColor;
            CalculateFormContent();
        }

        private void SetLabelLineGameObjectActive(bool value, bool overrideActive = true) {
            if (overrideActive) {
                _isActiveLineRender = value;
            }
            if (value && View.LineRenderer.enabled && !_isCalculatedLabelLinePosition) {
                CalculateLabelLinePosition();
            }
            View.LineRenderer.gameObject.SetActive(value);
        }
        
        private void SetLabelLineRendererActive(bool value) {
            View.LineRenderer.enabled = value;
        }
        
        private void SetLabelLineXRayActive(bool value) {
            SetLabelLineXRayValue(value ? 8 : 2);
        }
        
        private void SetLabelLineXRayValue(int i) {
            View.LineRenderer.material.SetFloat(ZTestXRay, i);
        }
        
        private void UpdateModelAndRefreshView(ILanguageHandler languageHandler) {
            if (_data.LocalNames == null) { return; }
            
            RefreshView();
        }
        
        private void UpdateModelAndRefreshView(int id, bool layerActive, bool labelActive) {
            if (!_data.Id.Equals(id)) { return; }

            View.Content.SetActive(layerActive);
        }

        private void CalculateFormContent() {
            var bodySize = View.DisplayText.textBounds.extents * View.TextScaleMultiplier + Vector3.forward;
            var headerScale = View.HeaderTransform.localScale;
            View.HeaderTransform.localScale = new Vector3(bodySize.x, headerScale.y, headerScale.z);
            var bodyScale = View.BodyTransform.localScale;
            View.BodyTransform.localScale = new Vector3(bodySize.x, bodyScale.y, bodyScale.z);
        }
        
        private void CalculateLabelLinePosition() {
            if (!_managerModelHandler.TryGetController(_data.Id, out var controller)) { return; }

            // var modelPosition = controller.View.GameObject.GetAllVertexPosition(true).GetClosestPositionTo(transform.position);
            // var elementPosition = View.HeaderTransform.gameObject.GetAllVertexPosition().GetClosestPositionTo(modelPosition);
            // View.LineRenderer.SetPosition(1, modelPosition);
            // View.LineRenderer.SetPosition(0, elementPosition);
            // _isCalculatedLabelLinePosition = true;

            controller.View.GameObject.GetAllVertexPosition(true, result => {
                var modelPosition = result.GetClosestPositionTo(transform.position);
                var elementPosition = View.HeaderTransform.gameObject.GetAllVertexPosition().GetClosestPositionTo(modelPosition);
                View.LineRenderer.SetPosition(1, modelPosition);
                View.LineRenderer.SetPosition(0, elementPosition);
                _isCalculatedLabelLinePosition = true;
            });
        }
        
        private bool IsCameraLookOnLabelFace(float value = 0f) {
            return _backgroundCamera && Vector3.Dot(transform.forward, _backgroundCamera.transform.forward) > value;
        }
    }
}