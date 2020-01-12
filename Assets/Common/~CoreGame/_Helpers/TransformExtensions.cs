using UnityEngine;

namespace CoreGame
{
    public static class TransformExtensions
    {
        public static void ResetLocal(this Transform transform)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static void ResetLocalPositionAndRotation(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static void ResetScale(this Transform transform)
        {
            transform.localScale = Vector3.one;
        }
    }

    public static class RectTransformExtensions
    {
        public static void Reset(this RectTransform rectTransform, RectTransformSetup RectTransformSetup)
        {
            switch (RectTransformSetup)
            {
                case RectTransformSetup.CENTER:
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    break;
                case RectTransformSetup.BOTTOM_LEFT:
                    rectTransform.anchorMax = new Vector2(0f, 0f);
                    rectTransform.anchorMin = new Vector2(0f, 0f);
                    break;
                case RectTransformSetup.BOTTOM_RIGHT:
                    rectTransform.anchorMax = new Vector2(1f, 0f);
                    rectTransform.anchorMin = new Vector2(1f, 0f);
                    break;
            }

            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }

        public static void SetPivot(this RectTransform rectTransform, RectTransformSetup RectTransformSetup)
        {
            switch (RectTransformSetup)
            {
                case RectTransformSetup.CENTER:
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case RectTransformSetup.BOTTOM_LEFT:
                    rectTransform.pivot = new Vector2(0f, 0f);
                    break;
                case RectTransformSetup.BOTTOM_RIGHT:
                    rectTransform.pivot = new Vector2(1f, 0f);
                    break;
            }
        }

        public static void SetSizeRelativeToScaler(this RectTransform rectTransform, Vector2 sizeRelativeToScaler, Canvas rootCanvas)
        {
            rectTransform.offsetMax = new Vector2(sizeRelativeToScaler.x * (rootCanvas.transform as RectTransform).sizeDelta.x, sizeRelativeToScaler.y * (rootCanvas.transform as RectTransform).sizeDelta.y);
            rectTransform.sizeDelta = new Vector2(sizeRelativeToScaler.x * (rootCanvas.transform as RectTransform).sizeDelta.x, sizeRelativeToScaler.y * (rootCanvas.transform as RectTransform).sizeDelta.y);
        }

        public static void SetLocalPositionRelativeToCanvasScaler(this RectTransform rectTransform, Vector2 positionPercentage)
        {
            rectTransform.localPosition = positionPercentage * CoreGameSingletonInstances.CanvasScaler().referenceResolution;
        }

        public static void SetWorldPositionRelativeToCanvasScaler(this RectTransform rectTransform, Vector2 positionPercentage)
        {
            rectTransform.position = positionPercentage * CoreGameSingletonInstances.CanvasScaler().referenceResolution;
        }
    }

    public enum RectTransformSetup
    {
        CENTER,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }
}