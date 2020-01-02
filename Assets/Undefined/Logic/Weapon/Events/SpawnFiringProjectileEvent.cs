using System;
using CoreGame;
using InteractiveObjects;

namespace Weapon
{
    public class SpawnFiringProjectileEvent : GameSingleton<SpawnFiringProjectileEvent>
    {
        public delegate void SpawnFiringProjectileEventDelegate(CoreInteractiveObject ProjectileObject, Weapon sourceWeapon);
        private event SpawnFiringProjectileEventDelegate evt;

        public void RegisterSpawnFiringProjectileEventListener(SpawnFiringProjectileEventDelegate SpawnFiringProjectileEventDelegate)
        {
            this.evt += SpawnFiringProjectileEventDelegate;
        }
        
        public void UnRegisterSpawnFiringProjectileEventListener(SpawnFiringProjectileEventDelegate SpawnFiringProjectileEventDelegate)
        {
            this.evt -= SpawnFiringProjectileEventDelegate;
        }
        
        public void OnFiringProjectileSpawned(CoreInteractiveObject ProjectileObject, Weapon sourceWeapon)
        {
            this.evt?.Invoke(ProjectileObject, sourceWeapon);
        }
    }
}