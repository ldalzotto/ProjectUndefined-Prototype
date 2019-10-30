using UnityEngine;
using System.Collections;

public class WaitForTransformPositionReached : CustomYieldInstruction
{
    private Transform transform;
    private Vector3 targetPosition;
    private float maxDelta;

    public WaitForTransformPositionReached(Transform transform, Vector3 targetPosition, float maxDelta)
    {
        this.transform = transform;
        this.targetPosition = targetPosition;
        this.maxDelta = maxDelta;
    }

    public override bool keepWaiting => Vector3.Distance(transform.position, targetPosition) > maxDelta;

}
