using System.Collections.Generic;
using CoreGame;
using UnityEngine;
using UnityEngine.Rendering;

namespace VisualFeedback
{
    public class CircleFillBarRendererManager : GameSingleton<CircleFillBarRendererManager>
    {
        #region External Dependencies

        private CircleFillBarConfiguration CircleFillBarConfiguration;

        #endregion

        private List<CircleFillBarType> CircleFillBarTypeToRender = new List<CircleFillBarType>();

        public CommandBuffer CommandBuffer { get; private set; }
        private MaterialPropertyBlock materialProperty;

        public void Init()
        {
            this.CommandBuffer = new CommandBuffer();
            this.CommandBuffer.name = this.GetType().Name;

            this.materialProperty = new MaterialPropertyBlock();

            this.CircleFillBarConfiguration = CircleFillBarConfigurationGameObject.Get().CircleFillBarConfiguration;
        }


        public void Tick(float d)
        {
            this.CommandBuffer.Clear();

            foreach (var circleFillBarType in this.CircleFillBarTypeToRender)
            {
                if (circleFillBarType.CurrentProgression != 0f)
                {
                    this.materialProperty.SetFloat(Shader.PropertyToID("_Progression"), circleFillBarType.CurrentProgression);
                    this.CommandBuffer.DrawMesh(this.CircleFillBarConfiguration.ForwardQuadMesh, circleFillBarType.transform.localToWorldMatrix, this.CircleFillBarConfiguration.CircleProgressionMaterial, 0, 0, materialProperty);
                }
            }
        }

        #region External Event

        public void OnCircleFillBarTypeCreated(CircleFillBarType CircleFillBarTypeRef)
        {
            this.CircleFillBarTypeToRender.Add(CircleFillBarTypeRef);
        }

        public void OnCircleFillBarTypeDestroyed(CircleFillBarType CircleFillBarTypeRef)
        {
            this.CircleFillBarTypeToRender.Remove(CircleFillBarTypeRef);
        }

        #endregion
    }
}