using AIObjects;
using InteractiveObjects;
using UnityEngine;

namespace SoliderAIBehavior
{
    /// <summary>
    /// Holds informations about the direction were the <see cref="TrackUnknownBehavior"/> will try to look something for.
    /// </summary>
    public class TrackUnknownInterestDirectionSystem
    {
        /// <summary>
        /// The direction were the <see cref="TrackUnknownBehavior"/> will try to look something for.
        /// </summary>
        public Vector3 WorldDirectionInterest { get; private set; }

        private CoreInteractiveObject AssociatedInteractiveObject;

        public TrackUnknownInterestDirectionSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
        }
        
        /// <summary>
        /// Sets the <see cref="WorldDirectionInterest"/> as the inverse forward direction of the <see cref="AssociatedInteractiveObject"/>.
        /// The forward direction is supposed to be the direction where FiredProjectile is coming to. 
        /// </summary>
        public void DamageDealt(CoreInteractiveObject damageDealerInteractiveObject)
        {
             this.WorldDirectionInterest = -damageDealerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward.normalized;
        }
    }
}