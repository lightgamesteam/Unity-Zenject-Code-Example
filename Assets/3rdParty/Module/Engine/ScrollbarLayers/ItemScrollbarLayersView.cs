using Module.Core;
using Module.Core.Attributes;
using Module.Core.UIComponent;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Engine.ScrollbarLayers.Item {
    public class ItemScrollbarLayersView : ItemScrollbarLayersPartViewBase {
        #region Variables

        [SerializeField] protected ComponentButton SubContentButton;
        [LabelOverride("Sub.Content -> Content")]
        [SerializeField] protected GameObject SubContentGameObject;
        [LabelOverride("Sub.Content -> Hide")]
        [SerializeField] protected CanvasGroup SubContentHideCanvasGroup;
        [LabelOverride("Sub.Content -> Show")]
        [SerializeField] protected CanvasGroup SubContentShowCanvasGroup;

        #endregion

        public void InitializeSubContent(SubContentType contentType, UnityAction action) {
            SetSubContent(contentType);
            SubContentButton.AddListener(action);
        }

        public void SetSubContent(SubContentType contentType) {
            switch (contentType) {
                case SubContentType.None:
                    SubContentGameObject.SetActive(false);
                    Utilities.Component.SetActiveCanvasGroup(SubContentHideCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(SubContentShowCanvasGroup, false);
                    break;
                case SubContentType.Hide:
                    SubContentGameObject.SetActive(true);
                    Utilities.Component.SetActiveCanvasGroup(SubContentHideCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(SubContentShowCanvasGroup, false);
                    break;
                case SubContentType.Show:
                    SubContentGameObject.SetActive(true);
                    Utilities.Component.SetActiveCanvasGroup(SubContentHideCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(SubContentShowCanvasGroup, true);
                    break;
                default:
                    SubContentGameObject.SetActive(false);
                    Utilities.Component.SetActiveCanvasGroup(SubContentHideCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(SubContentShowCanvasGroup, false);
                    break;
            }
        }
    }
}
