Shader "Unlit/PathVisualFeedbackShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TranslationSpeed("Translation Speed", Float) = 1.0
        [HDR] _Color ("Color", Color) = (1.0,1.0,1.0,0.0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half _TranslationSpeed;
            half4 _Color;
            CBUFFER_END
            
            half UNSCALED_TIME;
            
            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformWorldToHClip(TransformObjectToWorld(v.vertex.xyz));
                o.uv = TRANSFORM_TEX(v.uv + float2(_TranslationSpeed * UNSCALED_TIME, 0), _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _Color;
            }
            ENDHLSL
        }
    }
}
