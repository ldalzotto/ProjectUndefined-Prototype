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
    public bool SetPlayerPosition;
    public Transform TargetPlayerPosition;

    public bool DestroyAllObstacles;
    public bool RestartLevel;

    public bool RestoreHealth;

    private void Update()
    {
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
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
        }
    }
}
#endif