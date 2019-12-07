using AIObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
#if UNITY_EDITOR
using InteractiveObjects;
using LevelManagement;
using Obstacle;
using UnityEngine;

public class TrainingLevelDebugComponent : MonoBehaviour
{
    public bool FireProjectileToInteractiveObject;
    public Collider LogicCollider;

    public bool SetPlayerPosition;
    public Transform TargetPlayerPosition;

    public bool DestroyAllObstacles;
    public bool RestartLevel;

    public bool RestoreHealth;
    public bool DeflectAllWithPlayerAsHolder;
    private void Update()
    {
        if (FireProjectileToInteractiveObject)
        {
            FireProjectileToInteractiveObject = false;
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider[this.LogicCollider].AskToFireAFiredProjectile_Forward();
        }

        if (SetPlayerPosition)
        {
            SetPlayerPosition = false;
            PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.SetDestination(new ForwardAgentMovementCalculationStrategy(new AIDestination() {Rotation = this.TargetPlayerPosition.rotation, WorldPosition = this.TargetPlayerPosition.position}));
        }

        if (DestroyAllObstacles)
        {
            DestroyAllObstacles = false;

            for (var i = ObstacleInteractiveObjectManager.Get().AllObstacleInteractiveObjects.Count - 1; i >= 0; i--)
            {
                var currentInteractiveObject = ObstacleInteractiveObjectManager.Get().AllObstacleInteractiveObjects[i];
               currentInteractiveObject.Destroy();
            }
            
        }

        if (RestartLevel)
        {
            LevelTransitionManager.Get().RestartCurrentLevel();
            RestartLevel = false;
        }

        if (RestoreHealth)
        {
            PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.DealDamage(40, null);
            RestoreHealth = false;
        }

        if (DeflectAllWithPlayerAsHolder)
        {
            foreach (var interactiveObject in InteractiveObjectV2Manager.Get().InteractiveObjects)
            {
                interactiveObject.DeflectProjectile(PlayerInteractiveObjectManager.Get().PlayerInteractiveObject);
            }
            DeflectAllWithPlayerAsHolder = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
        }
    }
}
#endif