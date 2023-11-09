using UnityEngine;

namespace Module.Core {
    public partial class Utilities {
        public class Camera {
            public static void ResetViewPort(UnityEngine.Camera camera) {
                camera.rect = new Rect(0f, 0f, 1f, 1f);
            }

            public enum RecalculateTypes {
                // InsideContentHor,
                // InsideContentVer,
                // InsideContentHorAndVer,
                OutsideContentHor,
                OutsideContentVer,
                OutsideContentHorAndVer,
            }

            public static void RecalculateViewPort(UnityEngine.Camera camera, RectTransform content, RecalculateTypes type) {
                camera.rect = new Rect(0f, 0f, 1f, 1f);
                var canvas = content.GetComponentInParent<Canvas>();
                var scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
                var viewport = camera.ScreenToViewportPoint(content.sizeDelta * scaleFactor);
                var rect = new Rect(0f, 0f, 1f, 1f);
                switch (type) {
                    // case RecalculateTypes.InsideContentHor:
                    //     rect.x = content.pivot.x - content.anchorMin.x * viewport.x;
                    //     rect.width = viewport.x;
                    //     break;
                    // case RecalculateTypes.InsideContentVer:
                    //     rect.y = content.pivot.y - content.anchorMin.y * viewport.y;
                    //     rect.height = viewport.y;
                    //     break;
                    // case RecalculateTypes.InsideContentHorAndVer:
                    //     rect.x = content.pivot.x - content.anchorMin.x * viewport.x;
                    //     rect.y = content.pivot.y - content.anchorMin.y * viewport.y;
                    //     rect.width = viewport.x;
                    //     rect.height = viewport.y;
                    //     break;
                    case RecalculateTypes.OutsideContentHor:
                        rect.x = viewport.x - content.pivot.x * viewport.x;
                        rect.width = 1f - viewport.x;
                        break;
                    case RecalculateTypes.OutsideContentVer:
                        rect.y = viewport.y - content.pivot.y * viewport.y;
                        rect.height = 1f - viewport.y;
                        break;
                    case RecalculateTypes.OutsideContentHorAndVer:
                        rect.x = viewport.x - content.pivot.x * viewport.x;
                        rect.y = viewport.y - content.pivot.y * viewport.y;
                        rect.width = 1f - viewport.x;
                        rect.height = 1f - viewport.y;
                        break;
                }
                camera.rect = rect;
            }
        }
    }
}
