#ifndef RANGE_STRUCT_DEFINITION
#define RANGE_STRUCT_DEFINITION

struct CircleRangeBufferData {
	float3 CenterWorldPosition;
	float Radius;
	float4 AuraColor;
	int OccludedByFrustums;
};

struct BoxRangeBufferData
{
	float3 FC1;
	float3 FC2;
	float3 FC3;
	float3 FC4;
	float3 FC5;

	float3 normal1;
	float3 normal2;
	float3 normal3;
	float3 normal4;
	float3 normal5;
	float3 normal6;

	float3 BoundingBoxMax;
	float3 BoundingBoxMin;
	
	float4 AuraColor;
};

struct FrustumRangeBufferData
{
	float3 FC1;
	float3 FC2;
	float3 FC3;
	float3 FC4;
	float3 FC5;

	float3 normal1;
	float3 normal2;
	float3 normal3;
	float3 normal4;
	float3 normal5;
	float3 normal6;

	int OccludedByFrustums;
	float4 AuraColor;
};

struct RoundedFrustumRangeBufferData
{
	float3 FC1;
	float3 FC2;
	float3 FC3;
	float3 FC4;
	float3 FC5;

	float3 normal1;
	float3 normal2;
	float3 normal3;
	float3 normal4;
	float3 normal5;
	float3 normal6;

	float3 BoundingBoxMax;
	float3 BoundingBoxMin;

	float RangeRadius;
	float3 CenterWorldPosition;

	int OccludedByFrustums;
	float4 AuraColor;
};

struct FrustumBufferData
{
	float3 FC1;
	float3 FC2;
	float3 FC3;
	float3 FC4;
	float3 FC5;
	float3 FC6;
	float3 FC7;
	float3 FC8;

	float3 normal1;
	float3 normal2;
	float3 normal3;
	float3 normal4;
	float3 normal5;
	float3 normal6;
};

#endif