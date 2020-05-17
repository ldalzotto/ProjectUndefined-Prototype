using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace RangeObjects
{
    [SceneHandleDraw]
    public abstract class RangeObjectInitialization : SerializedScriptableObject
    {
        public bool IsTakingIntoAccountObstacles;

        [CustomEnum()] public RangeTypeID RangeTypeID;
    }

    public static class RangeObjectInitializationDataBuilderV2
    {
        public static SphereRangeObjectInitialization SphereRangeWithObstacleListener(float sphereRange, RangeTypeID rangeTypeID)
        {
            return new SphereRangeObjectInitialization
            {
                RangeTypeID = rangeTypeID,
                IsTakingIntoAccountObstacles = true,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = sphereRange
                }
            };
        }

        public static BoxRangeObjectInitialization BoxRangeNoObstacleListener(Vector3 center, Vector3 size, RangeTypeID rangeTypeID)
        {
            return new BoxRangeObjectInitialization
            {
                RangeTypeID = rangeTypeID,
                IsTakingIntoAccountObstacles = false,
                BoxRangeTypeDefinition = new BoxRangeTypeDefinition
                {
                    Center = center,
                    Size = size
                }
            };
        }
    }
}