// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaveNoiseV2"
{
	Properties
	{
		_Color("_Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		[Toggle(NORMAL_MAP)] _IsNormalMap("Normal Map ?", Float) = 0.0
		[ShowOnKeywordDrawer(NORMAL_MAP)] _BumpMap("Bump Map", 2D) = "bump" {}
		[Toggle(VERTEX_COLOR)] _IsVertexColor("Vertex Color ?", Float) = 1.0

		[Toggle(EMISISON)] _IsEmission("Emission ?", Float) = 0.0
		[ShowOnKeywordDrawer(EMISISON)] [HDR]  _EmissionColor("Emission Color", Color) = (0,0,0,0)

		[Toggle(VERTEX_DISPLACEMENT)] _IsVertexDisplaced("Vertex Displacement ?", Float) = 0.0
		_MaxIntensity("Max Intensity", Float) = 1.0
		_MinIntensity("Min Intensity", Float) = -1.0

		[KeywordEnum(Noise, Wave)] _VertexDisplacementType("Vertex Displacement type.", Float) = 0.0
		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_NOISE)] _DisplacementFactorMap("Displacement factor map", 2D) = "white" {}
		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_NOISE)] _WorldSpaceDirection("World space direction", Vector) = (1,1,1,1)
		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_NOISE)] _NoiseSpeed("Noise Speed", Float) = 1.0
		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_NOISE)] _NoiseFrequency("Noise frequency", Float) = 1.0

		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_WAVE)] _WaveMap("(R) Amplitude texture, (G) Speed texture, (B) Frequency texture", 2D) = "white" {}
		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_WAVE)] _MaxSpeed("Max speed", Float) = 1.0
		[ShowOnKeywordDrawer(_VERTEXDISPLACEMENTTYPE_WAVE)] _MaxFrequency("Max frequency", Float) = 1.0

		[Toggle(DIRECTION_TEXTURE)] _IsDirectionTexture("Direction texture?", Float) = 0.0
		[ShowOnKeywordDrawer(DIRECTION_TEXTURE)] _DirectionTexture("Direction texture", 2D) = "grey" {}

		/*
		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		*/
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		/*
		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]
		*/

		CGPROGRAM

		#pragma surface WaveNoiseSurf Standard addshadow vertex:WaveNoiseVert

		#pragma target 3.0

		#pragma shader_feature_local VERTEX_COLOR NORMAL_MAP EMISISON
		#pragma shader_feature_local VERTEX_DISPLACEMENT
		#pragma shader_feature_local _VERTEXDISPLACEMENTTYPE_NOISE _VERTEXDISPLACEMENTTYPE_WAVE
		#pragma shader_feature_local DIRECTION_TEXTURE

		#include "Assets/_Shader/WaveNoiseV2/WaveNoiseV2Include.cginc"
		ENDCG

	}

		CustomEditor "WaveNoiseV2ShaderGUI"
		FallBack "Diffuse"
}
