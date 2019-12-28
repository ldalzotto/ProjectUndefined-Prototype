using System;
using System.Collections.Generic;
using SequencedAction;
using Targetting;
using UnityEngine;

namespace Targetting_Test
{
    public struct MoveTargetCursorSmoothScreenPositionData
    {
        public Vector2 StartViewportPosition;
        public Vector2 EndViewportPosition;
        public float Speed;

        public Vector2 ScreenDirection { get; private set; }

        public MoveTargetCursorSmoothScreenPositionData(Vector2 startViewportPosition, Vector2 endViewportPosition, float speed)
        {
            StartViewportPosition = startViewportPosition;
            EndViewportPosition = endViewportPosition;
            Speed = speed;

            this.ScreenDirection = (this.EndViewportPosition - this.StartViewportPosition).normalized;
        }
    }

    public class MoveTargetCursorSmoothScreenPosition : ASequencedAction
    {
        private TargetCursorManager TargetCursorManager = TargetCursorManager.Get();
        private MoveTargetCursorSmoothScreenPositionData MoveTargetCursorSmoothScreenPositionData;

        private Vector2 CurrentTargetViewportPosition;

        public MoveTargetCursorSmoothScreenPosition(MoveTargetCursorSmoothScreenPositionData MoveTargetCursorSmoothScreenPositionData)
        {
            this.MoveTargetCursorSmoothScreenPositionData = MoveTargetCursorSmoothScreenPositionData;
        }

        public override void FirstExecutionAction()
        {
            this.TargetCursorManager.SetTargetCursorPosition(
                Camera.main.ViewportToScreenPoint(this.MoveTargetCursorSmoothScreenPositionData.StartViewportPosition));
            this.CurrentTargetViewportPosition = this.MoveTargetCursorSmoothScreenPositionData.StartViewportPosition;
        }

        public override bool ComputeFinishedConditions()
        {
            return Vector3.Distance(this.MoveTargetCursorSmoothScreenPositionData.EndViewportPosition, this.MoveTargetCursorSmoothScreenPositionData.StartViewportPosition)
                   <= Vector3.Distance(this.CurrentTargetViewportPosition, this.MoveTargetCursorSmoothScreenPositionData.StartViewportPosition);
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
            this.CurrentTargetViewportPosition += (this.MoveTargetCursorSmoothScreenPositionData.ScreenDirection * d * this.MoveTargetCursorSmoothScreenPositionData.Speed);
            this.TargetCursorManager.SetTargetCursorPosition(Camera.main.ViewportToScreenPoint(this.CurrentTargetViewportPosition));
        }
    }
}