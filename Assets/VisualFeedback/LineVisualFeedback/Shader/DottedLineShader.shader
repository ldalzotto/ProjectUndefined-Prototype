Shader "Unlit/DottedLineShader"
{
	Properties
	{
		_ColorPointPosition("_ColorPointPosition", Vector) = (0,0,0,0)
		_MovingWidth("_MovingWidth", Float) = 1
		[HDR] _MovingColor("_MovingColor", Color) = (0,0,0,0)
		[HDR] _BaseColor("_BaseColor", Color) = (0,0,0,0)
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float3 _ColorPointPosition;
			float3 _MovingColor;
			float3 _BaseColor;
			float _MovingWidth;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
				float4 vertex : POSITION;
				float4 worldPosition : TEXCOORD0;
            };
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float dist = distance(i.worldPosition, _ColorPointPosition);
			    return (smoothstep(0, _MovingWidth, dist) * fixed4(_BaseColor, 0.5)) + ( (1 - smoothstep(0, _MovingWidth, dist)) * fixed4(_MovingColor, 0.5));
            }
            ENDCG
        }
    }
}
