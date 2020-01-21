using Input;
using InteractiveObjectAction;

namespace PlayerObject
{
    public struct AimingInteractiveObjectActionStateBehaviorInputDataSystem
    {
        public GameInputManager GameInputManager;
        public InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem;
        public InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData;

        public AimingInteractiveObjectActionStateBehaviorInputDataSystem(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData)
        {
            GameInputManager = gameInputManager;
            InteractiveObjectActionPlayerSystem = interactiveObjectActionPlayerSystem;
            this.firingInteractiveObjectActionInherentData = firingInteractiveObjectActionInherentData;
        }
    }
}