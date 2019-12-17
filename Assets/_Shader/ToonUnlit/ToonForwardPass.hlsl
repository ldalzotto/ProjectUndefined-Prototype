#ifndef TOON_FORWARD_PASS
#define TOON_FORWARD_PASS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/_Shader/ToonUnlit/ToonLightning.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float2 texcoord     : TEXCOORD0;
    float2 lightmapUV   : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};


struct Varyings
{
    float2 uv                       : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

#ifdef _ADDITIONAL_LIGHTS
    float3 positionWS               : TEXCOORD2;
#endif

#ifdef _NORMALMAP
    half4 normalWS                  : TEXCOORD3;    // xyz: normal, w: viewDir.x
    half4 tangentWS                 : TEXCOORD4;    // xyz: tangent, w: viewDir.y
    half4 bitangentWS                : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
#else
    half3 normalWS                  : TEXCOORD3;
    half3 viewDirWS                 : TEXCOORD4;
#endif

    half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light

#ifdef _MAIN_LIGHT_SHADOWS
    float4 shadowCoord              : TEXCOORD7;
#endif

    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

ToonLightningData InitializeToonLightningData(Varyings varyings){
    ToonLightningData toonLightningData;

    //Diffuse map + color
    toonLightningData.diffuse = SampleBaseMap(varyings.uv);
    #ifdef _ADDITIONAL_LIGHTS
    toonLightningData.worldPosition = varyings.positionWS;
    #endif
    #if defined(RIM_LIGHTNING_ENABLED) || defined(TOON_SPECULAR_ENABLED)
    toonLightningData.worldViewDirection = varyings.viewDirWS;
    #endif
    toonLightningData.worldNormal = NormalizeNormalPerPixel(varyings.normalWS);
    #if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
    toonLightningData.shadowCoords = varyings.shadowCoord;
    #else
        toonLightningData.shadowCoords = float4(0, 0, 0, 0);
    #endif
    #if defined(TOON_EMISSION_ENABLED) || defined(TOON_SPECULAR_ENABLED) || defined(RIM_LIGHTNING_ENABLED)
    toonLightningData.uv = varyings.uv;
    #endif
    toonLightningData.bakedGI = SAMPLE_GI(varyings.lightmapUV, varyings.vertexSH, toonLightningData.worldNormal);
    return toonLightningData;
}

Varyings LitToonVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);

    output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
    output.viewDirWS = SafeNormalize(viewDirWS);
    
    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

#ifdef _ADDITIONAL_LIGHTS
    output.positionWS = vertexInput.positionWS;
#endif

#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
    output.shadowCoord = GetShadowCoord(vertexInput);
#endif

    output.positionCS = vertexInput.positionCS;

    return output;
}


half4 LitToonFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    ToonLightningData toonLightningData = InitializeToonLightningData(input);

    return ToonFragment(toonLightningData);
}

#endif