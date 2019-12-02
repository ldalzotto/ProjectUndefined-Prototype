#ifndef TOON_TO_BUILTIN_UNITY
#define TOON_TO_BUILTIN_UNITY

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

Light ToonGetMainLight(float4 shadowCoord)
{
    return GetMainLight(shadowCoord);
}

#endif