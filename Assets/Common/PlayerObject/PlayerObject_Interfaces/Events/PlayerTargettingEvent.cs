using System;
using AnimatorPlayable;
using CoreGame;

namespace PlayerObject_Interfaces
{
    public class PlayerStartTargettingEvent : GameSingleton<PlayerStartTargettingEvent>
    {
        private Action<A_AnimationPlayableDefinition> OnPlayerStartTargettingEvent;

        public void RegisterOnPlayerStartTargettingEvent(Action<A_AnimationPlayableDefinition> action)
        {
            this.OnPlayerStartTargettingEvent += action;
        }

        public void UnRegisterOnPlayerStartTargettingEvent(Action<A_AnimationPlayableDefinition> action)
        {
            this.OnPlayerStartTargettingEvent -= action;
        }

        public void OnPlayerStartTargetting(A_AnimationPlayableDefinition StartTargettingPoseAnimation)
        {
            this.OnPlayerStartTargettingEvent?.Invoke(StartTargettingPoseAnimation);
        }
    }
    
    public class PlayerStoppedTargettingEvent : GameSingleton<PlayerStoppedTargettingEvent>
    {
        private Action OnPlayerStoppedTargettingEvent;

        public void RegisterOnPlayerStoppedTargettingEvent(Action action)
        {
            this.OnPlayerStoppedTargettingEvent += action;
        }

        public void UnRegisterOnPlayerStoppedTargettingEvent(Action action)
        {
            this.OnPlayerStoppedTargettingEvent -= action;
        }

        public void OnPlayerStoppedTargetting()
        {
            this.OnPlayerStoppedTargettingEvent?.Invoke();
        }
    }
}