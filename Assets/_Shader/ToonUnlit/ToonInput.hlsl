#ifndef TOON_INPUT
#define TOON_INPUT


#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
float4 _DiffuseRamp_ST;
half4 _BaseColor;
half _Cutoff;

half _RimPower;
float4 _RimMap_ST;
half _RimOffset;
half4 _RimColor;

float4 _SpecularRamp_ST;
float4 _SpecularMask_ST;
half _SpecularPower;
half3 _SpecularColor;

float4 _BumpMap_ST;
half _BumpScale;

float4 _EmissionMap_ST;
half3 _EmissionColor;
CBUFFER_END


TEXTURE2D(_BaseMap);       SAMPLER(sampler_BaseMap);
TEXTURE2D(_BumpMap);       SAMPLER(sampler_BumpMap);
TEXTURE2D(_DiffuseRamp);   SAMPLER(sampler_DiffuseRamp);
TEXTURE2D(_RimMap);        SAMPLER(sampler_RimMap);
TEXTURE2D(_SpecularRamp);  SAMPLER(sampler_SpecularRamp);
TEXTURE2D(_SpecularMap);   SAMPLER(sampler_SpecularMap);
TEXTURE2D(_EmissionMap);   SAMPLER(sampler_EmissionMap);

half4 SampleBaseMap(float2 uv){
    return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
}

half4 SampleDiffuseRamp(float2 uv){
    return SAMPLE_TEXTURE2D(_DiffuseRamp, sampler_DiffuseRamp, uv);
}

half3 SampleNormal(float2 uv, half scale = 1.0h)
{
#ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
    return UnpackNormalScale(n, scale);
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

#ifdef RIM_LIGHTNING_ENABLED
half4 SampleRimMap(float2 uv){
    return SAMPLE_TEXTURE2D(_RimMap, sampler_RimMap, uv);
}
#endif

#ifdef TOON_SPECULAR_ENABLED
half4 SampleSpecularRamp(float2 uv){
    return SAMPLE_TEXTURE2D(_SpecularRamp, sampler_SpecularRamp, uv);
}
half4 SampleSpecularMap(float2 uv){
    return SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, uv);
}
#endif

#ifdef TOON_EMISSION_ENABLED
half4 SampleEmissionMap(float2 uv){
    return SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv);
}
#endif

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
}

///////////////////////////////////////////////////////////////////////////////
//                      Material Property Helpers                            //
///////////////////////////////////////////////////////////////////////////////
half Alpha(half albedoAlpha, half4 color, half cutoff)
{
#if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
    half alpha = albedoAlpha * color.a;
#else
    half alpha = color.a;
#endif

#if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
#endif

    return alpha;
}

#endif