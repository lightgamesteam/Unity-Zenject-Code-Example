Shader "3DL/Module/3DL Simple Shadow With Stencil Shader" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader {
		Pass {
			Stencil {
				Ref 1
				Comp Equal
			}

			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			uniform float4 _LightColor0;
			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord0 : TEXCOORD0;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};
			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv = v.texcoord0;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			float4 frag(VertexOutput i) : COLOR {
				float3 diffuseColor = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)) * _Color.rgb;
				float normalLightDot = dot(i.normalDir, normalize(_WorldSpaceLightPos0.xyz));
				float3 directDiffuse = normalLightDot * _LightColor0.xyz;
				float3 diffuse = (directDiffuse + UNITY_LIGHTMODEL_AMBIENT.rgb) * diffuseColor;
				return fixed4(diffuse, _Color.a);
			}
			
			ENDCG
		}
	}
	FallBack "Mobile/VertexLit (Only Directional Lights)"
}