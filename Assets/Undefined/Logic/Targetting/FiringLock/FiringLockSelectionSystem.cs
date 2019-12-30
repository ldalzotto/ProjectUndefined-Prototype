using System;
using System.Collections.Generic;
using CoreGame;
using Input;
using InteractiveObjects;
using UnityEngine;

namespace Targetting
{
    /// <summary>
    /// This system tracks intersected interactive objects calculated by <see cref="InteractiveObjectCursorScreenIntersectionManager"/> (by registering to it's events).
    /// And provides the <see cref="CurrentlyTargettedInteractiveObject"/>.
    /// It handles Interactive object deletion and switches between interactive objects. All changes of <see cref="CurrentlyTargettedInteractiveObject"/> are notified via the callback <see cref="OnNewInteractiveObjectTargettedCallback"/>.
    /// </summary>
    public class FiringLockSelectionSystem
    {
        #region External Dependencies

        private GameInputManager GameInputManager;

        #endregion

        /// <summary>
        /// The currently selected interactive object.
        /// </summary>
        private ObjectVariable<CoreInteractiveObject> CurrentlyTargettedInteractiveObject;

        /// <summary>
        /// /!\ When an operation on this list provokes a change in <see cref="CurrentlyTargettedInteractiveObject"/>. It is crutial
        /// to update first the <see cref="AllSelectableTargettedInteractiveObject"/> then the <see cref="CurrentlyTargettedInteractiveObject"/>.
        /// This is because of the <see cref="CurrentlyTargettedInteractiveObject"/> change event <see cref="OnCurrentlytargettedObjectChanged"/> may change
        /// <see cref="CurrentlyTargettedInteractiveObject"/> value based on current <see cref="AllSelectableTargettedInteractiveObject"/>.
        /// </summary>
        private List<CoreInteractiveObject> AllSelectableTargettedInteractiveObject;

        /// <summary>
        /// The event callback called when the <see cref="CurrentlyTargettedInteractiveObject"/> value has changed.
        /// /!\ This callback is also called on initialisation when the constructor is called. 
        /// </summary>
        private Action<CoreInteractiveObject> OnNewInteractiveObjectTargettedCallback;

        public FiringLockSelectionSystem(Action<CoreInteractiveObject> OnNewInteractiveObjectTargettedCallback)
        {
            this.GameInputManager = GameInputManager.Get();
            this.AllSelectableTargettedInteractiveObject = new List<CoreInteractiveObject>();
            this.OnNewInteractiveObjectTargettedCallback = OnNewInteractiveObjectTargettedCallback;
            this.CurrentlyTargettedInteractiveObject = default;

            this.CurrentlyTargettedInteractiveObject = new ObjectVariable<CoreInteractiveObject>(
                OnObjectValueChanged: this.OnCurrentlytargettedObjectChanged
            );
            this.InitializeEvents();

            /// Initialization
            foreach (var cursorIntersectedInteractiveObject in InteractiveObjectCursorScreenIntersectionManager.Get().IntersectingInteractiveObjects)
            {
                this.OnCursorOverObject(cursorIntersectedInteractiveObject);
            }
        }


        private void InitializeEvents()
        {
            InteractiveObjectCursorScreenIntersectionManager.Get().RegisterOnCursorOverObjectEvent(this.OnCursorOverObject);
            InteractiveObjectCursorScreenIntersectionManager.Get().RegisterOnCursorNoMoreOverObjectEvent(this.OnCursorNoMoreOverObject);
        }

        public void Dispose()
        {
            InteractiveObjectCursorScreenIntersectionManager.Get().UnRegisterOnCursorOverObjectEvent(this.OnCursorOverObject);
            InteractiveObjectCursorScreenIntersectionManager.Get().UnRegisterOnCursorNoMoveOverObjectEvent(this.OnCursorNoMoreOverObject);

            foreach (var selectableTargettedInteractiveObject in AllSelectableTargettedInteractiveObject)
            {
                selectableTargettedInteractiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            }
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

        private void OnCursorOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.AllSelectableTargettedInteractiveObject.Add(CoreInteractiveObject);
            CoreInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);

            /// If the CurrentlyTargettedInteractiveObject is null means that there is currently no target.
            /// Then the value is setted to immediately get the target.
            if (this.CurrentlyTargettedInteractiveObject.GetValue() == null)
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(CoreInteractiveObject);
            }
        }

        private void OnCursorNoMoreOverObject(CoreInteractiveObject coreInteractiveObject)
        {
            this.OnInteractiveObjectDestroyed(coreInteractiveObject);
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
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
            if (oldObject != null)
            {
                oldObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            }

            if (newObject == null && this.AllSelectableTargettedInteractiveObject.Count > 0)
            {
                this.CurrentlyTargettedInteractiveObject.SetValue(this.AllSelectableTargettedInteractiveObject[0]);
            }

            if (newObject != null)
            {
                this.OnNewInteractiveObjectTargettedCallback.Invoke(newObject);
            }
        }

        #endregion
    }
}