using UnityEngine;

namespace TextWindows
{
    public static class RectTransformHelper
    {
        public static void SetPivot(RectTransform RectTransform, PivotType PivotType)
        {
            switch (PivotType)
            {
                case PivotType.UpperLeft:
                    RectTransform.pivot = new Vector2(0, 1);
                    break;
                case PivotType.UpperMiddle:
                    RectTransform.pivot = new Vector2(0.5f, 1);
                    break;
                case PivotType.UpperRight:
                    RectTransform.pivot = new Vector2(1, 1);
                    break;
                case PivotType.MiddleLeft:
                    RectTransform.pivot = new Vector2(0, 0.5f);
                    break;
                case PivotType.MiddleMiddle:
                    RectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case PivotType.MiddleRight:
                    RectTransform.pivot = new Vector2(1, 0.5f);
                    break;
                case PivotType.BottomLeft:
                    RectTransform.pivot = new Vector2(0, 0);
                    break;
                case PivotType.BottomMiddle:
                    RectTransform.pivot = new Vector2(0.5f, 0);
                    break;
                case PivotType.BottomRight:
                    RectTransform.pivot = new Vector2(1f, 0);
                    break;
            }
        }
    }

    public enum PivotType
    {
        UpperLeft = 0,
        UpperMiddle = 1,
        UpperRight = 2,
        MiddleLeft = 3,
        MiddleMiddle = 4,
        MiddleRight = 5,
        BottomLeft = 6,
        BottomMiddle = 7,
        BottomRight = 8
    }
}