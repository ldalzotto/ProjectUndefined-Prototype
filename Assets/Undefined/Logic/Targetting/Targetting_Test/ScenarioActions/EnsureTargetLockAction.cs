using InteractiveObjects;
using SequencedAction;
using Targetting;
using Tests;
using UnityEngine;
using Weapon;

namespace Targetting_Test
{
    public class EnsureTargetLockAction : ASequencedAction
    {
        // TargetCursorManager.Get()
        private IEM_ProjectileFireActionInput_Retriever firingInteractiveObject;
        private CoreInteractiveObject target;

        public EnsureTargetLockAction(IEM_ProjectileFireActionInput_Retriever firingInteractiveObject, CoreInteractiveObject target)
        {
            this.firingInteractiveObject = firingInteractiveObject;
            this.target = target;
            this.TargetIsCorrect = false;
        }

        private bool TargetIsCorrect;

        public override void FirstExecutionAction()
        {
            TargetAndFireTestUtil.SetupInputForStartingFiring();
            this.Tick(0f);
        }

        public override bool ComputeFinishedConditions()
        {
            return this.TargetIsCorrect;
        }

        public override void AfterFinishedEventProcessed()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.SwitchSelectionButtonD = false;
        }

        public override void Tick(float d)
        {
            if (!this.target.IsAskingToBeDestroyed)
            {
                TargetCursorManager.Get().SetTargetCursorPosition(Camera.main.WorldToScreenPoint(
                    this.target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.target.GetFiringTargetLocalPosition())));
            }

            if (this.firingInteractiveObject.GetCurrentlyTargettedInteractiveObject() != target)
            {
                GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.SwitchSelectionButtonD = true;
            }
            else
            {
                GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.SwitchSelectionButtonD = false;
            }

            this.TargetIsCorrect = (this.firingInteractiveObject.GetCurrentlyTargettedInteractiveObject() == target);

            if (this.target.IsAskingToBeDestroyed)
            {
                this.TargetIsCorrect = true;
            }

            if (this.TargetIsCorrect)
            {
                TargetAndFireTestUtil.SetupInputForStoppingFiring();
            }
        }
    }
}