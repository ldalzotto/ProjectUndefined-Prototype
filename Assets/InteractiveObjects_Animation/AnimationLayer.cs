using System.Collections.Generic;

namespace InteractiveObject_Animation
{
    public struct AnimationLayer
    {
        public int ID;

        public AnimationLayer(int id)
        {
            ID = id;
        }
    }

    public enum AnimationLayerID
    {
        LocomotionLayer,
        ContextActionLayer
    }

    public static class AnimationLayerStatic
    {
        public static Dictionary<AnimationLayerID, AnimationLayer> AnimationLayers = new Dictionary<AnimationLayerID, AnimationLayer>()
        {
            {AnimationLayerID.LocomotionLayer, new AnimationLayer(0)},
            {AnimationLayerID.ContextActionLayer, new AnimationLayer(10)}
        };
    }
}