Shader "XD Paint/Paint Preview" {
    Properties {
        _MainTex ("Main", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black" {}
        _BrushTex ("Brush", 2D) = "black" {}
        _BrushOffset ("Brush offset", Vector) = (0, 0, 0, 0)
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
        }
        Pass
        {
            SetTexture [_MaskTex]
            {
                combine texture * previous
            }
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            sampler2D _BrushTex;
            float4 _BrushOffset;
            float4 _Color;
           
            float4 frag (v2f_img i) : SV_Target
            {
                float4 result = tex2D(_BrushTex, float2(i.uv.x / _BrushOffset.z - _BrushOffset.x + 0.5f, i.uv.y / _BrushOffset.w - _BrushOffset.y + 0.5f)) * _Color;
                return result;
            }
            ENDCG
        }
    }
}