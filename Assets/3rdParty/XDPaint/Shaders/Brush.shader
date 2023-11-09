Shader "XD Paint/Brush"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _SrcBlend ("__src", Int) = 5
        _DstBlend ("__dst", Int) = 10
        _ColorAlpha ("__alpha", Float) = 0.0
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        Blend [_SrcBlend] [_DstBlend]
        ColorMask RGBA
        AlphaToMask On
        AlphaTest GEqual [_ColorAlpha]
        Pass
        {
            SetTexture[_MainTex]
            {
                constantColor [_Color]
                combine texture * constant
            }
        }
    }
}