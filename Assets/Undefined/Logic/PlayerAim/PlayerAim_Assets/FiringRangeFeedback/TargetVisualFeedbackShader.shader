Shader "Unlit/TargetVisualFeedbackShader"
{
    Properties
    {
       [HDR] _Color("Color", Color) = (1,1,1,1)
        _AttenuationPower("Attenuation power", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            half _AttenuationPower;
CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(_Color.xyz, pow(i.uv.y, _AttenuationPower));
            }
            ENDCG
        }
    }
}
