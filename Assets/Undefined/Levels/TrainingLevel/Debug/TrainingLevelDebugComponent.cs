using AIObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
#if UNITY_EDITOR
using InteractiveObjects;
using Obstacle;
using UnityEngine;

public class TrainingLevelDebugComponent : MonoBehaviour
{
    public bool FireProjectileToInteractiveObject;
    public Collider LogicCollider;

    public bool SetPlayerPosition;
    public Transform TargetPlayerPosition;

    public bool DestroyAllObstacles;

    private void Update()
    {
        if (FireProjectileToInteractiveObject)
        {
            FireProjectileToInteractiveObject = false;
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider[this.LogicCollider].AskToFireAFiredProjectile();
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
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            var playerGameObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.InteractiveGameObject;
            GizmoHelper.DrawBox(playerGameObject.GetLocalToWorld(), playerGameObject.AverageModelBounds.Bounds.center, playerGameObject.AverageModelBounds.Bounds, Color.blue, "Player", MyEditorStyles.LabelGreen);
        }
    }
}
#endif