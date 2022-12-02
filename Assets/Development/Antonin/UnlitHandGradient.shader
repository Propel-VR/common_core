Shader "Custom/Unlit Hand Gradient"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _FresnelPow("Fresnel Pow", Float) = 2
        _MinMaxFade("Min Max Fade Z", Vector) = (0, 1, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

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
                float4 posOS : POSITION;
                float4 normalOS : NORMAL;
                float3 tex0 : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
                float3 posWS : INTERP0;
                float4 posOS : INTERP1;
                float3 normalOS : INTERP2;
                float3 tex0 : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _Color;
            float _FresnelPow;
            float4 _MinMaxFade;

            float unlerp(float from, float to, float value) {
                return (value - from) / (to - from);;
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.posCS = UnityObjectToClipPos(v.posOS);
                o.posOS = v.posOS;
                o.posWS = mul(unity_ObjectToWorld, v.posOS).xyz;
                o.normalOS = v.normalOS;
                o.tex0 = v.tex0;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = _Color;
                col.a *= 1-pow(saturate(dot(i.normalOS, normalize(ObjSpaceViewDir(i.posOS)))), _FresnelPow);
                col.a *= 1-saturate(unlerp(_MinMaxFade.x, _MinMaxFade.y, i.tex0.x));
                return col;
            }

            
            ENDCG
        }
    }
}
