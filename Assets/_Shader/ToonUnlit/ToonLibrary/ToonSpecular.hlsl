#ifndef TOON_SPECULAR
#define TOON_SPECULAR

#include "Assets/ToonUnlit/ToonInput.hlsl"

half3 ToonSpecular(half3 worldViewDirection, half3 worldNormal, half3 worldLightDirection, half3 lightColor, half lightAttenuation){
    half3 halfDir = SafeNormalize(worldLightDirection + worldViewDirection);
    half NoH = pow(saturate(dot(worldNormal, halfDir)), _SpecularPower);
    return SampleSpecularRamp(float2(NoH,0)) * lightColor * lightAttenuation * _SpecularColor;
}

#endif