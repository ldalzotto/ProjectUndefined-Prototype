Shader "Unlit/CircleBar"
{
	Properties
	{
		_MaskTexture("Mask Texture", 2D) = "white" {}
		_EmptyColor("Empty Color", Color) = (0,0,0,1)
		_FillColor("Fill Color", Color) = (1,1,1,1)
		_Progression("Progression", Range(0.0, 1.0)) = 0.0
	}
		SubShader
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#define PI 3.1415926
				#include "UnityCG.cginc"

				float4 _EmptyColor;
				float4 _FillColor;
				float _Progression;

				sampler2D _MaskTexture;
				float4  _MaskTexture_ST;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 localPos : TEXCOORD1;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MaskTexture);
					o.localPos = v.vertex;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 maskColor = tex2D(_MaskTexture, i.uv);
					float ang = (((atan2(i.localPos.x , -i.localPos.y) + lerp(0 , -2 * PI , _Progression)) / PI) + 1) / 2;
					return fixed4(fixed3((_FillColor.xyz * (ang <= 0)) + (_EmptyColor.xyz * (ang > 0))), maskColor.r * 0.75);
				}
				ENDCG
			}
		}
}
