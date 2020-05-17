﻿Shader "Custom/UI/Selection/SelectedWithMaskShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_MaskTexture("Mask Texture", 2D) = "white" {}
		_AuraMaskTexture("Aura Texture", 2D) = "white" {}
		_AuraAlphaFactor("Aura Alpha Factor", Float) = 1
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]


			Pass {
				Name "Aura"
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_CLIP_RECT
				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float2 masktextcoord : TEXCOORD2;
					float2 auramasktextcoord : TEXCOORD3;
					float4 worldPosition : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				sampler2D _MaskTexture;
				sampler2D _AuraMaskTexture;
				fixed4 _Color;
				fixed4 _GlowColor;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				float4 _MainTex_ST;
				float4 _MaskTexture_ST;
				float4 _AuraMaskTexture_ST;
				float _AuraAlphaFactor;

				v2f vert(appdata_t v)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.worldPosition = v.vertex;

					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

					OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					OUT.auramasktextcoord = TRANSFORM_TEX(v.texcoord, _AuraMaskTexture);
					OUT.color = v.color;
					return OUT;
				}

				fixed4 frag(v2f IN) : SV_Target
				{

					half4 inputImageColor = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
					half4 maskImageColor = (tex2D(_MaskTexture, IN.masktextcoord));
					half4 auraMaskImageColor = (tex2D(_AuraMaskTexture, IN.auramasktextcoord));
							
				half4 color = half4(0,0,0,0);
					if(auraMaskImageColor.a > 0.001) {
						color = saturate(inputImageColor * IN.color * maskImageColor);
						color = saturate(color + (auraMaskImageColor * abs(sin(_Time.x * 50)) * _AuraAlphaFactor * IN.color));
					}

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif

					return color;
				}
			ENDCG
			}

			Pass
			{
				Name "Main texture"
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				#pragma multi_compile __ UNITY_UI_CLIP_RECT
				#pragma multi_compile __ UNITY_UI_ALPHACLIP

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
					float2 masktextcoord : TEXCOORD2;
					float4 worldPosition : TEXCOORD1;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				sampler2D _MaskTexture;
				fixed4 _Color;
				fixed4 _GlowColor;
				fixed4 _TextureSampleAdd;
				float4 _ClipRect;
				float4 _MainTex_ST;
				float4 _MaskTexture_ST;

				v2f vert(appdata_t v)
				{
					v2f OUT;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
					OUT.worldPosition = v.vertex;

					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

					OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					OUT.masktextcoord = TRANSFORM_TEX(v.texcoord, _MaskTexture);
					OUT.color = v.color;
					return OUT;
				}

				fixed4 frag(v2f IN) : SV_Target
				{

					half4 inputImageColor = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
					half4 maskImageColor = (tex2D(_MaskTexture, IN.masktextcoord));
							
				half4 color = half4(0,0,0,0);
				color = saturate((inputImageColor * maskImageColor) * IN.color );

					#ifdef UNITY_UI_ALPHACLIP
					clip(color.a - 0.001);
					#endif

					return color;
				}
			ENDCG
			}
		}



}
