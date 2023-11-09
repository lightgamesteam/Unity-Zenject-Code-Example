Shader "XD Paint/Restore" {
    Properties {
        _MainTex ("Main", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            SetTexture [_MainTex]
            {
                combine texture
            }
            SetTexture [_MaskTex]
            {
                combine previous, texture
            } 
        }     
    }
}