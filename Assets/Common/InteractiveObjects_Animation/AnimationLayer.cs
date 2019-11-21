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

    /// <summary>
    /// Animation layers prefixed with '*_{number}' are considered overrides of the '*_{number-1}' layer.
    /// </summary>
    public enum AnimationLayerID
    {
        LocomotionLayer,
        LocomotionLayer_1,
        ContextActionLayer
    }

    public static class AnimationLayerStatic
    {
        public static Dictionary<AnimationLayerID, AnimationLayer> AnimationLayers = new Dictionary<AnimationLayerID, AnimationLayer>()
        {
            {AnimationLayerID.LocomotionLayer, new AnimationLayer(0)},
            {AnimationLayerID.LocomotionLayer_1, new AnimationLayer(1)},
            {AnimationLayerID.ContextActionLayer, new AnimationLayer(10)}
        };
    }
}