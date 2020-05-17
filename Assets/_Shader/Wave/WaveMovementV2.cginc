#ifndef WAVE_MOVEMENT
#define WAVE_MOVEMENT
struct WaveMovementDefinition {
	sampler2D waveDisplacementTexture;
	sampler2D waveMap;
	float maxAmplitude;
	float maxSpeed;
	float maxFrequency;
};


float3 WaveMovement(float2 texcoord, float3 vertex, float3 normal, WaveMovementDefinition waveDefinition) {
	float4 displacementSample = tex2Dlod(waveDefinition.waveDisplacementTexture, float4(texcoord.xy, 0, 0));
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range

	float4 waveMapSample = tex2Dlod(waveDefinition.waveMap, float4(texcoord.xy, 0, 0));

	float theta = dot(displacementSample.xyz, vertex.xyz);
	float amplitude = waveMapSample.r * waveDefinition.maxAmplitude;
	float speed = waveMapSample.g * waveDefinition.maxSpeed;
	float frequency = waveMapSample.b * waveDefinition.maxFrequency;

	float sinusWave = sin(theta * (2 / frequency) + (_Time.x* speed));

	float3 finalLocalPosition = float3(0, 0, 0);
	finalLocalPosition += vertex.xyz + (normal *  amplitude * sinusWave);

	return finalLocalPosition;
}

#endif // WAVE_MOVEMENT
