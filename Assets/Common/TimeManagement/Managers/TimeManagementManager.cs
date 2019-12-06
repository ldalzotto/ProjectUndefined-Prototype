using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using Input;
using UnityEngine;
using UnityEngine.UI;

namespace TimeManagement
{
    /// <summary>
    /// Holds the current delta time value.
    /// Every call to <see cref="Time"/> unity API must replaced by <see cref="TimeManagementManager"/>.
    /// This allow full control of slow motion effect.
    /// /!\ Manager entry points can be called from either <see cref="FixedTick"/> or <see cref="Tick"/>. This is because inputs can be checked in the Physics step too.
    /// </summary>
    public class TimeManagementManager : GameSingleton<TimeManagementManager>
    {
        private TimeFreezeSystem _timeFreezeSystem;
        private TimeInputSystem TimeInputSystem;
        private TimePausedIconSystem TimePausedIconSystem;

        public TimeManagementManager()
        {
            #region External Dependencies

            var GameInputManager = Input.GameInputManager.Get();

            #endregion

            _timeFreezeSystem = new TimeFreezeSystem();
            this.TimePausedIconSystem = new TimePausedIconSystem(TimeManagementConfigurationGameObject.Get().TimeManagementConfiguration);
            TimeInputSystem = new TimeInputSystem(GameInputManager,
                this.OnTimeFrozen,
                this.OnTimeResettedToNormal);
        }

        public void FixedTick()
        {
            this.TimeInputSystem.FixedTick();
        }

        public void Tick()
        {
            this.TimeInputSystem.Tick();
        }

        public void LateTick()
        {
            this.TimeInputSystem.LateTick();
        }

        #region Internal Events

        private void OnTimeFrozen()
        {
            this._timeFreezeSystem.OnTimeFrozen();
            this.TimePausedIconSystem.OnTimeFrozen();
        }

        private void OnTimeResettedToNormal()
        {
            this._timeFreezeSystem.OnTimeResettedToNormal();
            this.TimePausedIconSystem.OnTimeResettedToNormal();
        }

        #endregion

        #region External Events

        public float GetCurrentDeltaTime()
        {
            return Time.deltaTime;
        }

        public float GetCurrentDeltaTimeUnscaled()
        {
            return Time.unscaledDeltaTime;
        }

        public float GetCurrentFixedDeltaTime()
        {
            return Time.fixedDeltaTime;
        }

        #endregion

        #region Logical Conditions

        /// <summary>
        /// /!\ This method can be called either from FixedTick or Tick.
        /// </summary>
        public bool IsTimeFrozen()
        {
            return this.TimeInputSystem.IsTimeFrozen();
        }

        #endregion
    }

    class TimeFreezeSystem
    {
        /// <summary>
        /// The initial physics delta time set for Unity project (ProjectSettings->Time->FixedTimeStep)
        /// /!\ This value must not be modified as it is used as a reference for updating the physics timestep. 
        /// </summary>
        private float InitialPhysicsDeltaTime = Time.fixedDeltaTime;

        private const float SlowedTimeScale = 0f;

        public void OnTimeFrozen()
        {
            Time.timeScale = SlowedTimeScale;
            Time.fixedDeltaTime = this.InitialPhysicsDeltaTime * Time.timeScale;
        }

        public void OnTimeResettedToNormal()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = this.InitialPhysicsDeltaTime * Time.timeScale;
        }
    }
    
    /// <summary>
    /// Because time condition can be checked in the physics step, the internal state of <see cref="TimeInputSystem"/> must be updated only once per frame. Because multiple update would lead to possible press/release
    /// buffer loss.
    /// This is achieved by tracking the update state <see cref="HasBeenUpdatedThisFrame"/> that is resetted in the <see cref="LateTick"/> and set to true on either <see cref="Tick"/> or <see cref="LateTick"/>. 
    /// </summary>
    class TimeInputSystem
    {
        private bool HasBeenUpdatedThisFrame;
        private GameInputManager GameInputManager;

        private BoolVariable TimeSlowTriggerValue;

        public TimeInputSystem(GameInputManager GameInputManager, Action OnTrimeTriggerToTrue, Action OnTimeTriggerToFalse)
        {
            this.HasBeenUpdatedThisFrame = false;
            this.TimeSlowTriggerValue = new BoolVariable(false, OnTrimeTriggerToTrue, OnTimeTriggerToFalse);
            this.GameInputManager = GameInputManager;
        }

        public void FixedTick()
        {
            this.InternalTick();
        }

        public void Tick()
        {
            this.InternalTick();
        }

        public void LateTick()
        {
            this.HasBeenUpdatedThisFrame = false;
        }

        private void InternalTick()
        {
            if (!HasBeenUpdatedThisFrame)
            {
                if (this.GameInputManager.CurrentInput.FreezeTimeDown())
                {
                    this.TimeSlowTriggerValue.SetValue(!this.TimeSlowTriggerValue.GetValue());
                }

                this.HasBeenUpdatedThisFrame = true;
            }
        }

        #region Logical Conditions

        public bool IsTimeFrozen()
        {
            return this.TimeSlowTriggerValue.GetValue();
        }

        #endregion
    }

    class TimePausedIconSystem
    {
        private Image InstanciatedPauseIcon;

        public TimePausedIconSystem(TimeManagementConfiguration TimeManagementConfiguration)
        {
            this.InstanciatedPauseIcon = MonoBehaviour.Instantiate(TimeManagementConfiguration.TimePausedIconPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            this.InstanciatedPauseIcon.enabled = false;
        }

        public void OnTimeFrozen()
        {
            this.InstanciatedPauseIcon.enabled = true;
        }

        public void OnTimeResettedToNormal()
        {
            this.InstanciatedPauseIcon.enabled = false;
        }
    }
}