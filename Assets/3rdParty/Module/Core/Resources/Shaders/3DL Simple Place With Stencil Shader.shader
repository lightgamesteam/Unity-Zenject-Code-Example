Shader "3DL/Module/3DL Simple Place With Stencil Shader" {
	SubShader {
		Tags { "Queue" = "Geometry-1" }
		ColorMask 0
		ZWrite Off

		Pass {
			Stencil {
				Ref 1
				Comp Always
				Pass Replace
			}
		}
	}
}