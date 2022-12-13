Shader "Procedural Tube/Standard(Basic)/Transparent" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[Space(15)][Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", int) = 2
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Tags { "ForceNoShadowCasting"="True"}
        LOD 200
		cull [_Cull]

        CGPROGRAM
		
		// All the vertex transform code is included in the below cginc
		#include "../CGIncludes/ProceduralTubeCG_Standard.cginc"

        #pragma surface surf Standard noshadow alpha
		#pragma vertex vert_tube  // Vertex Function name should be vert_tube (.cginc)
		#pragma instancing_options procedural:setup  // procedural Function name should be setup
		
		struct Input {
            float2 uv_MainTex;
        };

		sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
