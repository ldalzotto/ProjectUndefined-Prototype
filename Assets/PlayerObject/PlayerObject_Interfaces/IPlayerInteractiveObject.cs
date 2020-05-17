using InteractiveObjects_Interfaces;

namespace PlayerObject_Interfaces
{
    public interface IPlayerInteractiveObject
    {
        IInteractiveGameObject InteractiveGameObject { get; }
    }
}