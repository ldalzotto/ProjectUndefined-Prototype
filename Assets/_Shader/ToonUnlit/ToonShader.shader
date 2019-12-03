Shader "Unlit/ToonShader"
{
    Properties
    {
        //Diffuse
        _DiffuseRamp("Diffuse Ramge", 2D) = "white" {}
        _BaseMap ("Base diffuse map", 2D) = "white" {}
        _BaseColor ("Base color", Color) = (1,1,1,1)
        
        //Rim lightning
        _RimPower("Rim power", Range(-0.1, 1.00)) = -0.1
        _RimOffset("Rim offset", Float) = 1.0
        _RimColor("Rim color", Color) = (1,1,1,1)
        
        //Specular
        _SpecularRamp("Specular Ramp", 2D) = "white" {}
        _SpecularPower("Specular Power", Float) = 0.0
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        
        //Emission
        _EmissionMap("Emission map", 2D) = "black" {}
        [HDR] _EmissionColor("Emission color", Color) = (1,1,1,1)
        
        _ReceiveShadows("Receive Shadows", Float) = 1.0   
        //Blending state
        _Cutoff("Cutoff", Float) = 1.0
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        
        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        
        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}
            
             // Use same blending / depth states as Standard shader
            //Blend SrcAlpha OneMinusSrcAlpha
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]
            
            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #pragma shader_feature RIM_LIGHTNING_ENABLED
            #pragma shader_feature TOON_SPECULAR_ENABLED
            #pragma shader_feature TOON_EMISSION_ENABLED

            #include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"
            #include "Assets/_Shader/ToonUnlit/ToonForwardPass.hlsl"

            #pragma vertex LitToonVertex
            #pragma fragment LitToonFragment

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex LightweightVertexMeta
            #pragma fragment LightweightFragmentMeta

            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma shader_feature _SPECGLOSSMAP

            #include "Assets/_Shader/ToonUnlit/ToonInput.hlsl"
            #include "Assets/_Shader/ToonUnlit/ToonMetaPass.hlsl"

            ENDHLSL
        }
    }
    
    CustomEditor "ToonShaderEditor"
}
