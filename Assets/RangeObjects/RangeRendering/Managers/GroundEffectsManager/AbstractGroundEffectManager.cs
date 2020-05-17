using System.Collections.Generic;
using Obstacle;

namespace RangeObjects
{
    public interface IAbstractGroundEffectManager
    {
        void OnRangeCreated(ARangeObjectRenderingDataProvider rangeObjectRenderingDataProvider);
        void Tick(float d, List<GroundEffectType> affectedGroundEffectsType);
        bool MeshMustBeRebuild();
        List<GroundEffectType> GroundEffectTypeToRender();
        ObstacleListenerSystem GetObstacleListener();
    }

    public abstract class AbstractGroundEffectManager : IAbstractGroundEffectManager
    {
        private List<GroundEffectType> groundEffectTypesToRender;
        private bool isGroundEffectTypeToRenderChanged;
        protected ARangeObjectRenderingDataProvider rangeObjectRenderingDataProvider;
        protected RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;

        public AbstractGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData)
        {
            this.rangeTypeInherentConfigurationData = rangeTypeInherentConfigurationData;
            isGroundEffectTypeToRenderChanged = false;
            groundEffectTypesToRender = new List<GroundEffectType>();
        }

        public void Tick(float d, List<GroundEffectType> affectedGroundEffectsType)
        {
            var involvedGroundEffectsType = new List<GroundEffectType>();

            if (affectedGroundEffectsType != null)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                    if (affectedGroundEffectType.MeshRenderer.isVisible
                        && rangeObjectRenderingDataProvider.BoundingCollider.bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                        involvedGroundEffectsType.Add(affectedGroundEffectType);

                isGroundEffectTypeToRenderChanged = false;
                if (involvedGroundEffectsType.Count != groundEffectTypesToRender.Count) isGroundEffectTypeToRenderChanged = true;

                if (!isGroundEffectTypeToRenderChanged)
                    foreach (var involvedGroundEffectType in involvedGroundEffectsType)
                        if (!groundEffectTypesToRender.Contains(involvedGroundEffectType))
                        {
                            isGroundEffectTypeToRenderChanged = true;
                            break;
                        }
            }
            else
            {
                isGroundEffectTypeToRenderChanged = true;
            }

            groundEffectTypesToRender = involvedGroundEffectsType;
        }

        public void OnRangeCreated(ARangeObjectRenderingDataProvider rangeObjectRenderingDataProvider)
        {
            this.rangeObjectRenderingDataProvider = rangeObjectRenderingDataProvider;
            Tick(0, null);
        }

        public bool MeshMustBeRebuild()
        {
            return isGroundEffectTypeToRenderChanged;
        }

        public List<GroundEffectType> GroundEffectTypeToRender()
        {
            return groundEffectTypesToRender;
        }

        public ObstacleListenerSystem GetObstacleListener()
        {
            return rangeObjectRenderingDataProvider.ObstacleListener;
        }

        public RangeTypeID GetRangeTypeID()
        {
            return rangeObjectRenderingDataProvider.RangeTypeID;
        }
    }
}