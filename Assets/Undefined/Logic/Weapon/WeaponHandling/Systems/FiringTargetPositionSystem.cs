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

        
        /// <summary>
        /// This is the optimum position local position of where objects should aim to hit the associated <see cref="CoreInteractiveObject"/>. 
        /// </summary>
        public Vector3 GetFiringTargetLocalPosition()
        {
            return this.FiringTargetPositionSystemDefinition.TargetPositionPointLocalOffset;
        }
    }
}