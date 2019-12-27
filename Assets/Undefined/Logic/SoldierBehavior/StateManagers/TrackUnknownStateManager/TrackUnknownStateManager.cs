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
        [VE_Nested] private TrackUnknownStateBehavior _trackUnknownStateBehavior;

        public TrackUnknownStateManager(CoreInteractiveObject associatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            SoldierAIBehaviorExternalCallbacksV2 SoldierAIBehaviorExternalCallbacksV2,
            Action AskTrackUnknownStateManagerToExitAction)
        {
            this._trackUnknownStateBehavior = new TrackUnknownStateBehavior();
            this._trackUnknownStateBehavior.Init(associatedInteractiveObject, SoldierAIBehaviorDefinition, SoldierAIBehaviorExternalCallbacksV2, AskTrackUnknownStateManagerToExitAction);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this._trackUnknownStateBehavior.Tick(d);
        }

        #region External Health Events

        public override void DamageDealt(CoreInteractiveObject DamageDealerInteractiveObject)
        {
            this._trackUnknownStateBehavior.DamageDealt(DamageDealerInteractiveObject);
        }

        #endregion

        #region External Agent Events

        public override void OnDestinationReached()
        {
            base.OnDestinationReached();
            this._trackUnknownStateBehavior.OnDestinationReached();
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="SoldierStateBehavior"/> will move in direction of interest direction calculated by <see cref="TrackUnknownInterestDirectionSystem"/>.
    /// </summary>
    public class TrackUnknownStateBehavior : StateBehavior<TrackUnknownStateEnum, SoldierStateManager>
    {
        private TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem;

        public void Init(CoreInteractiveObject associatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            SoldierAIBehaviorExternalCallbacksV2 SoldierAIBehaviorExternalCallbacksV2,
            Action AskTrackUnknownStateManagerToExitAction)
        {
            this.TrackUnknownInterestDirectionSystem = new TrackUnknownInterestDirectionSystem(associatedInteractiveObject);
            this.StateManagersLookup = new Dictionary<TrackUnknownStateEnum, SoldierStateManager>()
            {
                {TrackUnknownStateEnum.MOVE_TOWARDS_INTEREST_DIRECTION, new MoveTowardsInterestDirectionStateManager(associatedInteractiveObject, this.TrackUnknownInterestDirectionSystem, SoldierAIBehaviorDefinition, SoldierAIBehaviorExternalCallbacksV2, AskTrackUnknownStateManagerToExitAction)}
            };
            
            base.Init(TrackUnknownStateEnum.MOVE_TOWARDS_INTEREST_DIRECTION);
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