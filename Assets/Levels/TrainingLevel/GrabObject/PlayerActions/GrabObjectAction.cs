using PlayerActions;

namespace RTPuzzle
{
    public class GrabObjectAction : PlayerAction
    {
        private PlayerAction addedPlayerAction;

        public GrabObjectAction(PlayerAction addedPlayerAction, CorePlayerActionDefinition CorePlayerActionDefinition) : base(CorePlayerActionDefinition)
        {
            this.addedPlayerAction = addedPlayerAction;
        }

        public override bool FinishedCondition()
        {
            return true;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            PlayerActionEntryPoint.Get().IncreaseOrAddActionsRemainingExecutionAmount(this.addedPlayerAction, 1);
            this.PlayerActionConsumed();
        }

        public override void Tick(float d)
        {
        }

        public override void GizmoTick()
        {
        }

        public override void GUITick()
        {
        }

        public override void LateTick(float d)
        {
        }
    }
}