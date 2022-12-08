Shader "Custom/PuddleShader"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _MinFade ("Min Fade", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular alpha:fade interpolateview
        #pragma target 3.0

        sampler2D _MainTex;
        float _MinFade;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float viewDot = 1-saturate(dot(IN.viewDir, IN.worldNormal));
            viewDot = max(_MinFade,pow(viewDot, 2.0));

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = half3(1,1,1);
            o.Smoothness = c.a * 0.5f + 0.5f;
            o.Alpha = c.a * viewDot;
            o.Specular = float3(o.Smoothness, o.Smoothness, o.Smoothness);
        }
        ENDCG
    }
}
