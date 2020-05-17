using System;

namespace TextMesh
{
    public class GeneratedTextDimensions
    {
        private GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;

        public GeneratedTextDimensions(GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent)
        {
            this.GeneratedTextDimensionsComponent = GeneratedTextDimensionsComponent;
        }

        public float GetMaxWindowHeight()
        {
            return this.GeneratedTextDimensionsComponent.MaxWindowHeight;
        }

        public float GetWindowHeight(TextMesh TextMesh)
        {
            return TextMesh.GetMeshHeight();
        }

        public float GetWindowWidth(TextMesh TextMesh)
        {
            return TextMesh.GetMeshWidth();
        }

        public float GetMaxWindowWidth()
        {
            return this.GeneratedTextDimensionsComponent.MaxWindowWidth;
        }
    }

    [Serializable]
    public class GeneratedTextDimensionsComponent
    {
        public float MaxWindowWidth;
        public float MaxWindowHeight;
    }
}