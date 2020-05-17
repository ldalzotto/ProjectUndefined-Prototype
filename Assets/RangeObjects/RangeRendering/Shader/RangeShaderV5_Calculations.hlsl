#ifndef RANGE_CALCULATIONS
#define RANGE_CALCULATIONS

uniform StructuredBuffer<CircleRangeBufferData> CircleRangeBuffer;
uniform StructuredBuffer<BoxRangeBufferData> BoxRangeBuffer;
uniform StructuredBuffer<FrustumRangeBufferData> FrustumRangeBuffer;
uniform StructuredBuffer<RoundedFrustumRangeBufferData> RoundedFrustumRangeBuffer;

uniform StructuredBuffer<FrustumBufferData> FrustumBufferDataBuffer;
int _FrustumBufferDataBufferCount;

/*
 *     C5----C6
 *    / |    /|
 *   C1----C2 |
 *   |  C8  | C7
 *   | /    |/     C3->C7  Forward
 *   C4----C3
 */

int PointInsideFrustumV2(float3 comparisonPoint, float3 FC1, float3 FC2, float3 FC3, float3 FC4, float3 FC5,
				float3 normal1, float3 normal2, float3 normal3, float3 normal4, float3 normal5, float3 normal6) {
	int isInside = 1;
	isInside = (dot(normal1, comparisonPoint - FC1) > 0);
	if (!isInside) { return isInside; }
	isInside = (dot(normal2, comparisonPoint - FC1) > 0);
	if (!isInside) { return isInside; }
	isInside = (dot(normal3, comparisonPoint - FC2) > 0);
	if (!isInside) { return isInside; }
	isInside = (dot(normal4, comparisonPoint - FC3) > 0);
	if (!isInside) { return isInside; }
	isInside = (dot(normal5, comparisonPoint - FC4) > 0);
	if (!isInside) { return isInside; }
	isInside = (dot(normal6, comparisonPoint - FC5) > 0);
	if (!isInside) { return isInside; }
	return isInside;
}

int PointInsideFrustumV2(float3 comparisonPoint, FrustumRangeBufferData frustumRangeBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumRangeBufferData.FC1, frustumRangeBufferData.FC2, frustumRangeBufferData.FC3, frustumRangeBufferData.FC4,
		frustumRangeBufferData.FC5, frustumRangeBufferData.normal1, frustumRangeBufferData.normal2, frustumRangeBufferData.normal3, frustumRangeBufferData.normal4, frustumRangeBufferData.normal5, frustumRangeBufferData.normal6);
}

int PointInsideFrustumV2(float3 comparisonPoint, RoundedFrustumRangeBufferData frustumBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
		frustumBufferData.FC5, frustumBufferData.normal1, frustumBufferData.normal2, frustumBufferData.normal3, frustumBufferData.normal4, frustumBufferData.normal5, frustumBufferData.normal6);
}

int PointInsideFrustumV2(float3 comparisonPoint, FrustumBufferData frustumBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
		frustumBufferData.FC5, frustumBufferData.normal1, frustumBufferData.normal2, frustumBufferData.normal3, frustumBufferData.normal4, frustumBufferData.normal5, frustumBufferData.normal6);
}

int PointInsideFrustumV2(float3 comparisonPoint, BoxRangeBufferData frustumBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
		frustumBufferData.FC5, frustumBufferData.normal1, frustumBufferData.normal2, frustumBufferData.normal3, frustumBufferData.normal4, frustumBufferData.normal5, frustumBufferData.normal6);
}

int PointIsOccludedByFrustumV2(float3 comparisonPoint) {
	int isInsideFrustum = 0;
	for (int index = 0; (index < _FrustumBufferDataBufferCount); index++) {
		if (!isInsideFrustum) {
			isInsideFrustum = PointInsideFrustumV2(comparisonPoint, FrustumBufferDataBuffer[index]);
		}

	}
	return isInsideFrustum;
}

int PointIsInsideAABB(float3 comparisonPoint, float3 minBound, float3 maxBound) {
	return (comparisonPoint.x >= minBound.x && comparisonPoint.x <= maxBound.x)
		&& (comparisonPoint.y >= minBound.y && comparisonPoint.y <= maxBound.y)
		&& (comparisonPoint.z >= minBound.z && comparisonPoint.z <= maxBound.z);
}

#endif