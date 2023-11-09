using UnityEngine;

public static class RenderTextureExtension
{
    public static void Clear(this RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, new Color(1f, 1f, 1f, 0f));
        RenderTexture.active = rt;
    }
}
