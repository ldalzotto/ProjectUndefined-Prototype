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
}
#endif