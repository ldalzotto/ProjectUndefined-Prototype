using OdinSerializer;

namespace ProjectileDeflection
{
    [SceneHandleDraw]
    public class ProjectileDeflectionDefinition : SerializedScriptableObject
    {
        /// <summary>
        /// This radius is the radius of the sphere (where center is the center of the logic collider) that is used
        /// to detect projectile objects to deflect
        /// </summary>
        [WireCircleWorldAttribute(B = 1f, UseTransform = true, PositionFieldName = nameof(ProjectileDetectionRadius))]
        public float ProjectileDetectionRadius;
    }
}