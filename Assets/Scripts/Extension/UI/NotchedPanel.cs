using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class NotchedPanel : MonoBehaviour
{
   private RectTransform Panel;
   private Rect LastSafeArea = Rect.zero;
   
   private Rect safeArea => Screen.safeArea;

   void Awake()
   {
      if (ScreenExtension.IsNotched())
      {
         Panel = GetComponent<RectTransform>();
         Refresh(); 
      }
   }

   private void LateUpdate()
   {
      if (ScreenExtension.IsNotched())
         Refresh();
   }

   private void Refresh()
   {
      if (safeArea != LastSafeArea)
         ApplySafeArea(safeArea);
   }
   
   private void ApplySafeArea(Rect r)
   {
      LastSafeArea = r;

      Vector2 anchorMin = r.position;
      Vector2 anchorMax = r.position + r.size;
      anchorMin.x /= Screen.width;
      anchorMin.y /= Screen.height;
      anchorMax.x /= Screen.width;
      anchorMax.y /= Screen.height;
      Panel.anchorMin = anchorMin;
      Panel.anchorMax = anchorMax;
   }
}