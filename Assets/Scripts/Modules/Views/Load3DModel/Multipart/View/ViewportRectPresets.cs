using UnityEngine;

public static class ViewportRectPresets
{
    public static readonly Rect FullScreen = new Rect(0,0,1,1);

    //Multipart
    public static readonly Rect TopLeftZero = new Rect(0, 1, 0, 0);
    public static readonly Rect TopLeft = new Rect(0,0.88f,0.12f,0.12f);
    public static readonly Rect LeftOfTwo = new Rect(0,0,0.5f,1);
    public static readonly Rect RightOfTwo = new Rect(0.5f,0,0.5f,1);
    public static readonly Rect LeftOfThree = new Rect(0,0,0.3333333f,1);
    public static readonly Rect MiddleOfThree = new Rect(0.3333333f,0,0.3333333f,1);
    public static readonly Rect RightOfThree = new Rect(0.6666666f,0,0.3333333f,1);
}

