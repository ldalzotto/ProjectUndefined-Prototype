using CoreGame;
using UnityEngine;

namespace Health
{
    /// <summary>
    /// Manage Health UI for the listened <see cref="HealthSystem"/>.
    /// We use a <see cref="HealthSystem"/> as a listener to separate the HealthUI from the InteractiveObject where <see cref="ListenedHealthSystem"/> is associated to.
    /// </summary>
    public class HealthUIManager : GameSingleton<HealthUIManager>
    {
        private GameObject HealthUIInstanciated;
        private HealthUIFullBarType healthUiFullBarType;

        public HealthUIManager()
        {
            this.HealthUIInstanciated = GameObject.Instantiate(HealthUIConfigurationGameObject.Get().HealthUIConfiguration.HealthUIPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            this.healthUiFullBarType = HealthUIInstanciated.GetComponentInChildren<HealthUIFullBarType>();
        }

        public void Init()
        {
            // to trigger ctor
        }

        /// <summary>
        /// The <see cref="HealthSystem"/> that will trigger internal <see cref="OnHealthValueChanged"/> event and thus, update the UI.
        /// </summary>
        private HealthSystem ListenedHealthSystem;

        /// <summary>
        /// Register an event when health value change from the <paramref name="ListenedHealthSystem"/>.
        /// Also resets the <see cref="healthUiFullBarType"/> value.
        /// /!\ This must only be called from the InteractiveObject that wants it's associated <see cref="HealthSystem"/> value to be displayed by UI.
        /// </summary>
        public void InitEvents(HealthSystem ListenedHealthSystem)
        {
            this.ListenedHealthSystem = ListenedHealthSystem;
            ListenedHealthSystem.RegisterOnHealthValueChangedEventListener(this.OnHealthValueChanged);
        }

        /// <summary>
        /// Event called only from <see cref="ListenedHealthSystem"/> <see cref="HealthSystem.OnHealthValueChangedEvent"/>.
        /// </summary>
        private void OnHealthValueChanged(float oldValue, float newValue)
        {
            this.healthUiFullBarType.SetHealth01(
                Mathf.Clamp01(newValue / this.ListenedHealthSystem.GetMaxHealth())
            );
        }
    }
}