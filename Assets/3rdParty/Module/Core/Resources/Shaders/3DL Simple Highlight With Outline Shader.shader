Shader "3DL/Module/3DL Simple Highlight With Outline Shader" {
    Properties {
        _HighlightColor ("Highlight color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline width", Range(0,0.1)) = 0.02
        _OutlineColor ("Outline color", Color) = (1, 0.4, 0, 1)
    }
    SubShader {
        Pass {
            CGPROGRAM

			#pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
            };

			float4 vert (appdata_base v) : SV_POSITION {
                return UnityObjectToClipPos(v.vertex);
            }

			uniform float4 _HighlightColor;

            half4 frag(v2f i) : COLOR {
                return _HighlightColor;
            }

            ENDCG
        }
        Pass {
            Tags { "RenderType"="Opaque" }
            Cull Front
 
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct v2f {
                float4 pos : SV_POSITION;
            };
 
            float _OutlineWidth;
            float4 _OutlineColor;
 
            float4 vert(appdata_base v) : SV_POSITION {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 normal = mul((float3x3) UNITY_MATRIX_MV, v.normal);
                normal.x *= UNITY_MATRIX_P[0][0];
                normal.y *= UNITY_MATRIX_P[1][1];
                o.pos.xy += normal.xy * _OutlineWidth;
                return o.pos;
            }
 
            half4 frag(v2f i) : COLOR {
                return _OutlineColor;
            }
 
            ENDCG
        }
    }
    FallBack "Diffuse"
}