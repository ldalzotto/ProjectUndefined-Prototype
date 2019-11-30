using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using Input;
using UnityEngine;

namespace TimeManagement
{
    /// <summary>
    /// Holds the current delta time value.
    /// Every call to <see cref="Time"/> unity API must replaced by <see cref="TimeManagementManager"/>.
    /// This allow full control of slow motion effect.
    /// </summary>
    public class TimeManagementManager : GameSingleton<TimeManagementManager>
    {
        private TimeFreezeSystem _timeFreezeSystem;
        private TimeInputSystem TimeInputSystem;

        public TimeManagementManager()
        {
            #region External Dependencies

            var GameInputManager = Input.GameInputManager.Get();

            #endregion

            _timeFreezeSystem = new TimeFreezeSystem();
            TimeInputSystem = new TimeInputSystem(GameInputManager,
                this._timeFreezeSystem.OnTimeFrozen,
                this._timeFreezeSystem.OnTimeResettedToNormal);
        }

        public void Tick()
        {
            this.TimeInputSystem.Tick();
        }

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

    class TimeInputSystem
    {
        private GameInputManager GameInputManager;

        private BoolVariable TimeSlowTriggerValue;

        public TimeInputSystem(GameInputManager GameInputManager, Action OnTrimeTriggerToTrue, Action OnTimeTriggerToFalse)
        {
            this.TimeSlowTriggerValue = new BoolVariable(false, OnTrimeTriggerToTrue, OnTimeTriggerToFalse);
            this.GameInputManager = GameInputManager;
        }

        public void Tick()
        {
            if (this.GameInputManager.CurrentInput.FreezeTimeDown())
            {
                this.TimeSlowTriggerValue.SetValue(!this.TimeSlowTriggerValue.GetValue());
            }
        }
        
        #region Logical Conditions

        public bool IsTimeFrozen()
        {
            return this.TimeSlowTriggerValue.GetValue();
        }
        #endregion
        
    }
}