Shader "Procedural Tube/Standard(non-NormalMap)/Opaque" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 1.0
		_MetallicGlossMap("Metallic, A: Smoothness", 2D) = "white"{}
		_OcclusionStrength ("Occlusion", Range(0,1)) = 1.0
		_OcclusionMap("Occlusion Map", 2D) = "white"{}
		[HDR]_EmissionColor("Emission Color", color) = (0,0,0,1)
		_EmissionMap("Emission Map", 2D) = "black"
		[Space(15)][Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", int) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
		Tags { "ForceNoShadowCasting"="True"}
        LOD 200
		cull [_Cull]

        CGPROGRAM
		
		// All the vertex transform code is included in the below cginc
		#include "../CGIncludes/ProceduralTubeCG_Standard.cginc"

        #pragma surface surf Standard noshadow
		#pragma vertex vert_tube  // Vertex Function name should be vert_tube (.cginc)
		#pragma instancing_options procedural:setup  // procedural Function name should be setup
		#pragma target 3.0
		
		struct Input {
            float2 uv_MainTex;
            float2 uv_MetallicGlossMap;
            float2 uv_OcclusionMap;
            float2 uv_EmissionMap;
        };

		sampler2D _MainTex;
		sampler2D _MetallicGlossMap;
		sampler2D _OcclusionMap;
		sampler2D _EmissionMap;

		fixed4 _Color;
		float4 _EmissionColor;
		half _Metallic;
        half _Glossiness;
        half _OcclusionStrength;
        
       
        void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
			fixed4 m = tex2D(_MetallicGlossMap, IN.uv_MetallicGlossMap);
            o.Metallic = m.r*_Metallic;
            o.Smoothness = m.a*_Glossiness;
			o.Occlusion = saturate(tex2D(_OcclusionMap, IN.uv_OcclusionMap)+_OcclusionStrength);
			o.Emission = tex2D(_EmissionMap, IN.uv_EmissionMap) * _EmissionColor;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
