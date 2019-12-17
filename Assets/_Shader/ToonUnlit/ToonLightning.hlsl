#ifndef TOON_LIGHTNING
#define TOON_LIGHTNING

#include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"
#include "Assets/_Shader/ToonUnlit/ToonLibrary/ToonToBuiltinUnity.hlsl"
#include "Assets/_Shader/ToonUnlit/ToonLibrary/ToonDiffuse.hlsl"

#ifdef RIM_LIGHTNING_ENABLED
#include "Assets/_Shader/ToonUnlit/ToonLibrary/ToonRim.hlsl"
#endif

#ifdef TOON_SPECULAR_ENABLED
#include "Assets/_Shader/ToonUnlit/ToonLibrary/ToonSpecular.hlsl"
#endif

#ifdef TOON_EMISSION_ENABLED
#include "Assets/_Shader/ToonUnlit/ToonLibrary/ToonEmission.hlsl"
#endif

struct ToonLightningData {
    half4 diffuse; //The diffuse color (sampled from _BaseMap_ST)
    half3 worldPosition; // From Varyings
    half3 worldNormal; // From Varyings
#if defined(RIM_LIGHTNING_ENABLED) || defined(TOON_SPECULAR_ENABLED)
    half3 worldViewDirection; // From Varyings
#endif
#if defined(TOON_EMISSION_ENABLED) || defined(TOON_SPECULAR_ENABLED) || defined(RIM_LIGHTNING_ENABLED)
    float2 uv; // From Varyings
#endif
    float4 shadowCoords; // From Varyings
    half3 bakedGI; // From Varyings
};

void ToonLight(ToonLightningData ToonLightningData, Light light, inout half3 diffuseColor, inout half3 rimColor, inout half3 specularColor){
    half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
    diffuseColor += ToonDiffuse(ToonLightningData.worldNormal, light.direction, light.color, lightAttenuation);
    
#ifdef RIM_LIGHTNING_ENABLED
    rimColor += ToonRim(ToonLightningData.uv, ToonLightningData.worldNormal, ToonLightningData.worldViewDirection, light.color, lightAttenuation);
#endif

#ifdef TOON_SPECULAR_ENABLED
    specularColor += ToonSpecular(ToonLightningData.uv, ToonLightningData.worldViewDirection, ToonLightningData.worldNormal, light.direction, light.color, lightAttenuation);
#endif
}

half4 ToonFragment(ToonLightningData ToonLightningData){
    Light mainLight =  ToonGetMainLight(ToonLightningData.shadowCoords);
    MixRealtimeAndBakedGI(mainLight, ToonLightningData.worldNormal, ToonLightningData.bakedGI, half4(0, 0, 0, 0));

    half alpha = ToonLightningData.diffuse.w * _BaseColor.w;
    half3 diffuseColor = ToonLightningData.bakedGI;
    half3 rimColor = half3(0,0,0);
    half3 specularColor = half3(0,0,0);
    half3 emissionColor = half3(0,0,0);

    ToonLight(ToonLightningData, mainLight, diffuseColor, rimColor, specularColor);

#ifdef _ADDITIONAL_LIGHTS
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, ToonLightningData.worldPosition);
        ToonLight(ToonLightningData, light, diffuseColor, rimColor, specularColor);
    }
#endif

#ifdef TOON_EMISSION_ENABLED
    emissionColor += ToonEmission(ToonLightningData.uv);
#endif

    half3 finalColor = (diffuseColor * ToonLightningData.diffuse.xyz * _BaseColor.xyz) + rimColor + specularColor + emissionColor;
    return half4(finalColor, alpha);
}

#endif