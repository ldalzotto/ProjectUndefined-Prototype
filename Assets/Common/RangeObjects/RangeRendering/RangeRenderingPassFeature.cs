using UnityEngine.Rendering;


namespace RangeObjects
{
    public class RangeRenderingPassFeature : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {
        private CustomRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass();
            // Configures where the render pass should be injected.
            m_ScriptablePass.renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingOpaques;
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            renderer.EnqueuePass(m_ScriptablePass);
        }

        private class CustomRenderPass : UnityEngine.Rendering.Universal.ScriptableRenderPass
        {
            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
            {
                var RangeDrawCommand = GroundEffectsManagerV2.Get().RangeDrawCommand;
                if (RangeDrawCommand != null) context.ExecuteCommandBuffer(RangeDrawCommand);
            }
        }
    }
}