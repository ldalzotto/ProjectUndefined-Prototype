using System;
using AnimatorPlayable;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using Object = UnityEngine.Object;

namespace InteractiveObjects
{
    public abstract class CoreInteractiveObject
    {
        #region External Dependencies

        protected InteractiveObjectEventsManager InteractiveObjectEventsManager = InteractiveObjectEventsManager.Get();

        #endregion

        [VE_Nested] protected InteractiveObjectTag interactiveObjectTag;

        [VE_Ignore] protected bool isAskingToBeDestroyed;

        [VE_Ignore] public bool IsUpdatedInMainManager;

        protected void BaseInit(IInteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true)
        {
            isAskingToBeDestroyed = false;
            this.IsUpdatedInMainManager = IsUpdatedInMainManager;
            InteractiveGameObject = interactiveGameObject;
            if (InteractiveGameObject.Animator != null)
            {
                this.AnimatorPlayable = new AnimatorPlayableObject(InteractiveGameObject.InteractiveGameObjectParent.name, InteractiveGameObject.Animator);
            }

            this.AnimationController = new AnimationController(InteractiveGameObject.Agent, this.AnimatorPlayable, InteractiveGameObject.PhysicsRigidbody, this.OnRootMotionEnabled, this.OnRootMotionDisabled);

            this.Init();
            this.InteractiveObjectEventsManager.OnInteractiveObjectCreated(this);
        }

        public IInteractiveGameObject InteractiveGameObject { get; protected set; }

        public InteractiveObjectTag InteractiveObjectTag => interactiveObjectTag;

        public AnimatorPlayableObject AnimatorPlayable { get; protected set; }
        public AnimationController AnimationController { get; protected set; }
        public bool IsAskingToBeDestroyed => isAskingToBeDestroyed;

        public abstract void Init();

        public virtual void FixedTick(float d)
        {
        }

        public virtual void Tick(float d)
        {
            this.AnimatorPlayable?.Tick(d);
            this.AnimationController.Tick(d);
        }

        public virtual void AfterTicks(float d)
        {
        }

        public virtual void LateTick(float d)
        {
        }

        public virtual void Destroy()
        {
            this.AnimatorPlayable?.Destroy();
            InteractiveObjectEventsManager.OnInteractiveObjectDestroyed(this);
            Object.Destroy(InteractiveGameObject.InteractiveGameObjectParent);
        }

        #region Animation Object Events

        protected virtual void OnRootMotionEnabled()
        {
        }

        protected virtual void OnRootMotionDisabled()
        {
        }

        public virtual void OnAnimationObjectSetUnscaledSpeedMagnitude(float UnscaledSpeedMagnitude)
        {
        }

        #endregion

        #region AI Events

        public virtual void SetAIDestination(AIDestination AIDestination)
        {
        }

        public virtual void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
        }

        public virtual void OnAIDestinationReached()
        {
        }

        #endregion

        #region Attractive Object Events

        public virtual void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        #endregion

        #region Disarm Object Events

        public virtual void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        public virtual void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
        }

        #endregion
    }

    public class InteractiveObjectTag
    {
        public bool IsAi;
        public bool IsAttractiveObject;
        public bool IsLevelCompletionZone;
        public bool IsObstacle;
        public bool IsPlayer;
    }

    [Serializable]
    public struct InteractiveObjectTagStruct
    {
        public int IsAttractiveObject;
        public int IsAi;
        public int IsObstacle;
        public int IsPlayer;
        public int IsLevelCompletionZone;

        public InteractiveObjectTagStruct(int isAttractiveObject = -1, int isAi = -1, int isObstacle = -1, int isPlayer = -1, int isLevelCompletionZone = -1)
        {
            IsAttractiveObject = isAttractiveObject;
            IsAi = isAi;
            IsObstacle = isObstacle;
            IsPlayer = isPlayer;
            IsLevelCompletionZone = isLevelCompletionZone;
        }

        public bool Compare(InteractiveObjectTag InteractiveObjectTag)
        {
            return (IsAttractiveObject == -1 || IsAttractiveObject == Convert.ToInt32(InteractiveObjectTag.IsAttractiveObject))
                   && (IsAi == -1 || IsAi == Convert.ToInt32(InteractiveObjectTag.IsAi))
                   && (IsObstacle == -1 || IsObstacle == Convert.ToInt32(InteractiveObjectTag.IsObstacle))
                   && (IsPlayer == -1 || IsPlayer == Convert.ToInt32(InteractiveObjectTag.IsPlayer))
                   && (IsLevelCompletionZone == -1 || IsLevelCompletionZone == Convert.ToInt32(InteractiveObjectTag.IsLevelCompletionZone));
        }
    }
}