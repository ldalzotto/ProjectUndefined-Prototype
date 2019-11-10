using System;
using System.Collections.Generic;

namespace AIObjects
{
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

    public abstract class AIBehavior<S, SM> where S : Enum where SM : StateManager
    {
        private EnumVariable<S> CurrentState;
        protected Dictionary<S, SM> StateManagersLookup;

        protected AIBehavior(S StartState)
        {
            this.CurrentState = new EnumVariable<S>(StartState, this.OnStateChanged);
        }

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

        public S GetCurrentState()
        {
            return this.CurrentState.GetValue();
        }

        public virtual void SetState(S NewState)
        {
            this.CurrentState.SetValue(NewState);
        }
    }
}