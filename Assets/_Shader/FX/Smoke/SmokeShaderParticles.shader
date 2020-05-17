Shader "FX/SmokeShaderParticles"
{
	Properties
	{
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[Toggle(SMOOTH_ALPHA_CUTOUT)] _SmoothAlphaCutout ("Smooth alpha cutout ?", Float) = 0
		[ShowOnKeyword(SMOOTH_ALPHA_CUTOUT)]	_AlphaCutSmooth("Alpha Cut Smooth", Range(0.0,1.0)) = 0.05
		_DeformationFactor("Deformation Factor", Range(0.0,5.0)) = 1.0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" }

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert fullforwardshadows alpha:blend
			#pragma shader_feature SMOOTH_ALPHA_CUTOUT
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "Assets/_Shader/Noise/noiseSimplex.cginc"
			
			half _Glossiness;
			half _Metallic;
			half _DeformationFactor;
			half _AlphaCutSmooth;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
				fixed4 custom : TEXCOORD0;
				fixed4 randomNbparticle : TEXCOORD1;
			};

			struct Input
			{
				float4 color : COLOR;
				fixed samplexNoiseValue01;
				float fixedRandom;
			};

			void vert(inout appdata v, out Input o) {
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float unNormalizedSamplexNoise = snoise(worldPos * v.randomNbparticle.x / 2);
				o.samplexNoiseValue01 = (unNormalizedSamplexNoise + 1)*0.5;
				o.fixedRandom = v.custom.x;
				o.color = v.color;
				v.vertex.xyz += unNormalizedSamplexNoise * _DeformationFactor;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = IN.color;
				o.Albedo = c.rgb; 
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				//noise
				//rim effect
				half cutoutAlpha = IN.fixedRandom;
#if SMOOTH_ALPHA_CUTOUT
				half maskA = smoothstep(cutoutAlpha - _AlphaCutSmooth, cutoutAlpha + _AlphaCutSmooth, IN.samplexNoiseValue01);
#else
				half maskA = step(cutoutAlpha, IN.samplexNoiseValue01);
#endif
				o.Alpha = maskA * IN.color.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
