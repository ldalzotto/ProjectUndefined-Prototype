Shader "Unlit/UICooldownShaderText"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackgroundColor ("BackgroundColor", Color) = (1.0,1.0,1.0,1.0)
        _CooldownProgress ("Cooldown Progress", Range(0.0,1.0)) =  0.0
        _CooldownColor ("Cooldown Color", Color) = (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half _CooldownProgress;
            half4 _CooldownColor;
            half4 _BackgroundColor;
CBUFFER_END

TEXTURE2D(_MainTex);                   SAMPLER(sampler_MainTex);

           // StructuredBuffer<Point> _inputUV;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs VertexPositionInputs = GetVertexPositionInputs(v.vertex);
                o.vertex = VertexPositionInputs.positionCS;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv1 = v.uv1;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col;
                
                half4 sampledTexture = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                if(1-step(1 - i.uv1.y, _CooldownProgress))
                {
                    col.xyz = sampledTexture.xyz * _BackgroundColor * _CooldownColor;
                } 
                else 
                {
                    col.xyz = sampledTexture * _BackgroundColor;
                }
                
                return half4(col.xyz, sampledTexture.w);
            }
            ENDHLSL
        }
    }
}
