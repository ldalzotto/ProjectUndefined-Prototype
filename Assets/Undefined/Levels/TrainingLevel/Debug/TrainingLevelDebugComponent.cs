using AIObjects;
using InteractiveObjects_Interfaces;
#if UNITY_EDITOR
using InteractiveObjects;
using UnityEngine;

public class TrainingLevelDebugComponent : MonoBehaviour
{
    public bool FireProjectileToInteractiveObject;
    public Collider LogicCollider;

    public bool SetDestinationForward;
    public Collider Source;
    public Collider Target;
    public Vector3 Destination;

    private void Update()
    {
        if (FireProjectileToInteractiveObject)
        {
            FireProjectileToInteractiveObject = false;
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider[this.LogicCollider].AskToFireAFiredProjectile();
        }

        if (SetDestinationForward)
        {
            SetDestinationForward = false;
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider[this.Source].SetDestination(new LookingAtAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = Destination},
                InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider[this.Target]));
        }
    }
}
#endif