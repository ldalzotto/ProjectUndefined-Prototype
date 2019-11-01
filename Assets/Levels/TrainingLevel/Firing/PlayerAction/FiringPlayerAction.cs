using PlayerActions;
using UnityEngine;

namespace Firing
{
    public class FiringPlayerAction : PlayerAction
    {
        private FiringPlayerActionInherentData FiringPlayerActionInherentData;

        public FiringPlayerAction(FiringPlayerActionInherentData FiringPlayerActionInherentData) : base(FiringPlayerActionInherentData.CorePlayerActionDefinition)
        {
            this.FiringPlayerActionInherentData = FiringPlayerActionInherentData;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            Debug.Log(this.GetType().Name);
        }

        public override bool FinishedCondition()
        {
            return true;
        }

        public override void Tick(float d)
        {
        }

        public override void LateTick(float d)
        {
        }

        public override void GUITick()
        {
        }

        public override void GizmoTick()
        {
        }
    }
}