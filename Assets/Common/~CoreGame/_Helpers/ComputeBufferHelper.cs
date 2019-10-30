using UnityEngine;

namespace CoreGame
{
    public static class ComputeBufferHelper
    {
        public static void SafeCommandBufferReleaseAndDispose(ComputeBuffer computeBuffer)
        {
            if (computeBuffer != null && computeBuffer.IsValid())
            {
                computeBuffer.Release();
                computeBuffer.Dispose();
            }
        }
    }

}
