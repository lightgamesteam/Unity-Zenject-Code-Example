using UnityEngine;

namespace Module.Core {
    public static partial class Utilities {
        public static class Component {
            public static bool IsActiveCanvasGroup(CanvasGroup canvasGroup) {
                if (canvasGroup == null) {
                    return false;
                }
                return canvasGroup.alpha > .1f && canvasGroup.interactable && canvasGroup.blocksRaycasts;
            }

            public static void SetActiveCanvasGroup(CanvasGroup canvasGroup, bool isActive) {
                if (canvasGroup == null) {
                    return;
                }
                canvasGroup.alpha = isActive ? 1f : 0f;
                canvasGroup.interactable = isActive;
                canvasGroup.blocksRaycasts = isActive;
            }
        }
    }
}
