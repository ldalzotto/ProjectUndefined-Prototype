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
        [VE_Ignore] private InteractiveInteractiveObjectPhysicsListener _interactiveInteractiveObjectPhysicsListener;

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

        public void RegisterInteractiveObjectPhysicsEventListener(AInteractiveObjectPhysicsEventListener AInteractiveObjectPhysicsEventListener)
        {
            if (this.InteractiveGameObject.LogicCollider != null)
            {
                if (this._interactiveInteractiveObjectPhysicsListener == null)
                {
                    this._interactiveInteractiveObjectPhysicsListener = this.InteractiveGameObject.LogicCollider.gameObject.AddComponent<InteractiveInteractiveObjectPhysicsListener>();
                    this._interactiveInteractiveObjectPhysicsListener.Init(this);
                }

                this._interactiveInteractiveObjectPhysicsListener.AddPhysicsEventListener(AInteractiveObjectPhysicsEventListener);
            }
        }

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
            this._interactiveInteractiveObjectPhysicsListener?.Destroy();
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

    [Serializable]
    public struct InteractiveObjectTag
    {
        public bool IsAttractiveObject;
        public bool IsAi;
        public bool IsObstacle;
        public bool IsPlayer;
    }
}