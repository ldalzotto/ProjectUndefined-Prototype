using PlayerObject;
#if UNITY_EDITOR
using InteractiveObjects;
using UnityEngine;

public class TrainingLevelDebugComponent : MonoBehaviour
{
    public bool FireProjectileToInteractiveObject;
    public Collider LogicCollider;

    private void Update()
    {
        if (FireProjectileToInteractiveObject)
        {
            FireProjectileToInteractiveObject = false;
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider[this.LogicCollider].AskToFireAFiredProjectile();
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