Shader "Unlit/OutlineShader"
{
    Properties
    {
        _OutlineColor("Outline color", Color) = (1,1,1,1)
        _OutlineLocalDistance("Outline factor", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        ZTest Always
        //ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            half _OutlineLocalDistance;
            half3 _OutlineColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xyz += (v.normal.xyz * _OutlineLocalDistance);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_OutlineColor, 1);
            }
            ENDCG
        }
    }
}
