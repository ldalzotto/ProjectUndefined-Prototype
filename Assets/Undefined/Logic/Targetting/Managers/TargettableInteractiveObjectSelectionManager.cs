using System.Collections.Generic;
using CoreGame;
using Input;
using InteractiveObjects;
using UnityEngine;

namespace Targetting
{
    public class TargettableInteractiveObjectSelectionManager : GameSingleton<TargettableInteractiveObjectSelectionManager>
    {
        private GameInputManager GameInputManager = GameInputManager.Get(); 
        public CoreInteractiveObject CurrentlyTargettedInteractiveObject { get; private set; }
        private List<CoreInteractiveObject> AllSelectableTargettedInteractiveObject = new List<CoreInteractiveObject>();

        public void OnCursorOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            if (this.CurrentlyTargettedInteractiveObject == null)
            {
                this.CurrentlyTargettedInteractiveObject = CoreInteractiveObject;
            }
            this.AllSelectableTargettedInteractiveObject.Add(CoreInteractiveObject);
        }

        public void Tick()
        {
            if (GameInputManager.CurrentInput.SwitchSelectionButtonD())
            {
                if (this.CurrentlyTargettedInteractiveObject != null)
                {
                    var currentlySelectedObjectIndex = this.AllSelectableTargettedInteractiveObject.IndexOf(this.CurrentlyTargettedInteractiveObject);
                    currentlySelectedObjectIndex++;
                    currentlySelectedObjectIndex = (int)Mathf.Repeat(currentlySelectedObjectIndex, this.AllSelectableTargettedInteractiveObject.Count);
                    this.CurrentlyTargettedInteractiveObject = this.AllSelectableTargettedInteractiveObject[currentlySelectedObjectIndex];
                }
            }
        }

        public void OnCursorNoMoveOverObject(CoreInteractiveObject coreInteractiveObject)
        {
            this.OnInteractiveObjectDestroyed(coreInteractiveObject);
        }

        #region Logical Conditions

        public bool IsCurrentlyTargetting()
        {
            return this.CurrentlyTargettedInteractiveObject != null;
        }

        #endregion

        /// <summary>
        /// /!\ Called only from <see cref="TargettableInteractiveObjectScreenIntersectionManager"/> when a Targettable listened InteractiveObject is destroyed.
        /// </summary>
        public void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject == this.CurrentlyTargettedInteractiveObject)
            {
                this.CurrentlyTargettedInteractiveObject = null;
            }

            this.AllSelectableTargettedInteractiveObject.Remove(CoreInteractiveObject);
        }
    }
}