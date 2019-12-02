#ifndef TOON_EMISSION
#define TOON_EMISSION

#include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"

half3 ToonEmission(float2 uv) {
    return SampleEmissionMap(uv) * _EmissionColor;
}

#endif