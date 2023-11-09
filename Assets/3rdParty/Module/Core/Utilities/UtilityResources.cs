using UnityEngine;

namespace Module.Core {
    public static partial class Utilities {
        public static class Resources {
            public static T GetObject<T>(string assetPath, string assetName) where T : Object {
                return GetObject<T>(assetPath + assetName);
            }

            public static T GetObject<T>(string fullPath) where T : Object {
                return UnityEngine.Resources.Load<T>(fullPath);
            }

            public static GameObject GetGameObject(string assetPath, string assetName) {
                return GetObject<GameObject>(assetPath, assetName);
            }

            public static GameObject GetGameObject(string fullPath) {
                return GetObject<GameObject>(fullPath);
            }
        }
    }
}
