Shader "Custom/FX/Range/InsideRangeShader"
{
    Properties
    {
		_PlaneYPos("Plane Y world pos", Float) = 0
		_BarThickness("Bar thickness", Float) = 1
		_BarColor("Bar color", Color) = (1,1,1,1)
		_BarDistance("Bar distance", Float) = 1
		_TranslationSpeed("Translation speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		Blend OneMinusDstColor One
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD0;
            };

			float _PlaneYPos;
			float _BarThickness;
			float _BarDistance;
			float _TranslationSpeed;
			float4 _BarColor;

            v2f vert (appdata v)
            {
                v2f o;
				o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float normalizedWorldPosition = fmod(abs(i.worldPos.y + _PlaneYPos + (_TranslationSpeed * _Time.x)) ,_BarDistance);
				float distanceFromPlane = distance(normalizedWorldPosition, 0);
				clip(-distanceFromPlane + _BarThickness);
                return _BarColor;
            }
            ENDCG
        }
    }
}
