using UnityEngine;

namespace Module.Engine.ScrollbarLabels.Item {
    public class ItemScrollbarLabelsSelectComponentView : MonoBehaviour {
        public ItemScrollbarLabelsGroupView GroupView { get; private set; }
        public ItemScrollbarLabelsPartView[] PartViewArray { get; private set; }

        public void Initialize(GameObject groupPrefab, GameObject partPrefab, int subPartCount) {
            GroupView = InstantiatePrefab<ItemScrollbarLabelsGroupView>(groupPrefab);
            PartViewArray = new ItemScrollbarLabelsPartView[subPartCount];
            for (var i = 0; i < subPartCount; i++) {
                PartViewArray[i] = InstantiatePrefab<ItemScrollbarLabelsPartView>(partPrefab);
            }
        }

        private T InstantiatePrefab<T>(GameObject prefab) {
            var view = Instantiate(prefab);
            view.transform.SetParent(transform);
            view.transform.localScale = Vector3.one;
            return view.GetComponent<T>();
        }
    }
}
