using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveObjects;
using SequencedAction;
using Targetting;
using UnityEditor;
using UnityEngine;

namespace Targetting_Test
{
    public class MoveTargetCursorToInteractiveObjectAction : ASequencedAction
    {
        private CoreInteractiveObject Target;

        public MoveTargetCursorToInteractiveObjectAction(CoreInteractiveObject Target, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.Target = Target;
        }

        private bool ended;
        private Coroutine Coroutine;

        public override void FirstExecutionAction()
        {
            this.ended = false;
            this.Coroutine = Coroutiner.Instance.StartCoroutine(this.LocalCoroutine());
        }

        public override bool ComputeFinishedConditions()
        {
            return this.ended;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }

        private IEnumerator LocalCoroutine()
        {
            TargetCursorManager.Get().SetTargetCursorPosition(Camera.main.WorldToScreenPoint(
                this.Target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.Target.InteractiveGameObject.AverageModelLocalBounds.Bounds.center)));
            yield return null;
            this.ended = true;
        }

        public override void Interupt()
        {
            base.Interupt();
            if (this.Coroutine != null)
            {
                Coroutiner.Instance.StopCoroutine(this.Coroutine);
            }
        }
    }
}