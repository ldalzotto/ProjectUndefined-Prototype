Shader "Custom/FX/Range/RangeShaderV5_Pass"
{
	Properties
	{
		_AlbedoBoost("Albedo Boost", Float) = 1.0
		_RangeMixFactor("Range Mix Factor", Range(0.0, 1.0)) = 0.5
	}

	HLSLINCLUDE
		#include "UnityCG.cginc"
		#include "RangeShaderV5_StructDefinition.hlsl"
		#include "RangeShaderV5_Calculations.hlsl"

	    struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 worldPos : TEXCOORD1;
		};


		uniform int _ExecutionOrderIndex;

		sampler2D _AuraTexture;
		float4 _AuraTexture_ST;

		sampler2D _WorldPositionBuffer;

		float _AlbedoBoost;
		float _RangeMixFactor;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			return o;
		}
	ENDHLSL

	SubShader
	{

		Tags{ "RenderType" = "Transparent" }
		Pass
		{
			Name "SphereBuffer"
			ZWrite Off
			Blend One One
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				CircleRangeBufferData rangeBuffer = CircleRangeBuffer[0];
				float calcDistance = abs(distance(worldPos, rangeBuffer.CenterWorldPosition));
				fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
				newCol = saturate(newCol) * _AlbedoBoost;

				int isInside = (calcDistance <= rangeBuffer.Radius);
				if (isInside) {
					if (rangeBuffer.OccludedByFrustums) {
						isInside = !PointIsOccludedByFrustumV2(worldPos);
					}
				}

				computeCol = lerp(computeCol, newCol, _RangeMixFactor * isInside);

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
			ENDHLSL
		}

		Pass
		{
			Name "BoxBuffer"
			ZWrite Off
			Blend One One
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				BoxRangeBufferData rangeBuffer = BoxRangeBuffer[0];
				fixed4 newCol = rangeBuffer.AuraColor;
				newCol = saturate(newCol) * _AlbedoBoost;

				int isInside = PointIsInsideAABB(worldPos, rangeBuffer.BoundingBoxMin, rangeBuffer.BoundingBoxMax);
				if (isInside) {
					isInside = PointInsideFrustumV2(worldPos, rangeBuffer);
				}
				
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * isInside);

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
		ENDHLSL
		}

		Pass
		{
			Name "FrustumBuffer"
			ZWrite Off
			Blend One One
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				FrustumRangeBufferData rangeBuffer = FrustumRangeBuffer[0];
				fixed4 newCol = rangeBuffer.AuraColor;
				newCol = saturate(newCol) * _AlbedoBoost;

				int isInside = PointInsideFrustumV2(worldPos, rangeBuffer);
				if (isInside) {
					if (rangeBuffer.OccludedByFrustums) {
						isInside = !PointIsOccludedByFrustumV2(worldPos);
					}
				}

				computeCol = lerp(computeCol, newCol, _RangeMixFactor * isInside);

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
		ENDHLSL
		}

		Pass
		{
			Name "RoundedFrustumBuffer"
			ZWrite Off
			Blend One One
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				RoundedFrustumRangeBufferData rangeBuffer = RoundedFrustumRangeBuffer[0];

				float calcDistance = abs(distance(worldPos, rangeBuffer.CenterWorldPosition));
				fixed4 newCol = rangeBuffer.AuraColor;
				newCol = newCol * _AlbedoBoost;

				int isInside = (calcDistance <= rangeBuffer.RangeRadius);
				if (isInside) {
					isInside = PointIsInsideAABB(worldPos, rangeBuffer.BoundingBoxMin, rangeBuffer.BoundingBoxMax);
					if (isInside) {
						isInside = PointInsideFrustumV2(worldPos, rangeBuffer);
						if (isInside) {
							if (rangeBuffer.OccludedByFrustums) {
								isInside = !PointIsOccludedByFrustumV2(worldPos);
							}
						}
					}
				}

				computeCol = lerp(computeCol, newCol, _RangeMixFactor * isInside);

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
		ENDHLSL
		}
	}
	
	FallBack "Diffuse"
}