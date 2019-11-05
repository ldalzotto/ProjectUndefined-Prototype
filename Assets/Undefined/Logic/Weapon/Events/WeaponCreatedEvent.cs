using CoreGame;

namespace Weapon
{
    public class WeaponCreatedEvent : GameSingleton<WeaponCreatedEvent>
    {
        public void OnWeaponCreated(Weapon weapon)
        {
            WeaponRecoilTimeManager.Get().OnWeaponCreated(weapon);
        }
    }
}