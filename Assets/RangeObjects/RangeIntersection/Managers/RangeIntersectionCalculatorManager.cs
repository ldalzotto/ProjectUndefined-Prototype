using System.Collections.Generic;
using CoreGame;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace RangeObjects
{
    /// <summary>
    /// Assign a unique ID to <see cref="RangeIntersectionCalculator"/> objects and track them all.
    /// </summary>
    public class RangeIntersectionCalculatorManager : GameSingleton<RangeIntersectionCalculatorManager>
    {
        public List<RangeIntersectionCalculator> AllRangeIntersectionCalculatorV2 = new List<RangeIntersectionCalculator>();
        private int CurrentRangeIntersectionCalculatorV2ManagerCounter = 0;

        public int OnRangeIntersectionCalculatorV2ManagerCreation(RangeIntersectionCalculator rangeIntersectionCalculator)
        {
            this.AllRangeIntersectionCalculatorV2.Add(rangeIntersectionCalculator);
            this.CurrentRangeIntersectionCalculatorV2ManagerCounter += 1;
            return this.CurrentRangeIntersectionCalculatorV2ManagerCounter;
        }

        public void OnRangeIntersectionCalculatorV2ManagerDestroyed(RangeIntersectionCalculator rangeIntersectionCalculator)
        {
            this.AllRangeIntersectionCalculatorV2.Remove(rangeIntersectionCalculator);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.AllRangeIntersectionCalculatorV2.Clear();
        }

        public void GizmoTick()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                foreach (var RangeIntersectionCalculatorV2 in this.AllRangeIntersectionCalculatorV2)
                {
                    RangeIntersectionCalculationManagerV2.Get().TryGetRangeintersectionResult(RangeIntersectionCalculatorV2, out bool isInsideRange);
                    Color lineColor = isInsideRange ? Color.green : Color.red;
                    var oldColor = Handles.color;
                    Handles.color = lineColor;
                    Handles.DrawLine(RangeIntersectionCalculatorV2.GetAssociatedRangeObject().GetTransform().WorldPosition, RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition);
                    Handles.color = oldColor;
                }
            }
#endif
        }
    }
}