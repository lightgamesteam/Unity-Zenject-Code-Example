using UnityEngine;

public static class ScreenExtension
{
    public static bool IsNotched()
    {
        return Screen.cutouts.Length > 0;
    }
}
