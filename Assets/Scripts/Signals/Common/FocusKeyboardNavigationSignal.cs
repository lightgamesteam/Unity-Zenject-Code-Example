using UnityEngine;

public class FocusKeyboardNavigationSignal : ISignal
{
   public CanvasGroup FocusCanvasGroup { get; private set; }
   public bool IsOnFocus { get; private set; }
   
   public FocusKeyboardNavigationSignal(CanvasGroup focusCanvasGroup, bool isOnFocus)
   {
      FocusCanvasGroup = focusCanvasGroup;
      IsOnFocus = isOnFocus;
   }
}
