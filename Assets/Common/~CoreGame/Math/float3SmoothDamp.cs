using Unity.Mathematics;

namespace CoreGame
{
    public static class float3SmoothDamp
    {
        public static float3 SmoothDamp(
            float3 current,
            float3 target,
            ref float3 currentVelocity,
            float smoothTime,
            float maxSpeed,
            float deltaTime)
        {
            smoothTime = math.max(0.0001f, smoothTime);
            float num1 = 2f / smoothTime;
            float num2 = num1 * deltaTime;
            float num3 = (float) (1.0 / (1.0 + (double) num2 + 0.479999989271164 * (double) num2 * (double) num2 + 0.234999999403954 * (double) num2 * (double) num2 * (double) num2));
            float num4 = current.x - target.x;
            float num5 = current.y - target.y;
            float num6 = current.z - target.z;
            float3 vector3 = target;
            float num7 = maxSpeed * smoothTime;
            float num8 = num7 * num7;
            float num9 = (float) ((double) num4 * (double) num4 + (double) num5 * (double) num5 + (double) num6 * (double) num6);
            if ((double) num9 > (double) num8)
            {
                float num10 = (float) math.sqrt((double) num9);
                num4 = num4 / num10 * num7;
                num5 = num5 / num10 * num7;
                num6 = num6 / num10 * num7;
            }

            target.x = current.x - num4;
            target.y = current.y - num5;
            target.z = current.z - num6;
            float num11 = (currentVelocity.x + num1 * num4) * deltaTime;
            float num12 = (currentVelocity.y + num1 * num5) * deltaTime;
            float num13 = (currentVelocity.z + num1 * num6) * deltaTime;
            currentVelocity.x = (currentVelocity.x - num1 * num11) * num3;
            currentVelocity.y = (currentVelocity.y - num1 * num12) * num3;
            currentVelocity.z = (currentVelocity.z - num1 * num13) * num3;
            float x = target.x + (num4 + num11) * num3;
            float y = target.y + (num5 + num12) * num3;
            float z = target.z + (num6 + num13) * num3;
            float num14 = vector3.x - current.x;
            float num15 = vector3.y - current.y;
            float num16 = vector3.z - current.z;
            float num17 = x - vector3.x;
            float num18 = y - vector3.y;
            float num19 = z - vector3.z;
            if ((double) num14 * (double) num17 + (double) num15 * (double) num18 + (double) num16 * (double) num19 > 0.0)
            {
                x = vector3.x;
                y = vector3.y;
                z = vector3.z;
                currentVelocity.x = (x - vector3.x) / deltaTime;
                currentVelocity.y = (y - vector3.y) / deltaTime;
                currentVelocity.z = (z - vector3.z) / deltaTime;
            }

            return new float3(x, y, z);
        }
    }
}