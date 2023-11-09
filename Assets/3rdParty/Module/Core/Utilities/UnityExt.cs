using UnityEngine;

public static class UnityExt {
    public static void RemoveChildren(this Transform transform) {
        var arr = new Transform[transform.childCount];
        var i = 0;
        foreach (Transform childTransform in transform) {
            arr[i++] = childTransform;
        }

        for (i = 0; i < arr.Length; i++) {
            var childTransform = arr[i];
            childTransform.SetParent(null);
            Object.Destroy(childTransform.gameObject);
        }
    }

    public static void SetActive(this CanvasGroup canvasGroup, bool isActive) {
        if (canvasGroup == null) {
            return;
        }

        canvasGroup.alpha = isActive ? 1f : 0f;
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
    }
}