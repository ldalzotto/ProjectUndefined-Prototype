using System;
using InteractiveObjects;
using PlayerObject;
using UnityEngine;

namespace SoliderAIBehavior
{
    /// <summary>
    /// Holds and update all informations relative to the PlayerObject for the associated <see cref="SoliderEnemy"/>.
    /// It dispatch <see cref="SoldierStateManager.OnPlayerObjectJustOnSight"/> and <see cref="SoldierStateManager.OnPlayerObjectJustOutOfSight"/> events.
    /// </summary>
    public class PlayerObjectStateDataSystem
    {
        private CoreInteractiveObject playerObject;

        public CoreInteractiveObject PlayerObject()
        {
            if (this.playerObject == null)
            {
                this.playerObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
            }

            return this.playerObject;
        }

        public LastPlayerSeenPosition LastPlayerSeenPosition { get; private set; }
        public bool IsPlayerInSight { get; private set; }
        private SoldierAIBehavior SoldierAIBehaviorRef;

        private Action<CoreInteractiveObject> OnPlayerObjectJustOnSightAction;
        private Action<CoreInteractiveObject> OnPlayerObjectJustOutOfSightAction;

        public PlayerObjectStateDataSystem(Action<CoreInteractiveObject> OnPlayerObjectJustOnSightAction, Action<CoreInteractiveObject> OnPlayerObjectJustOutOfSightAction)
        {
            this.OnPlayerObjectJustOnSightAction = OnPlayerObjectJustOnSightAction;
            this.OnPlayerObjectJustOutOfSightAction = OnPlayerObjectJustOutOfSightAction;
        }

        public void Tick(float d)
        {
            if (this.IsPlayerInSight)
            {
                this.LastPlayerSeenPosition = new LastPlayerSeenPosition(this.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition, 
                 Quaternion.Euler(this.PlayerObject().InteractiveGameObject.GetTransform().WorldRotationEuler));
            }
        }

        public void OnInteractiveObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            if (InSightInteractiveObject.InteractiveObjectTag.IsPlayer)
            {
                this.IsPlayerInSight = true;
                this.OnPlayerObjectJustOnSightAction.Invoke(InSightInteractiveObject);
            }
        }

        public void OnInteractiveObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            if (NotInSightInteractiveObject.InteractiveObjectTag.IsPlayer)
            {
                this.IsPlayerInSight = false;
                this.OnPlayerObjectJustOutOfSightAction.Invoke(NotInSightInteractiveObject);
            }
        }
    }

    public struct LastPlayerSeenPosition
    {
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;

        public LastPlayerSeenPosition(Vector3 worldPosition, Quaternion worldRotation)
        {
            WorldPosition = worldPosition;
            WorldRotation = worldRotation;
        }
    }
}