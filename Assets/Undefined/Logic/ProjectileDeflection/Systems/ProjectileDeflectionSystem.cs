using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEngine;

namespace ProjectileDeflection
{
    public class ProjectileDeflectionSystem 
    {
        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();

        #endregion
        
        
        public void Tick(float d)
        {
            if (GameInputManager.CurrentInput.DeflectProjectileDown())
            {
            }
        }
    }

}
