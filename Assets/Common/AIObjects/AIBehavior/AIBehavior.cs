using System;
using System.Collections.Generic;

namespace AIObjects
{
    /// <summary>
    /// A StateManager can be associated to only one State.
    /// It defines logic when the <see cref="StateManager"/> is currently active and when the State exit and enter.
    /// </summary>
    public abstract class StateManager
    {
        public virtual void Tick(float d)
        {
        }

        public virtual void OnStateEnter()
        {
        }

        public virtual void OnStateExit()
        {
        }
    }

    /// <summary>
    /// The AIBehavior holds informations about :
    /// <list type="bullet">
    ///    <item><description>The current State.</description></item>
    ///    <item><description>The lookup table between State and StateManagers</description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="S">The State enum type</typeparam>
    /// <typeparam name="SM">The base StateManager type</typeparam>
    public abstract class AIBehavior<S, SM> where S : Enum where SM : StateManager
    {
        private EnumVariable<S> CurrentState;
        protected Dictionary<S, SM> StateManagersLookup;

        protected AIBehavior(S StartState)
        {
            this.CurrentState = new EnumVariable<S>(StartState, this.OnStateChanged);
        }

        /// <summary>
        /// When the <see cref="CurrentState"/> changes,
        /// <see cref="StateManager.OnStateExit"/> and <see cref="StateManager.OnStateEnter"/> workflow methods are called. 
        /// </summary>
        private void OnStateChanged(S Old, S New)
        {
            if (!EqualityComparer<S>.Default.Equals(Old, New))
            {
                this.StateManagersLookup[Old].OnStateExit();
                this.StateManagersLookup[New].OnStateEnter();
            }
        }

        public virtual void Tick(float d)
        {
            this.StateManagersLookup[this.CurrentState.GetValue()].Tick(d);
        }

        public SM GetCurrentStateManager()
        {
            return this.StateManagersLookup[this.CurrentState.GetValue()];
        }

        public virtual void SetState(S NewState)
        {
            this.CurrentState.SetValue(NewState);
        }
        
        public virtual void OnDestroy(){}
    }
}