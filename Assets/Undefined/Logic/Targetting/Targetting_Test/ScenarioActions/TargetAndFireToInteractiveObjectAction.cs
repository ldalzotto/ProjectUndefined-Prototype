﻿using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveObjects;
using SequencedAction;
using Targetting;
using Tests;
using UnityEngine;

namespace Targetting_Test
{
    public class TargetAndFireToInteractiveObjectAction : ASequencedAction
    {
        private CoreInteractiveObject Target;
        
        public TargetAndFireToInteractiveObjectAction(CoreInteractiveObject Target, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.Target = Target;
        }

        private bool ended;
        private Coroutine Coroutine;
        
        public override void FirstExecutionAction()
        {
            this.ended = false;
         this.Coroutine=   Coroutiner.Instance.StartCoroutine(this.ActionCoroutine());
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

        private IEnumerator ActionCoroutine()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionReleased = false;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDown = true;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringPorjectileDH = true;
            TargetCursorManager.Get().SetTargetCursorPosition(Camera.main.WorldToScreenPoint(
                this.Target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.Target.InteractiveGameObject.AverageModelLocalBounds.Bounds.center)));
            yield return null;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionReleased = true;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDown = false;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringPorjectileDH = false;
            this.ended = true;
        }

        public override void Interupt()
        {
            base.Interupt();
            Coroutiner.Instance.StopCoroutine(this.Coroutine);
        }
    }
}