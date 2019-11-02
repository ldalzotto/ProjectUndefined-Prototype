using System;
using InteractiveObjects_Interfaces;
using RangeObjects;

namespace InteractiveObjects
{
    public class AttractiveObjectSystem : AInteractiveObjectSystem
    {
        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public AttractiveObjectSystem(CoreInteractiveObject InteractiveObject, InteractiveObjectTag physicsInteractionSelectionGuard, AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition,
            Action<CoreInteractiveObject> onAttractiveSystemJustIntersected = null,
            Action<CoreInteractiveObject> onAttractiveSystemJustNotIntersected = null, Action<CoreInteractiveObject> onAttractiveSystemInterestedNothing = null)
        {
            SphereRange = new SphereRangeObjectV2(InteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization
                {
                    RangeTypeID = RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
                    IsTakingIntoAccountObstacles = true,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                    {
                        Radius = AttractiveObjectSystemDefinition.EffectRange
                    }
                }, InteractiveObject, InteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_AttractiveObjectRange");
            IsAskingTobedestroyed = false;

            SphereRange.RegisterIntersectionEventListener(new RangeIntersectionV2Listener_Delegated(SphereRange, physicsInteractionSelectionGuard, OnJustIntersectedAction: onAttractiveSystemJustIntersected,
                OnJustNotIntersectedAction: onAttractiveSystemJustNotIntersected, OnInterestedNothingAction: onAttractiveSystemInterestedNothing));
            AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectSystemDefinition.EffectiveTime);
        }

        #region Internal Dependencies

        public RangeObjectV2 SphereRange { get; set; }

        #endregion

        public bool IsAskingTobedestroyed { get; private set; }

        public override void Tick(float d)
        {
            AttractiveObjectLifetimeTimer.Tick(d);
            IsAskingTobedestroyed = AttractiveObjectLifetimeTimer.IsTimeOver();
        }

        public override void OnDestroy()
        {
            SphereRange.OnDestroy();
        }
    }

    internal class AttractiveObjectLifetimeTimer
    {
        private float effectiveTime;

        private float elapsedTime;

        public AttractiveObjectLifetimeTimer(float effectiveTime)
        {
            this.effectiveTime = effectiveTime;
        }

        #region Logical Condition

        public bool IsTimeOver()
        {
            return elapsedTime >= effectiveTime;
        }

        #endregion

        public void Tick(float d)
        {
            elapsedTime += d;
        }
    }
}