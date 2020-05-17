using OdinSerializer;
using UnityEngine;

namespace SelectableObject
{
    public class SelectableObjectsConfiguration : SerializedScriptableObject
    {
        [Header("Selection Materials")] public Mesh ForwardPlane;
        public Material SelectionDoticonMaterial;
        public Texture SelectionDotIconTexture;
        public Texture SelectionDotSwitchIconTexture;
    }
}