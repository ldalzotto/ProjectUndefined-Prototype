using UnityEngine;

public class ComponentSearchHelper
{
    public static void ComputeScaleFactorRecursively(Transform bottomTransform, Transform upTransform, ref Vector3 scaleFactor)
    {
        if (upTransform.GetInstanceID() != bottomTransform.GetInstanceID())
        {
            scaleFactor.Scale(bottomTransform.localScale);
            ComputeScaleFactorRecursively(bottomTransform.parent, upTransform, ref scaleFactor);
        }
        else
        {
            scaleFactor.Scale(upTransform.localScale);
        }
    }
}
