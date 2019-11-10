using UnityEngine;

namespace Weapon
{
    public class FiringTargetPositionSystem
    {
        private FiringTargetPositionSystemDefinition FiringTargetPositionSystemDefinition;

        public FiringTargetPositionSystem(FiringTargetPositionSystemDefinition firingTargetPositionSystemDefinition)
        {
            FiringTargetPositionSystemDefinition = firingTargetPositionSystemDefinition;
        }

        public Vector3 GetFiringTargetLocalPosition()
        {
            return this.FiringTargetPositionSystemDefinition.TargetPositionPointLocalOffset;
        }
    }
}