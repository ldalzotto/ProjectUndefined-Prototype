#ifndef TOON_SPECULAR
#define TOON_SPECULAR

#include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"

half3 ToonSpecular(float2 uv, half3 worldViewDirection, half3 worldNormal, half3 worldLightDirection, half3 lightColor, half lightAttenuation){   
    half3 halfDir = SafeNormalize(float3(worldLightDirection) + float3(worldViewDirection));
    half NdotH = saturate(dot(normalize(worldNormal), halfDir));
    half NoH = pow(NdotH, _SpecularPower);
    return SampleSpecularRamp(NoH) * NoH * lightColor * lightAttenuation * _SpecularColor * SampleSpecularMap(uv);
}

#endif