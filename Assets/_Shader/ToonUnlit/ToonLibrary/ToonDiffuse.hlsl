#ifndef TOON_DIFFUSE
#define TOON_DIFFUSE

#include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"

half3 ToonDiffuse(half3 worldNormal, half3 worldLightDirection, half3 lightColor, half lightAttenuation){
    half NdotL = (dot(worldNormal, worldLightDirection) + 1) * 0.5;
    return SampleDiffuseRamp(float2(NdotL, 0)) * lightColor * lightAttenuation;
}

#endif