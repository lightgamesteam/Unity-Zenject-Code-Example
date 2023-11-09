using UnityEngine;

public static class RectExtension
{
    public static Rect GetDownSwipe(this Rect rect)
    {
        return new Rect(rect.x, rect.y, rect.width, 0);
    }
}
