#ifndef WAVE_NOISE_V2_VERTEX_DISPLACEMENT
#define WAVE_NOISE_V2_VERTEX_DISPLACEMENT

#if _VERTEXDISPLACEMENTTYPE_WAVE
#include "Assets/_Shader/Wave/WaveMovementV2.cginc"
#endif

float _MaxIntensity;
float _MinIntensity;

sampler2D _DisplacementFactorMap;
float4 _DisplacementFactorMap_ST;
float _NoiseSpeed;
float _NoiseFrequency;

sampler2D _WaveMap;
float _MaxSpeed;
float _MaxFrequency;

sampler2D _DirectionTexture;
float3 _WorldSpaceDirection;

void VertexDisplacementNoise(inout appdata_full v) {

	float3 finalVertexPosition = v.vertex.xyz;
#if _VERTEXDISPLACEMENTTYPE_NOISE
#if DIRECTION_TEXTURE
	float4 displacementSample = tex2Dlod(_DirectionTexture, float4(v.texcoord.xy, 0, 0));
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range
	float3 localDirection = normalize(displacementSample);
#else
	float3 localDirection = normalize(mul(unity_WorldToObject, _WorldSpaceDirection));
#endif

	float3 worldPosition = mul(unity_ObjectToWorld, v.vertex);
	float noiseIntensity = snoise(worldPosition * (1 / _NoiseFrequency) + (_Time.x *_NoiseSpeed));
	noiseIntensity = _MinIntensity + ((abs(_MinIntensity) + abs(_MaxIntensity)) * ((noiseIntensity + 1) / 2));

	float displacementDelta = (tex2Dlod(_DisplacementFactorMap, float4(v.texcoord.xy, 0, 0)).x * _MaxIntensity * noiseIntensity);
	float3 projectedDisplacementDelta = displacementDelta * localDirection;
	finalVertexPosition += projectedDisplacementDelta;
#endif

#if _VERTEXDISPLACEMENTTYPE_WAVE
	WaveMovementDefinition waveDefinition;
	waveDefinition.waveDisplacementTexture = _DirectionTexture;
	waveDefinition.waveMap = _WaveMap;
	waveDefinition.maxAmplitude = _MaxIntensity;
	waveDefinition.maxSpeed = _MaxSpeed;
	waveDefinition.maxFrequency = _MaxFrequency;
	finalVertexPosition = WaveMovement(v.texcoord.xy, finalVertexPosition, v.normal.xyz, waveDefinition);
#endif

	v.vertex.xyz = finalVertexPosition;
}

#endif // WAVE_NOISE_V2_VERTEX_DISPLACEMENT