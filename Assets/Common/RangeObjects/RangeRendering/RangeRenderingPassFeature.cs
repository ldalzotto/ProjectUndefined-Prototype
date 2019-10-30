using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

namespace RangeObjects
{
    public class RangeRenderingPassFeature : ScriptableRendererFeature
    {
        private CustomRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass();
            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }

        private class CustomRenderPass : ScriptableRenderPass
        {
            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var RangeDrawCommand = GroundEffectsManagerV2.Get().RangeDrawCommand;
                if (RangeDrawCommand != null) context.ExecuteCommandBuffer(RangeDrawCommand);
            }
        }
    }
}