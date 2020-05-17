using UnityEngine;

namespace SelectionWheel
{
    public static class SelectionWheelInstancer
    {
        public static GameObject CreateSelectionWheelGameObject(GameObject selectionWheelPrefab, Canvas parentCanvas)
        {
            return MonoBehaviour.Instantiate(selectionWheelPrefab, parentCanvas.transform);
        }
    }
}