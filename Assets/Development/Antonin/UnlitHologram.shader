Shader "Custom/Unlit Hologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _Noise ("Noise", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _MaxDist ("Max Dist", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 posWS : INTERP0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _Noise;
            float4 _MainTex_ST;
            float4 _Color;
            float _MaxDist;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.posWS = float4(mul(unity_ObjectToWorld, v.vertex).xyz, _Time.y);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float filter (float x, float c) {
                return max(0, (x - (1 - 1 / c)) * c);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 sampleOffset = float2(0,filter(tex2D(_Noise, float2(i.uv.x * 0.3, i.posWS.w * 0.1)),3));

                // sample the texture
                float4 col = tex2D(_MainTex, i.uv + sampleOffset * 0.05) * _Color;
                float step = abs(frac(i.posWS.y * 50 - i.posWS.w + sampleOffset.y * 0.5) * 2 - 1) *0.75;

                float pwidth = length(float2(ddx(step), ddy(step)));
                float alpha = smoothstep(0.5, 0.5 - pwidth * 1.5, step);

                col.a *= saturate(max(0.5, alpha));
                col.a *= 1 - saturate(distance(i.posWS.xyz, _WorldSpaceCameraPos) / _MaxDist);
                return col;
            }

            
            ENDCG
        }
    }
}
