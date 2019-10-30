using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace SelectionWheel
{
    public class SelectionWheelGameObject
    {
        private GameObject selectionWheelGameObject;
        public GameObject SelectionWheelNodeContainerGameObject;

        public SelectionWheelGameObject(Canvas parentCanvas)
        {
            selectionWheelGameObject = Instanciate(parentCanvas);
        }

        public Animator Animator { get; private set; }

        private GameObject Instanciate(Canvas parentCanvas)
        {
            var selectionWheelGameObject = new GameObject(typeof(SelectionWheelGameObject).Name, typeof(RectTransform));
            selectionWheelGameObject.AddComponent<Image>();
            selectionWheelGameObject.AddComponent<CanvasRenderer>();

            var rectTransform = selectionWheelGameObject.transform as RectTransform;
            rectTransform.SetParent(parentCanvas.transform);
            rectTransform.Reset(RectTransformSetup.CENTER);

            rectTransform.SetSizeRelativeToScaler(new Vector2(0f, 0f), CoreGameSingletonInstances.GameCanvas);

            Animator = selectionWheelGameObject.AddComponent<Animator>();

            SelectionWheelNodeContainerGameObject = new GameObject(nameof(SelectionWheelNodeContainerGameObject), typeof(RectTransform));

            SelectionWheelNodeContainerGameObject.transform.parent = selectionWheelGameObject.transform;
            (SelectionWheelNodeContainerGameObject.transform as RectTransform).Reset(RectTransformSetup.CENTER);
            (SelectionWheelNodeContainerGameObject.transform as RectTransform).SetSizeRelativeToScaler(new Vector2(0f, 0f), CoreGameSingletonInstances.GameCanvas);


            return selectionWheelGameObject;
        }

        public void SetTransformPosition(Vector3 position)
        {
            selectionWheelGameObject.transform.position = position;
        }
    }
}