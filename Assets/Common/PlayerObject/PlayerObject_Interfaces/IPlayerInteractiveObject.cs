using AnimatorPlayable;
using InteractiveObjects_Interfaces;

namespace PlayerObject_Interfaces
{
    public interface IPlayerInteractiveObject
    {
        IInteractiveGameObject InteractiveGameObject { get; }

        void OnPlayerStartTargetting(A_AnimationPlayableDefinition StartTargettingPoseAnimation);
        void OnPlayerStoppedTargetting();
    }
}