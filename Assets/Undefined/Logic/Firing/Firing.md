# Firing

Firing workflow from shooting to receive damage.

````puml
skinparam node {
    backgroundColor<<InteractiveObject>> orange
    backgroundColor<<System>> yellow
}

interface AskToFireAFiredProjectile

package Shooter {
    node ShooterInteractiveObject <<InteractiveObject>>
    node WeaponHandlingSystem <<System>>
    node Weapon <<InteractiveObject>>
    AskToFireAFiredProjectile --> ShooterInteractiveObject
    ShooterInteractiveObject *-- WeaponHandlingSystem
    WeaponHandlingSystem *-- Weapon
}

package Projectile {
    node FiredProjectile <<InteractiveObject>>
    node DamageDealerEmitterSystem <<System>>
    FiredProjectile *-- DamageDealerEmitterSystem
}

package Receiver {
    node Enemy <<InteractiveObject>>
    node DamageDealerReceiverSystem <<System>>
}

ShooterInteractiveObject -> WeaponHandlingSystem : AskToFireAFiredProjectile
WeaponHandlingSystem -> Weapon : SpawnFiredProjectile
Weapon -> FiredProjectile : Create Projectile
Enemy -> DamageDealerEmitterSystem : Trigger Enter
DamageDealerEmitterSystem -> Enemy : DealDamage \n if IsTakingDamage tag
Enemy -> DamageDealerReceiverSystem : DealDamage
````