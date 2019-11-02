using System;
using AnimatorPlayable;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using Object = UnityEngine.Object;

namespace InteractiveObjects
{
    public abstract partial class CoreInteractiveObject
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

        #endregion
    }

    /// <summary>
    /// -1 is used to exclude in comparison
    /// </summary>
    [Serializable]
    public struct InteractiveObjectTag
    {
        public int IsAttractiveObject;
        public int IsAi;
        public int IsObstacle;
        public int IsPlayer;

        public InteractiveObjectTag(int isAttractiveObject = -1, int isAi = -1, int isObstacle = -1, int isPlayer = -1)
        {
            IsAttractiveObject = isAttractiveObject;
            IsAi = isAi;
            IsObstacle = isObstacle;
            IsPlayer = isPlayer;
        }

        public bool Compare(InteractiveObjectTag InteractiveObjectTag)
        {
            return (IsAttractiveObject == -1 || IsAttractiveObject == InteractiveObjectTag.IsAttractiveObject)
                   && (IsAi == -1 || IsAi == InteractiveObjectTag.IsAi)
                   && (IsObstacle == -1 || IsObstacle == InteractiveObjectTag.IsObstacle)
                   && (IsPlayer == -1 || IsPlayer == InteractiveObjectTag.IsPlayer);
        }
    }
}