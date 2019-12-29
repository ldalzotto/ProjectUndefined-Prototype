using System;
using PlayerObject_Interfaces;
using UnityEngine;

namespace PlayerActions
{
    [Serializable]
    [CreateAssetMenu(fileName = "DummyPlayerActionInherentData", menuName = "Test/DummyPlayerActionInherentData", order = 1)]
    public class DummyPlayerActionInherentData : PlayerActionInherentData
    {
        public override PlayerAction BuildPlayerAction(IPlayerInteractiveObject PlayerInteractiveObject, Action OnPlayerActionStartedCallback = null,
            Action OnPlayerActionEndCallback = null)
        {
            return new DummyPlayerAction(CorePlayerActionDefinition);
        }
    }

    public class DummyPlayerAction : PlayerAction
    {
        public DummyPlayerAction(CorePlayerActionDefinition CorePlayerActionDefinition) : base(CorePlayerActionDefinition)
        {
        }

        public override bool FinishedCondition()
        {
            return true || base.FinishedCondition();
        }

        public override void GizmoTick()
        {
        }

        public override void Dispose()
        {
            
        }

        public override void GUITick()
        {
        }

        public override void AfterPlayerTick(float d)
        {
            
        }

        public override void TickTimeFrozen(float d)
        {
            
        }

        public override void LateTick(float d)
        {
        }

        public override void BeforePlayerTick(float d)
        {
        }
    }
}