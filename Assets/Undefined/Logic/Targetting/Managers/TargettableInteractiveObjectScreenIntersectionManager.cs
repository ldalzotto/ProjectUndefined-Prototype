using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using UnityEngine;

namespace Targetting
{
    /// <summary>
    /// Stores and update all <see cref="CoreInteractiveObject"/> that are currently visible.
    /// </summary>
    public class TargettableInteractiveObjectScreenIntersectionManager : GameSingleton<TargettableInteractiveObjectScreenIntersectionManager>
    {
        /// <summary>
        /// A value of true means that the associated <see cref="CoreInteractiveObject"/> is visible on screen.
        /// This lookup table is only updated by <see cref="UpdateInteractiveObjectsScreenVisibility"/>.
        /// This lookup table is fed by the <see cref="CoreInteractiveObject"/> creation and destroy events <see cref="OnInteractiveObjectCreated"/>, <see cref="OnInteractiveObjectDestroyed"/>
        /// </summary>
        private Dictionary<CoreInteractiveObject, bool> InteractiveObjectsListened = new Dictionary<CoreInteractiveObject, bool>();

        /// <summary>
        /// A lookup table indicating which <see cref="InteractiveObjectsListened"/> are currently hovered by the play <see cref="TargetCursorSystem"/>.
        /// This table have the same keys as <see cref="InteractiveObjectsListened"/>.
        /// When the value change, the internal events <see cref="OnCursorOverObject"/> and <see cref="OnCursorNoMoveOverObject"/> are called.
        /// </summary>
        private Dictionary<CoreInteractiveObject, BoolVariable> InteractiveObjectsOverCursorTarget = new Dictionary<CoreInteractiveObject, BoolVariable>();

        public void InitializeEvents()
        {
            InteractiveObjectEventsManager.Get().RegisterOnInteractiveObjectCreatedEventListener(this.OnInteractiveObjectCreated);
            InteractiveObjectEventsManager.Get().RegisterOnInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
        }

        /// <summary>
        /// The manager is only update when the <see cref="TargetCursorSystem"/> is running because it it only at this moment that the player is aiming.
        /// This is to avoid unnecessary calcualtions.
        /// </summary>
        public void Tick(float d, Vector2 TargetCursorScreenPosition)
        {
            /// Update InteractiveObjectsListened Values
            UpdateInteractiveObjectsScreenVisibility();

            foreach (var interactiveObjectListened in this.InteractiveObjectsListened.Keys)
            {
                //If the itneractive object is visible
                if (this.InteractiveObjectsListened[interactiveObjectListened])
                {
                    var initialRect = interactiveObjectListened.InteractiveGameObject.AverageModelLocalBounds.Bounds.Mul(interactiveObjectListened.InteractiveGameObject.GetLocalToWorld()).ToScreenSpace(Camera.main);
                    InteractiveObjectsOverCursorTarget[interactiveObjectListened].SetValue(initialRect.Contains(TargetCursorScreenPosition));
                }
            }
        }

        /// <summary>
        /// Update <see cref="InteractiveObjectsListened"/> value by calculating if the InteractiveObject is visible or not.
        /// </summary>
        private void UpdateInteractiveObjectsScreenVisibility()
        {
            Dictionary<CoreInteractiveObject, bool> ChangedValues = null;
            foreach (var interactiveObjectListened in this.InteractiveObjectsListened.Keys)
            {
                var visibility = interactiveObjectListened.InteractiveGameObject.IsVisible();
                if (visibility != this.InteractiveObjectsListened[interactiveObjectListened])
                {
                    if (ChangedValues == null)
                    {
                        ChangedValues = new Dictionary<CoreInteractiveObject, bool>();
                    }

                    ChangedValues[interactiveObjectListened] = visibility;
                }
            }

            if (ChangedValues != null)
            {
                foreach (var changedValue in ChangedValues)
                {
                    this.InteractiveObjectsListened[changedValue.Key] = changedValue.Value;
                }
            }
        }

        private void OnInteractiveObjectCreated(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject.InteractiveObjectTag.IsTakingDamage)
            {
                this.InteractiveObjectsListened.Add(CoreInteractiveObject, CoreInteractiveObject.InteractiveGameObject.IsVisible());
                this.InteractiveObjectsOverCursorTarget.Add(CoreInteractiveObject, new BoolVariable(false, () => this.OnCursorOverObject(CoreInteractiveObject), () => { this.OnCursorNoMoveOverObject(CoreInteractiveObject); }));
            }
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject.InteractiveObjectTag.IsTakingDamage)
            {
                this.InteractiveObjectsListened.Remove(CoreInteractiveObject);
                this.InteractiveObjectsOverCursorTarget.Remove(CoreInteractiveObject);
                TargettableInteractiveObjectSelectionManager.Get().OnInteractiveObjectDestroyed(CoreInteractiveObject);
            }
        }

        #region Internal Events

        /// <summary>
        /// /!\ This events is only called when <see cref="InteractiveObjectsListened"/> value changes from false to true
        /// </summary>
        private void OnCursorOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            TargettableInteractiveObjectSelectionManager.Get().OnCursorOverObject(CoreInteractiveObject);
        }

        /// <summary>
        /// /!\ This events is only called when <see cref="InteractiveObjectsListened"/> value changes from true to false
        /// </summary>
        private void OnCursorNoMoveOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            TargettableInteractiveObjectSelectionManager.Get().OnCursorNoMoveOverObject(CoreInteractiveObject);
        }

        #endregion
    }
}