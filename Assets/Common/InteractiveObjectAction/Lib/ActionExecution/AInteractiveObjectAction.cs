using System;
using UnityEngine.Serialization;

namespace InteractiveObjectAction
{
    /// <summary>
    /// <see cref="AInteractiveObjectAction"/> are an expansion of the InteractiveObject. <br/>
    /// They allow to execute custom logic (with game loop callbacks) aside the associated InteractiveObject own logic. <br/>
    /// AInteractiveObjectAction execution can be constrained with cooldown defined in <see cref="CoreInteractiveObjectActionDefinition.CooldownEnabled"/>.
    /// /!\ <see cref="AInteractiveObjectAction"/> instances must not be created with it's constructor, it must be called from the <see cref="PlayerActionInherentData"/> workflow <see cref="PlayerActionInherentData.BuildPlayerAction"/>
    /// </summary>
    public abstract class AInteractiveObjectAction
    {
        public CoreInteractiveObjectActionDefinition CoreInteractiveObjectActionDefinition { get; private set; }
        public abstract string InteractiveObjectActionUniqueID { get; }

        public bool IsAborted { get; private set; }

        #region WorkflowEvents callback

        /// <summary>
        /// Callback called when <see cref="FirstExecution"/> is called.
        /// </summary>
        private Action OnPlayerActionStartedCallback;

        /// <summary>
        /// Callback called when <see cref="Dispose"/> is called.
        /// </summary>
        private Action OnPlayerActionEndCallback;

        #endregion

        /// <summary>
        /// /// /!\ <see cref="AInteractiveObjectAction"/> instances must not be created with it's constructor, it must be called from the <see cref="PlayerActionInherentData"/> workflow <see cref="PlayerActionInherentData.BuildPlayerAction"/>
        /// </summary>
        protected AInteractiveObjectAction(CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition, Action OnPlayerActionStartedCallback = null,
            Action OnPlayerActionEndCallback = null)
        {
            this.CoreInteractiveObjectActionDefinition = coreInteractiveObjectActionDefinition;

            this.IsAborted = false;

            this.OnPlayerActionStartedCallback = OnPlayerActionStartedCallback;
            this.OnPlayerActionEndCallback = OnPlayerActionEndCallback;
        }

        public virtual bool FinishedCondition()
        {
            return this.IsAborted;
        }

        public virtual void FixedTick(float d)
        {
        }

        public abstract void Tick(float d);
        public abstract void AfterTicks(float d);
        public abstract void TickTimeFrozen(float d);
        public abstract void LateTick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

        public virtual void Dispose()
        {
            this.OnPlayerActionEndCallback?.Invoke();
        }

        public virtual void FirstExecution()
        {
            this.OnPlayerActionStartedCallback?.Invoke();
        }

        public void Abort()
        {
            this.Dispose();
            this.IsAborted = true;
        }

        #region Logical Conditions

        public bool CooldownFeatureEnabled()
        {
            return this.CoreInteractiveObjectActionDefinition.CooldownEnabled;
        }

        public bool MovementAllowed()
        {
            return this.CoreInteractiveObjectActionDefinition.MovementAllowed;
        }

        #endregion
    }

    public interface IInteractiveObjectActionInput
    {
    }
}