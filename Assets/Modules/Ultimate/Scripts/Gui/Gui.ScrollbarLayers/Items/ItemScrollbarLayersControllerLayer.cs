using UnityEngine.Events;

namespace TDL.Modules.Ultimate.GuiScrollbarLayers {
    public class ItemScrollbarLayersControllerLayer : ItemScrollbarLayersControllerBase<ItemScrollbarLayersModelLayer, ItemScrollbarLayersViewLayer> {
        public void InitializeSubContent(SubContentType contentType, UnityAction action) {
            SetSubContent(contentType);
            View.SubContentButton.AddListener(action);
        }

        public void SetSubContent(SubContentType contentType) {
            DisableAll();
            
            switch (contentType) {
                case SubContentType.Hide:
                    View.SubContentGameObject.SetActive(true);
                    View.SubContentHideCanvasGroup.SetActive(true);
                    break;
                case SubContentType.Show:
                    View.SubContentGameObject.SetActive(true);
                    View.SubContentShowCanvasGroup.SetActive(true);
                    break;
            }
        }

        private void DisableAll() {
            View.SubContentGameObject.SetActive(false);
            View.SubContentHideCanvasGroup.SetActive(false);
            View.SubContentShowCanvasGroup.SetActive(false);
        }
    }
}