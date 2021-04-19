using System;
using System.Collections.Generic;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    /// <summary>
    /// Sending intersection events based on intersection calculation result of <see cref="RangeIntersectionCalculatorV2Manager"/>.
    /// </summary>
    public class RangeIntersectionV2System : ARangeObjectSystem
    {
        public RangeIntersectionV2System(RangeObjectV2 rangeObjectV2Ref) : base(rangeObjectV2Ref)
        {
        }

        public List<AInteractiveIntersectionListener> RangeIntersectionListeners { get; private set; } = null;

        public override void Tick(float d)
        {
            if (RangeIntersectionListeners != null)
                for (var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
                    RangeIntersectionListeners[RangeIntersectionListenerIndex].Tick();
        }

        public void RegisterIntersectionEventListener(AInteractiveIntersectionListener aInteractiveIntersectionListener,
            InteractiveInteractiveObjectPhysicsListener associatedInteractiveInteractiveObjectPhysicsListener)
        {
            if (RangeIntersectionListeners == null) RangeIntersectionListeners = new List<AInteractiveIntersectionListener>();

            associatedInteractiveInteractiveObjectPhysicsListener.AddPhysicsEventListener(aInteractiveIntersectionListener);
            RangeIntersectionListeners.Add(aInteractiveIntersectionListener);
        }

        public void OnDestroy()
        {
            if (RangeIntersectionListeners != null)
                for (var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
                    RangeIntersectionListeners[RangeIntersectionListenerIndex].OnDestroy();
        }
    }

    /// <summary>
    /// The interactive intersection listener instanciate range intersection calculation object when another range object is PhysicsTriggerEnter or PhysicsTriggerExit.
    /// Two ranges that are triggering doesn't necessary mean that they interset because their range may be smaller that the trigger volume. That is why an intersection calculation is generated.
    /// </summary>
    public abstract class AInteractiveIntersectionListener : AInteractiveObjectPhysicsEventListener
    {
        protected RangeObjectV2 associatedRangeObject;
        protected List<RangeIntersectionCalculator> intersectionCalculators = new List<RangeIntersectionCalculator>();
        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculator> intersectionCalculatorsIndexedByTrackedInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculator>();

        private List<RangeIntersectionCalculator> justTriggerExitedCalculators = new List<RangeIntersectionCalculator>();
        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculator> justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculator>();

        protected AInteractiveIntersectionListener(RangeObjectV2 associatedRangeObject)
        {
            this.associatedRangeObject = associatedRangeObject;
        }

        protected virtual void OnJustIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
        }

        protected virtual void OnJustNotIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
        }

        protected virtual void OnInterestedNothing(RangeIntersectionCalculator intersectionCalculator)
        {
        }

        protected virtual void OnTriggerEnterSuccess(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
        }

        protected virtual void OnTriggerExitSuccess(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
        }

        public virtual void OnDestroy()
        {
        }

        public void Tick()
        {
            foreach (var intersectionCalculator in intersectionCalculators) SingleCalculation(intersectionCalculator);

            for (var i = justTriggerExitedCalculators.Count - 1; i >= 0; i--) SingleJustExited(justTriggerExitedCalculators[i]);
        }

        public sealed override void OnTriggerEnter(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            PhysicsTriggerInfo.OtherInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            var rangeIntersectionCalculator = new RangeIntersectionCalculator(associatedRangeObject, PhysicsTriggerInfo.OtherInteractiveObject);
            intersectionCalculators.Add(rangeIntersectionCalculator);
            intersectionCalculatorsIndexedByTrackedInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject] = rangeIntersectionCalculator;
            OnTriggerEnterSuccess(PhysicsTriggerInfo);
        }

        public sealed override void OnTriggerExit(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = intersectionCalculatorsIndexedByTrackedInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject];

            if (rangeIntersectionCalculator.IsInside)
            {
                justTriggerExitedCalculators.Add(rangeIntersectionCalculator);
                justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Add(PhysicsTriggerInfo.OtherInteractiveObject, rangeIntersectionCalculator);
            }

            intersectionCalculators.Remove(rangeIntersectionCalculator);
            intersectionCalculatorsIndexedByTrackedInteractiveObject.Remove(PhysicsTriggerInfo.OtherInteractiveObject);
            OnTriggerExitSuccess(PhysicsTriggerInfo);
        }

        private void SingleCalculation(RangeIntersectionCalculator intersectionCalculator)
        {
            var intersectionOperation = intersectionCalculator.Tick();
            if (intersectionOperation == InterserctionOperationType.JustInteresected)
                OnJustIntersected(intersectionCalculator);
            else if (intersectionOperation == InterserctionOperationType.JustNotInteresected)
                OnJustNotIntersected(intersectionCalculator);
            else if (intersectionOperation == InterserctionOperationType.IntersectedNothing) OnInterestedNothing(intersectionCalculator);
        }

        private void SingleJustExited(RangeIntersectionCalculator justTriggerExitedRangeIntersectionCalculator)
        {
            //Debug.Log("JustTriggeredExit : " + justTriggerExitedCalculatorIndex + " " + this.justTriggerExitedCalculators.Count);
            OnJustNotIntersected(justTriggerExitedRangeIntersectionCalculator);
            justTriggerExitedRangeIntersectionCalculator.OnDestroy();
            justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Remove(justTriggerExitedRangeIntersectionCalculator.TrackedInteractiveObject);
            justTriggerExitedRangeIntersectionCalculator.TrackedInteractiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            justTriggerExitedCalculators.Remove(justTriggerExitedRangeIntersectionCalculator);
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject InteractiveObjectRefToRemove)
        {
            intersectionCalculatorsIndexedByTrackedInteractiveObject.TryGetValue(InteractiveObjectRefToRemove, out var RangeIntersectionCalculatorV2ToRemove);
            if (RangeIntersectionCalculatorV2ToRemove != null)
            {
                RangeIntersectionCalculatorV2ToRemove.OnDestroy();
                intersectionCalculators.Remove(RangeIntersectionCalculatorV2ToRemove);
                intersectionCalculatorsIndexedByTrackedInteractiveObject.Remove(InteractiveObjectRefToRemove);
            }

            justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.TryGetValue(InteractiveObjectRefToRemove, out var JustTriggeredExitRangeIntersectionCalculatorV2);

            if (JustTriggeredExitRangeIntersectionCalculatorV2 != null)
            {
                JustTriggeredExitRangeIntersectionCalculatorV2.OnDestroy();
                justTriggerExitedCalculators.Remove(JustTriggeredExitRangeIntersectionCalculatorV2);
                justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Remove(InteractiveObjectRefToRemove);
            }
        }
    }

    public class InteractiveIntersectionListenerDelegated : AInteractiveIntersectionListener
    {
        protected Func<InteractiveObjectTag, bool> InteractiveObjectSelectionGuardAction;
        private Action<CoreInteractiveObject> OnInterestedNothingAction = null;
        private Action<CoreInteractiveObject> OnJustIntersectedAction = null;
        private Action<CoreInteractiveObject> OnJustNotIntersectedAction = null;
        private Action<CoreInteractiveObject> OnTriggerEnterSuccessAction = null;
        private Action<CoreInteractiveObject> OnTriggerExitSuccessAction = null;

        public InteractiveIntersectionListenerDelegated(RangeObjectV2 associatedRangeObject, Func<InteractiveObjectTag, bool> InteractiveObjectSelectionGuardAction,
            Action<CoreInteractiveObject> OnInterestedNothingAction = null, Action<CoreInteractiveObject> OnJustIntersectedAction = null, Action<CoreInteractiveObject> OnJustNotIntersectedAction = null,
            Action<CoreInteractiveObject> OnTriggerEnterSuccessAction = null, Action<CoreInteractiveObject> OnTriggerExitSuccessAction = null) : base(associatedRangeObject)
        {
            this.OnInterestedNothingAction = OnInterestedNothingAction;
            this.OnJustIntersectedAction = OnJustIntersectedAction;
            this.OnJustNotIntersectedAction = OnJustNotIntersectedAction;
            this.OnTriggerEnterSuccessAction = OnTriggerEnterSuccessAction;
            this.OnTriggerExitSuccessAction = OnTriggerExitSuccessAction;
            this.InteractiveObjectSelectionGuardAction = InteractiveObjectSelectionGuardAction;
        }

        public override bool ColliderSelectionGuard(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            return this.InteractiveObjectSelectionGuardAction.Invoke(interactiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag());
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculator intersectionCalculator)
        {
            if (OnInterestedNothingAction != null) OnInterestedNothingAction.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnJustIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
            if (OnJustIntersectedAction != null) OnJustIntersectedAction.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
            if (OnJustNotIntersectedAction != null) OnJustNotIntersectedAction.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnTriggerEnterSuccess(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            if (OnTriggerEnterSuccessAction != null) OnTriggerEnterSuccessAction.Invoke(interactiveObjectPhysicsTriggerInfo.OtherInteractiveObject);
        }

        protected override void OnTriggerExitSuccess(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            if (OnTriggerExitSuccessAction != null) OnTriggerExitSuccessAction.Invoke(interactiveObjectPhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}