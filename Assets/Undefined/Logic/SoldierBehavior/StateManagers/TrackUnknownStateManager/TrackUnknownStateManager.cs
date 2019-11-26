using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public enum TrackUnknownStateEnum
    {
        MOVE_TOWARDS_INTEREST_DIRECTION
    }

    public class TrackUnknownStateManager : SoldierStateManager
    {
        private TrackUnknownBehavior TrackUnknownBehavior;

        public TrackUnknownStateManager(CoreInteractiveObject associatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            TrackUnknownStateManagerExternalCallbacks TrackUnknownStateManagerExternalCallbacks)
        {
            this.TrackUnknownBehavior = new TrackUnknownBehavior(associatedInteractiveObject, SoldierAIBehaviorDefinition, TrackUnknownStateManagerExternalCallbacks);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.TrackUnknownBehavior.Tick(d);
        }

        #region External Health Events

        public override void DamageDealt(CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this.TrackUnknownBehavior.DamageDealt(DamageDealerInteractiveObject);
        }

        #endregion

        #region External Agent Events

        public override void OnDestinationReached()
        {
            base.OnDestinationReached();
            this.TrackUnknownBehavior.OnDestinationReached();
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="SoldierAIBehavior"/> will move in direction of interest direction calculated by <see cref="TrackUnknownInterestDirectionSystem"/>.
    /// </summary>
    public class TrackUnknownBehavior : AIBehavior<TrackUnknownStateEnum, SoldierStateManager>
    {
        private TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem;

        public TrackUnknownBehavior(CoreInteractiveObject associatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            TrackUnknownStateManagerExternalCallbacks TrackUnknownStateManagerExternalCallbacks) : base(TrackUnknownStateEnum.MOVE_TOWARDS_INTEREST_DIRECTION)
        {
            this.TrackUnknownInterestDirectionSystem = new TrackUnknownInterestDirectionSystem(associatedInteractiveObject);
            this.StateManagersLookup = new Dictionary<TrackUnknownStateEnum, SoldierStateManager>()
            {
                {TrackUnknownStateEnum.MOVE_TOWARDS_INTEREST_DIRECTION, new MoveTowardsInterestDirectionStateManager(associatedInteractiveObject, this.TrackUnknownInterestDirectionSystem, SoldierAIBehaviorDefinition, TrackUnknownStateManagerExternalCallbacks)}
            };
        }

        /// <summary>
        /// /!\ It is crucial to call <see cref="TrackUnknownInterestDirectionSystem"/> before propagating DamageDealt event because
        /// it may need data from <see cref="TrackUnknownInterestDirectionSystem"/>.
        /// </summary>
        /// <param name="DamageDealerInteractiveObject"></param>
        public void DamageDealt(CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this.TrackUnknownInterestDirectionSystem.DamageDealt(DamageDealerInteractiveObject);
            
            this.SetState(TrackUnknownStateEnum.MOVE_TOWARDS_INTEREST_DIRECTION);
            this.GetCurrentStateManager().DamageDealt(DamageDealerInteractiveObject);
        }

        public void OnDestinationReached()
        {
            this.GetCurrentStateManager().OnDestinationReached();
        }
    }
}