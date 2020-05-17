using UnityEngine;

namespace VisualFeedback
{
    public interface ILinePositioning
    {
        Vector3 GetEndPosition(Vector3 startPosition);
    }
}