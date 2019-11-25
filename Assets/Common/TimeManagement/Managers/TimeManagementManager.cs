using System.Collections;
using System.Collections.Generic;
using CoreGame;
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
        /// <summary>
        /// The initial physics delta time set for Unity project (ProjectSettings->Time->FixedTimeStep)
        /// /!\ This value must not be modified as it is used as a reference for updating the physics timstep. 
        /// </summary>
        private float InitialPhysicsDeltaTime = Time.fixedDeltaTime;

        public void UpdateFixedDeltaTime()
        {
            Time.fixedDeltaTime = this.InitialPhysicsDeltaTime * Time.timeScale;
        }
        
        #region External Events

        public void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

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
    }
}