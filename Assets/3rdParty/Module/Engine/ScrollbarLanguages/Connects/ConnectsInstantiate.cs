using UnityEngine;

namespace Module.Engine.ScrollbarLanguages {
    public static partial class Connect {
        public class Instantiate {
            public static T Prefab<T>(GameObject prefab, Transform parent) {
                var view = Object.Instantiate(prefab);
                view.transform.SetParent(parent);
                view.transform.localScale = Vector3.one;
                return view.GetComponent<T>();
            }
        }
    }
}