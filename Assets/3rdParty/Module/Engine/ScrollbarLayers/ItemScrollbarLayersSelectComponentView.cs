using UnityEngine;

namespace Module.Engine.ScrollbarLayers.Item {
    public class ItemScrollbarLayersSelectComponentView : MonoBehaviour {
        public ItemScrollbarLayersView LayerView { get; private set; }
        public ItemScrollbarLayersPartView[] PartViewArray { get; private set; }
        public SubContentType SubContentType { get; private set; }

        public void Initialize(GameObject layerPrefab, GameObject partPrefab, int subPartCount) {
            LayerView = InstantiatePrefab<ItemScrollbarLayersView>(layerPrefab);
            PartViewArray = new ItemScrollbarLayersPartView[subPartCount];
            for (var i = 0; i < subPartCount; i++) {
                PartViewArray[i] = InstantiatePrefab<ItemScrollbarLayersPartView>(partPrefab);
            }

            SubContentType = subPartCount == 0 ? SubContentType.None : SubContentType.Hide;
            LayerView.InitializeSubContent(SubContentType, SwitchContent);
            SetSubContent(SubContentType);
        }

        public void SetSubContent(SubContentType contentType) {
            SubContentType = contentType;
            LayerView.SetSubContent(contentType);
            switch (contentType) {
                case SubContentType.Show:
                    if (PartViewArray != null) {
                        foreach (var partView in PartViewArray) {
                            partView.gameObject.SetActive(true);
                        }
                    }
                    break;
                case SubContentType.None:
                case SubContentType.Hide:
                default:
                    if (PartViewArray != null) {
                        foreach (var partView in PartViewArray) {
                            partView.gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }

        private void SwitchContent() {
            switch (SubContentType) {
                case SubContentType.Show:
                    SetSubContent(SubContentType.Hide);
                    break;
                case SubContentType.Hide:
                    SetSubContent(SubContentType.Show);
                    break;
            }
        }

        private T InstantiatePrefab<T>(GameObject prefab) where T : ItemScrollbarLayersPartViewBase {
            var view = Instantiate(prefab);
            view.transform.SetParent(transform);
            view.transform.localScale = Vector3.one;
            return view.GetComponent<T>();
        }
    }
}
