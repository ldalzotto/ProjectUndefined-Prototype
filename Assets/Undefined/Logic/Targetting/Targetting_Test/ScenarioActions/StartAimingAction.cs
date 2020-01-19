using SequencedAction;

namespace Targetting_Test
{
    public class StartAimingAction : ASequencedAction
    {
        public override void FirstExecutionAction()
        {
            TargetAndFireTestUtil.SetupInputForStartAiming();
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }
    }

    public class StopAimingAction : ASequencedAction
    {
        public override void FirstExecutionAction()
        {
            TargetAndFireTestUtil.SetupInputForStopAiming();
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }
    }
}