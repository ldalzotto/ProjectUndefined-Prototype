﻿using System;
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
        [VE_Ignore] private InteractiveInteractiveObjectPhysicsListener interactiveInteractiveObjectPhysicsListener;

        /// <summary>
        /// /!\ <see cref="CoreInteractiveObject"/> destroy event hook.
        /// This is the **ONLY** way to detect destroy of interactive object.
        /// This is to allow singular systems to have their own <see cref="CoreInteractiveObject"/> reference cleanup logic at InteractiveObject granularity. 
        /// </summary>
        private event Action<CoreInteractiveObject> OnInteractiveObjectDestroyedEvent;

        protected void BaseInit(IInteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true)
        {
            isAskingToBeDestroyed = false;
            this.IsUpdatedInMainManager = IsUpdatedInMainManager;
            InteractiveGameObject = interactiveGameObject;

            if (InteractiveGameObject != null)
            {
                if (InteractiveGameObject.Animator != null)
                {
                    this.AnimatorPlayable = new AnimatorPlayableObject(InteractiveGameObject.InteractiveGameObjectParent.name, InteractiveGameObject.Animator);
                }

                this.AnimationController = new AnimationController(InteractiveGameObject.Agent, this.AnimatorPlayable, InteractiveGameObject.PhysicsRigidbody, this.OnRootMotionEnabled, this.OnRootMotionDisabled);
            }


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
                if (this.interactiveInteractiveObjectPhysicsListener == null)
                {
                    this.interactiveInteractiveObjectPhysicsListener = this.InteractiveGameObject.LogicCollider.gameObject.AddComponent<InteractiveInteractiveObjectPhysicsListener>();
                    this.interactiveInteractiveObjectPhysicsListener.Init(this);
                }

                this.interactiveInteractiveObjectPhysicsListener.AddPhysicsEventListener(AInteractiveObjectPhysicsEventListener);
            }
        }

        public void RegisterInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action)
        {
            this.OnInteractiveObjectDestroyedEvent += action;
        }

        public void UnRegisterInteractiveObjectDestroyedEventListener(Action<CoreInteractiveObject> action)
        {
            this.OnInteractiveObjectDestroyedEvent -= action;
        }

        public virtual void FixedTick(float d)
        {
        }

        public virtual void FixedTickTimeFrozen(float d)
        {
        }

        protected void UpdateAniamtions(float d)
        {
            this.AnimatorPlayable?.Tick(d);
            this.AnimationController.Tick(d);
        }

        public virtual void Tick(float d)
        {
        }

        public virtual void AfterTicks(float d)
        {
        }

        public virtual void TickTimeFrozen(float d)
        {
        }

        public virtual void LateTick(float d)
        {
        }

        public virtual void Destroy()
        {
            this.AnimatorPlayable?.Destroy();
            this.interactiveInteractiveObjectPhysicsListener?.Destroy();

            this.OnInteractiveObjectDestroyedEvent?.Invoke(this);
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

        /// <summary>
        /// Will be included by the RangeSystem to calculate the Range obstacle occlusion.
        /// An obstacle is considered non traversable and act as an obstruction entity.
        /// Examples :
        ///     Firing range take into account obstacles to ensure that the projectile will reach it's target.
        ///     Projectiles are destroyed if entered in contact with obstacles.
        /// </summary>
        public bool IsObstacle;

        public bool IsPlayer;

        /// <summary>
        /// The associated interactive object is considered hostile to others. On trigger, it will
        /// try to damage other objects.
        /// Examples :
        ///     Deflection projectiles only delflects objects tagged.
        /// </summary>
        public bool IsDealingDamage;

        /// <summary>
        /// The associated interactive object is considered to interact with pprojectile and weapons.
        /// It generally have health. This means that damage can be taken and health can be recovered.
        /// Examples :
        ///     Projectiles are dealing damage to object tagged.
        ///     Health globes allow health recovery on object tagged.
        ///     The player targetting visual feedback takes into account this tag to position the visual feedback.
        /// </summary>
        public bool IsTakingDamage;
        
        public bool IsGivingHealth;
    }
}