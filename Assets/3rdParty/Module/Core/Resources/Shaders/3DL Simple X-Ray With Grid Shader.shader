// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "3DL/Module/3DL Simple X-Ray With Grid Shader" {
	Properties {
		[Toggle] _UseRim("Enable Rim", Float) = 1
		[Toggle] _UseInvert("Enable Invert", Float) = 0
		_Color("Color", Color) = (0, 1, 0, 1)
		_Inside("Inside", Range(0, 1)) = 0
		_Rim("Rim", Range(0, 2)) = 1.2
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4

		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		[Toggle] _UseGrid("Enable Grid", Float) = 1
		_GridThickness("Grid Thickness", Float) = 0.2
		_GridSpacing("Grid Spacing", Float) = 0.03
	}
	SubShader {
		//Tags { "Queue" = "Transparent" }
		//LOD 1
		//LOD 80

		Pass {
			Name "Darken"
			Cull off
			Zwrite off
			Blend dstcolor zero
			ZTest [_ZTest]

			CGPROGRAM

			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma multi_compile_fwdbase

			#include "HLSLSupport.cginc"
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_surf {
				half4 pos : SV_POSITION;
				fixed4 finalColor : COLOR0;
				float4 worldPos : TEXCOORD0;
				float2 texcoord : TEXCOORD1;
			};

			bool _UseRim;
			bool _UseInvert;
			uniform half4 _Color;
			uniform half _Rim;
			uniform half _Inside;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			bool _UseGrid;
			uniform float _GridThickness;
			uniform float _GridSpacing;

			v2f_surf vert_surf(appdata v) {
				v2f_surf o;
				o.pos = UnityObjectToClipPos(v.vertex);
				half3 uv = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				uv = normalize(uv);
				o.finalColor = lerp(half4(1, 1, 1, 1), _Color, saturate(max(1 - pow(uv.z, _Rim), _Inside)));
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag_surf(v2f_surf IN) : COLOR {
				if (_UseRim) {
					float4 finalColor = IN.finalColor;
					if (_UseInvert) {
						float4 texColor = tex2D(_MainTex, IN.texcoord);
						half sum = 0;
						sum += abs(texColor.r - finalColor.r);
						sum += abs(texColor.g - finalColor.g);
						sum += abs(texColor.b - finalColor.b);
						sum /= 3;
						if (sum > .4f) {
							finalColor = half4(1, 1, 1, 1) - finalColor;
						}
					}
					if (_UseGrid) {
						if (frac(IN.worldPos.x / _GridSpacing) < _GridThickness || frac(IN.worldPos.y / _GridSpacing) < _GridThickness) {
							return finalColor;
						}
						else {
							return half4(1, 1, 1, 1);
						}
					}
					else {
						return finalColor;
					}
				}
				else {
					return half4(1, 1, 1, 1);
				}
			}

			ENDCG
		}

		Pass {
			Name "Lighten"
			Cull off
			Zwrite off
			Blend oneminusdstcolor one

			CGPROGRAM

			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma multi_compile_fwdbase

			#include "HLSLSupport.cginc"
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_surf {
				half4 pos : SV_POSITION;
				fixed4 finalColor : COLOR0;
				float4 worldPos : TEXCOORD0;
				float2 texcoord : TEXCOORD1;
			};

			bool _UseRim;
			bool _UseInvert;
			uniform half4 _Color;
			uniform half _Rim;
			uniform half _Inside;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			bool _UseGrid;
			uniform float _GridThickness;
			uniform float _GridSpacing;

			v2f_surf vert_surf(appdata v) {
				v2f_surf o;
				o.pos = UnityObjectToClipPos(v.vertex);
				half3 uv = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				uv = normalize(uv);
				o.finalColor = lerp(half4(0, 0, 0, 0), _Color, saturate(max(1 - pow(uv.z, _Rim), _Inside)));
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag_surf(v2f_surf IN) : COLOR {
				if (_UseRim) {
					float4 finalColor = IN.finalColor;
					if (_UseInvert) {
						float4 texColor = tex2D(_MainTex, IN.texcoord);
						half sum = 0;
						sum += abs(texColor.r - finalColor.r);
						sum += abs(texColor.g - finalColor.g);
						sum += abs(texColor.b - finalColor.b);
						sum /= 3;
						if (sum > .6f) {
							finalColor = half4(1, 1, 1, 1) - finalColor;
						}
					}
					if (_UseGrid) {
						if (frac(IN.worldPos.x / _GridSpacing) < _GridThickness || frac(IN.worldPos.y / _GridSpacing) < _GridThickness) {
							return finalColor;
						}
						else {
							return half4(0, 0, 0, 0);
						}
					}
					else {
						return finalColor;
					}
				}
				else {
					return half4(0, 0, 0, 0);
				}
			}

			ENDCG
		}
	}
	FallBack "Mobile/VertexLit"
}