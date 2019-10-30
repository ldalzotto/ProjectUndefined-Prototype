using SelectableObjects_Interfaces;
using UnityEngine;
using UnityEngine.Rendering;

namespace SelectableObject
{
    internal class SelectableObjectRendererManager
    {
        private SelectableObjectsConfiguration SelectableObjectsConfiguration;
        private SelectableObjectIconAnimation SelectableObjectIconAnimation;

        private MaterialPropertyBlock SelectionDoticonMaterialProperty;

        public SelectableObjectRendererManager()
        {
            this.SelectableObjectsConfiguration = SelectableObjectsConfigurationGameObject.Get().SelectableObjectsConfiguration;
            CommandBufer = new CommandBuffer();
            CommandBufer.name = GetType().Name;

            //Camera.main.AddCommandBuffer(CameraEvent.AfterForwardAlpha, this.commandBufer);

            SelectableObjectIconAnimation = new SelectableObjectIconAnimation();
            SelectionDoticonMaterialProperty = new MaterialPropertyBlock();
        }

        public CommandBuffer CommandBufer { get; private set; }


        public void Tick(float d, ISelectableObjectSystem currentSelectedObject, bool hasMultipleAvailableSelectionObjects)
        {
            CommandBufer.Clear();

            if (currentSelectedObject != null)
            {
                SelectableObjectIconAnimation.Tick(d, hasMultipleAvailableSelectionObjects);
                var averageBoundsLocalSpace = currentSelectedObject.GetAverageModelBoundLocalSpace();

                if (!averageBoundsLocalSpace.IsNull())
                {
                    if (hasMultipleAvailableSelectionObjects)
                        SelectionDoticonMaterialProperty.SetTexture("_MainTex", SelectableObjectsConfiguration.SelectionDotSwitchIconTexture);
                    else
                        SelectionDoticonMaterialProperty.SetTexture("_MainTex", SelectableObjectsConfiguration.SelectionDotIconTexture);

                    var targetTransform = currentSelectedObject.GetTransform();

                    //icon
                    CommandBufer.DrawMesh(SelectableObjectsConfiguration.ForwardPlane,
                        Matrix4x4.TRS(targetTransform.position + Vector3.Project(new Vector3(0, averageBoundsLocalSpace.SideDistances.y * 0.5f, 0), targetTransform.up),
                            Quaternion.LookRotation(Camera.main.transform.position - targetTransform.position) * Quaternion.Euler(0, 0, SelectableObjectIconAnimation.GetRotationAngleDeg()), Vector3.one * SelectableObjectIconAnimation.GetIconScale()),
                        SelectableObjectsConfiguration.SelectionDoticonMaterial,
                        0, 0, SelectionDoticonMaterialProperty);
                }
            }
        }
    }
}