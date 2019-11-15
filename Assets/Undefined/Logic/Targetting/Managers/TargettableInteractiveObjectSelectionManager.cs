using System.Collections.Generic;
using CoreGame;
using Input;
using InteractiveObjects;
using UnityEngine;

namespace Targetting
{
    public class TargettableInteractiveObjectSelectionManager : GameSingleton<TargettableInteractiveObjectSelectionManager>
    {
        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();

        #endregion

        private ObjectVariable<CoreInteractiveObject> CurrentlyTargettedInteractiveObject;

        /// <summary>
        /// /!\ When an operation on this list provokes a change in <see cref="CurrentlyTargettedInteractiveObject"/>. It is crutial
        /// to update first the <see cref="AllSelectableTargettedInteractiveObject"/> then the <see cref="CurrentlyTargettedInteractiveObject"/>.
        /// This is because of the <see cref="CurrentlyTargettedInteractiveObject"/> change event <see cref="OnCurrentlytargettedObjectChanged"/> that may change
        /// <see cref="CurrentlyTargettedInteractiveObject"/> value based on current <see cref="AllSelectableTargettedInteractiveObject"/>.
        /// </summary>
        private List<CoreInteractiveObject> AllSelectableTargettedInteractiveObject = new List<CoreInteractiveObject>();

        public TargettableInteractiveObjectSelectionManager()
        {
            this.CurrentlyTargettedInteractiveObject = new ObjectVariable<CoreInteractiveObject>(
                OnObjectValueChanged: this.OnCurrentlytargettedObjectChanged
            );
        }

        public void Tick()
        {
            if (GameInputManager.CurrentInput.SwitchSelectionButtonD())
            {
                if (this.CurrentlyTargettedInteractiveObject.GetValue() != null)
                {
                    var currentlySelectedObjectIndex = this.AllSelectableTargettedInteractiveObject.IndexOf(this.CurrentlyTargettedInteractiveObject.GetValue());
                    currentlySelectedObjectIndex++;
                    currentlySelectedObjectIndex = (int) Mathf.Repeat(currentlySelectedObjectIndex, this.AllSelectableTargettedInteractiveObject.Count);
                    this.CurrentlyTargettedInteractiveObject.SetValue(this.AllSelectableTargettedInteractiveObject[currentlySelectedObjectIndex]);
                }
            }
        }

        #region External Events

        public void OnCursorOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.AllSelectableTargettedInteractiveObject.Add(CoreInteractiveObject);

            /// If the CurrentlyTargettedInteractiveObject is null means that there is currently no target.
            /// Then the value is setted to immediately get the target.
            if (this.CurrentlyTargettedInteractiveObject.GetValue() == null)
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(CoreInteractiveObject);
            }
        }

        public void OnCursorNoMoveOverObject(CoreInteractiveObject coreInteractiveObject)
        {
            this.OnInteractiveObjectDestroyed(coreInteractiveObject);
        }

        /// <summary>
        /// /!\ Called only from <see cref="TargettableInteractiveObjectScreenIntersectionManager"/> when a Targettable listened InteractiveObject is destroyed.
        /// </summary>
        public void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            this.AllSelectableTargettedInteractiveObject.Remove(CoreInteractiveObject);

            if (CoreInteractiveObject == this.CurrentlyTargettedInteractiveObject.GetValue())
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(null);
            }
        }

        #endregion

        #region Internal Events

        /// <summary>
        /// Called when <see cref="CurrentlyTargettedInteractiveObject"/> value has changed.
        /// If the <paramref name="newObject"/> is null then the <see cref="CurrentlyTargettedInteractiveObject"/> is setted to an available
        /// <see cref="AllSelectableTargettedInteractiveObject"/>.
        /// </summary>
        private void OnCurrentlytargettedObjectChanged(CoreInteractiveObject oldObject, CoreInteractiveObject newObject)
        {
            if (newObject == null && this.AllSelectableTargettedInteractiveObject.Count > 0)
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(this.AllSelectableTargettedInteractiveObject[0]);
            }
        }

        #endregion

        #region Logical Conditions

        public bool IsCurrentlyTargetting()
        {
            return this.CurrentlyTargettedInteractiveObject.GetValue() != null;
        }

        #endregion

        #region Data Retrieval

        public CoreInteractiveObject GetCurrentlyTargettedInteractiveObject()
        {
            return this.CurrentlyTargettedInteractiveObject.GetValue();
        }

        #endregion
    }
}