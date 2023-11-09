Shader "3DL/Module/3DL Simple Radar And Highlight Shader" {
	Properties {
		[HideInInspector] _Color("Color", Color) = (1, 1, 1, 1)
		[HideInInspector] _MainTex("Albedo", 2D) = "white" {}

		[Toggle]_UseHighlight("Enable Highlight", Float) = 1
		_HighlightColor("Highlight Color",Color) = (1.0, 0.0, 0.0, 0.4)
		
		[Toggle]_UseRadar("Enable Radar", Float) = 1
		_RadarColor("Radar Color",Color) = (0.0, 0.0, 1.0, 0.4)

		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
	#define UNITY_SETUP_BRDF_INPUT MetallicSetup
	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300
		ZTest LEqual
		Zwrite On

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass {
			Name "FORWARD" 
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "UnityStandardCoreForward.cginc"
			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass {
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend [_SrcBlend] One
			Fog { Color (0, 0, 0, 0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "UnityStandardCoreForward.cginc"
			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _PARALLAXMAP
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"
			ENDCG
		}
		// ------------------------------------------------------------------
		//  Deferred pass
		Pass {
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
			#pragma exclude_renderers nomrt

			// -------------------------------------
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_prepassfinal
			#pragma multi_compile_instancing

			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#include "UnityStandardCore.cginc"
			ENDCG
		}
		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass {
			Name "META" 
			Tags { "LightMode"="Meta" }

			Cull Off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature EDITOR_VISUALIZATION

			#include "UnityStandardMeta.cginc"
			ENDCG
		}

		Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" "IgnoreProjector" = "True" "Queue" = "Geometry" }
		ZTest Greater

		CGPROGRAM
		#pragma surface surf NoLighting noforwardadd noshadow alpha:fade keepalpha

		struct Input {
			float3 worldPos;
		};

		bool _UseRadar;
		fixed4 _RadarColor;

		half4 LightingNoLighting(SurfaceOutput s, half3 lightDir, fixed atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			if (_UseRadar) {
				fixed3 normalizeWorld = normalize(_WorldSpaceCameraPos - IN.worldPos);
				fixed3 normalizeOutput = normalize(o.Normal);
				fixed3 normalizeDot = dot(normalizeWorld, normalizeOutput);

				half rim = 1 - saturate(abs(normalizeDot)) + 0.5;
				rim = clamp(rim, 0.0, 1.0);

				half srim = 1 - (rim * _RadarColor.a);
				srim = clamp(srim, 0.0, 1.0);

				fixed4 gradient0 = _RadarColor * _RadarColor.a;
				fixed4 gradient1 = gradient0 * rim;
				fixed4 gradient2 = gradient0 * srim;
				fixed4 color = gradient1 + gradient2;
				color.a = clamp(color.a, 0.0, 1.0);

				o.Albedo = color;
				o.Alpha = ((rim + 1) > 1 ? 1 : 0) * color.a;
			} else {
				o.Albedo = 0;
				o.Alpha = 0;
			}
		}
		ENDCG

		Tags { "RenderType" = "Opaque" }
		ZTest LEqual
		ZWrite On

		CGPROGRAM
		#pragma surface surf NoLighting noshadow alpha:fade keepalpha

		struct Input {
			float3 worldPos;
		};

		bool _UseHighlight;
		fixed4 _HighlightColor;

		half4 LightingNoLighting(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			if (_UseHighlight) {
				fixed3 normalizeWorld = normalize(_WorldSpaceCameraPos - IN.worldPos);
				fixed3 normalizeOutput = normalize(o.Normal);
				fixed3 normalizeDot = dot(normalizeWorld, normalizeOutput);

				half rim = 1 - saturate(abs(normalizeDot)) + 0.5;
				rim = clamp(rim, 0.0, 1.0);

				half srim = 1 - (rim * _HighlightColor.a);
				srim = clamp(srim, 0.0, 1.0);

				fixed4 gradient0 = _HighlightColor * _HighlightColor.a;
				fixed4 gradient1 = gradient0 * rim;
				fixed4 gradient2 = gradient0 * srim;
				fixed4 color = gradient1 + gradient2;
				color.a = clamp(color.a, 0.0, 1.0);

				o.Albedo = color;
				o.Alpha = (rim + 2.0) * color.a;
			} else {
				o.Albedo = 0;
				o.Alpha = 0;
			}
		}
		ENDCG
	}

	FallBack "VertexLit"
}
