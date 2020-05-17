#include "Assets/_Shader/Noise/noiseSimplex.cginc"

#if VERTEX_DISPLACEMENT
#include "Assets/_Shader/WaveNoiseV2/WaveNoiseV2VertexDisplacement.cginc"
#endif

struct Input
{
	float4 vertexColor : COLOR;
#if NORMAL_MAP
	float2 uv_BumpMap;
#endif
};

float4 _Color;
float3 _EmissionColor;

half _Glossiness;
half _Metallic;

sampler2D _BumpMap;

void WaveNoiseVert(inout appdata_full v) {
#if VERTEX_DISPLACEMENT
	VertexDisplacementNoise(v);
#endif
}

void WaveNoiseSurf(Input IN, inout SurfaceOutputStandard o)
{
	o.Albedo = 
#if VERTEX_COLOR
		IN.vertexColor *
#endif
		_Color.xyz;

#if NORMAL_MAP
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
#endif

#if EMISISON
	o.Emission = _EmissionColor;
#endif

	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;
	o.Alpha = IN.vertexColor.w * _Color.w;
}