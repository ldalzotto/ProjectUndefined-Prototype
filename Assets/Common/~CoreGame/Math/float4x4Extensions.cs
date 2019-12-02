using Unity.Mathematics;

namespace CoreGame
{
    public static class float4x4Extensions
    {
        public static float3 MultiplyPoint(this float4x4 matrix, float3 point)
        {
            float3 vector3;
            vector3.x = (float) ((double) matrix[0][0] * (double) point.x + (double) matrix[1][0] * (double) point.y + (double) matrix[2][0] * (double) point.z) + matrix[3][0];
            vector3.y = (float) ((double) matrix[0][1]* (double) point.x + (double) matrix[1][1] * (double) point.y + (double) matrix[2][1] * (double) point.z) + matrix[3][1];
            vector3.z = (float) ((double) matrix[0][2] * (double) point.x + (double) matrix[1][2] * (double) point.y + (double) matrix[2][2] * (double) point.z) + matrix[3][2];
            float num = 1f / ((float) ((double) matrix[0][3] * (double) point.x + (double) matrix[1][3] * (double) point.y + (double) matrix[2][3] * (double) point.z) + matrix[3][3]);
            vector3.x *= num;
            vector3.y *= num;
            vector3.z *= num;
            return vector3;
        }
        public static float3 MultiplyVector(this float4x4 matrix, float3 vector)
        {
            float3 vector3;
            vector3.x = (float) ((double) matrix[0][0] * (double) vector.x + (double) matrix[1][0] * (double) vector.y + (double) matrix[2][0] * (double) vector.z);
            vector3.y = (float) ((double) matrix[0][1] * (double) vector.x + (double) matrix[1][1] * (double) vector.y + (double) matrix[2][1] * (double) vector.z);
            vector3.z = (float) ((double) matrix[0][2] * (double) vector.x + (double) matrix[1][2] * (double) vector.y + (double) matrix[2][2] * (double) vector.z);
            return vector3;
        }
    }
}