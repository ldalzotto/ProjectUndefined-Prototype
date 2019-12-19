using System;
using CoreGame;

namespace ProjectileDeflection_Interface
{
    public class OnProjectileDeflectionAttemptEvent : GameSingleton<OnProjectileDeflectionAttemptEvent>
    {
        private event Action evt;

        public void RegisterOnProjectileDeflectionAttemptEventListener(Action OnProjectileDeflectionAttempt)
        {
            this.evt += OnProjectileDeflectionAttempt;
        }

        public void UnRegisterOnProjectileDeflectionAttemptEvent(Action OnProjectileDeflectionAttempt)
        {
            this.evt -= OnProjectileDeflectionAttempt;
        }

        public void OnProjectileDeflectionAttempt()
        {
            this.evt?.Invoke();
        }
    }
}