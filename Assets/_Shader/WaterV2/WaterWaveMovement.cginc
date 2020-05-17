#ifndef WATER_WAVE_MOVEMENT
#define WATER_WAVE_MOVEMENT

#include "Assets/_Shader/Wave/WaveMovement.cginc"

sampler2D _WaveDisplacementTexture, _WaterWaveMap;
float  _MaxAmplitude, _MaxSpeed, _MaxFrequency;

void Displace(inout VertexInput v) {
	/*
	float2 tex = TRANSFORM_TEX(v.uv0, _MainTex);
	float4 displacementSample = tex2Dlod(_WaveDisplacementTexture, float4(tex.xy, 0, 0));	
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range

	float4 waterMapSample = tex2Dlod(_WaterWaveMap, float4(tex.xy, 0,0));

	*/
	WaveMovementDefinition waveDefinition;

	waveDefinition.waveDisplacementTexture = _WaveDisplacementTexture;
	waveDefinition.waveMap = _WaterWaveMap;
	waveDefinition.maxAmplitude = _MaxAmplitude;
	waveDefinition.maxSpeed = _MaxSpeed;
	waveDefinition.maxFrequency = _MaxFrequency;

	v.vertex.xyz = WaveMovement(v, waveDefinition);
}

#endif // WATER_WAVE_MOVEMENT
