using System;
using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using UnityEngine;

namespace Targetting
{
    /// <summary>
    /// Keep track of all <see cref="CoreInteractiveObject"/> that intersects the target cursor.
    /// Intersection is done by projecting the interactive object model bounds to screen space and checking intersection with cursor.
    /// </summary>
    public class InteractiveObjectCursorScreenIntersectionManager : GameSingleton<InteractiveObjectCursorScreenIntersectionManager>
    {
        /// <summary>
        /// A value of true means that the associated <see cref="CoreInteractiveObject"/> is visible on screen.
        /// This lookup table is only updated by <see cref="UpdateInteractiveObjectsScreenVisibility"/>.
        /// This lookup table is fed by the <see cref="CoreInteractiveObject"/> creation and destroy events <see cref="OnInteractiveObjectCreated"/>, <see cref="OnInteractiveObjectDestroyed"/>
        /// </summary>
        private Dictionary<CoreInteractiveObject, bool> InteractiveObjectsListened = new Dictionary<CoreInteractiveObject, bool>();

        /// <summary>
        /// A lookup table indicating which <see cref="InteractiveObjectsListened"/> are currently hovered by the player <see cref="TargetCursorSystem"/>.
        /// This table has the same keys as <see cref="InteractiveObjectsListened"/>.
        /// This table is only updated by <see cref="UpdateCursorIntersection"/>.
        /// When the value change, the internal events <see cref="OnCursorOverObject"/> and <see cref="OnCursorNoMoveOverObject"/> are called.
        /// </summary>
        private Dictionary<CoreInteractiveObject, BoolVariable> InteractiveObjectsOverCursorTarget = new Dictionary<CoreInteractiveObject, BoolVariable>();

        /// <summary>
        /// A List that contains all interactive objects that are currently intersecting with the cursor.
        /// This list is degined only to be read by exeternal sources for initialization. 
        /// </summary>
        public List<CoreInteractiveObject> IntersectingInteractiveObjects { get; private set; } = new List<CoreInteractiveObject>();

        private Func<InteractiveObjectTag, bool> InteractiveObjectSelectionGuard;

        public InteractiveObjectCursorScreenIntersectionManager()
        {
            this.InteractiveObjectSelectionGuard = (InteractiveObjectTag) => InteractiveObjectTag.IsTakingDamage && !InteractiveObjectTag.IsPlayer;
        }

        public void InitializeEvents()
        {
            InteractiveObjectEventsManager.Get().RegisterOnAllInteractiveObjectCreatedEventListener(this.OnInteractiveObjectCreated);
        }

        #region Internal Events

        private event Action<CoreInteractiveObject> OnCursorOverObjectEvent;

        public void RegisterOnCursorOverObjectEvent(Action<CoreInteractiveObject> action)
        {
            this.OnCursorOverObjectEvent += action;
        }

        public void UnRegisterOnCursorOverObjectEvent(Action<CoreInteractiveObject> action)
        {
            this.OnCursorOverObjectEvent -= action;
        }

        private event Action<CoreInteractiveObject> OnCursorNoMoveOverObjectEvent;

        public void RegisterOnCursorNoMoreOverObjectEvent(Action<CoreInteractiveObject> action)
        {
            this.OnCursorNoMoveOverObjectEvent += action;
        }

        public void UnRegisterOnCursorNoMoreOverObjectEvent(Action<CoreInteractiveObject> action)
        {
            this.OnCursorNoMoveOverObjectEvent -= action;
        }

        /// <summary>
        /// /!\ This events is only called when <see cref="InteractiveObjectsListened"/> value changes from false to true
        /// </summary>
        private void OnCursorOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.IntersectingInteractiveObjects.Add(CoreInteractiveObject);
            this.OnCursorOverObjectEvent?.Invoke(CoreInteractiveObject);
        }

        /// <summary>
        /// /!\ This events is only called when <see cref="InteractiveObjectsListened"/> value changes from true to false
        /// </summary>
        private void OnCursorNoMoveOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.IntersectingInteractiveObjects.Remove(CoreInteractiveObject);
            this.OnCursorNoMoveOverObjectEvent?.Invoke(CoreInteractiveObject);
        }

        #endregion

        /// <summary>
        /// The manager is only update when the <see cref="TargetCursorSystem"/> is running because it it only at this moment that the player is aiming.
        /// This is to avoid unnecessary calcualtions.
        /// TODO -> This step can be extracted to a BurstJob if calculations are too heavy  
        /// </summary>
        public void Tick(float unscaled, Vector2 TargetCursorScreenPosition)
        {
            UpdateInteractiveObjectsScreenVisibility();
            UpdateCursorIntersection(TargetCursorScreenPosition);
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

        /// <summary>
        /// Update <see cref="InteractiveObjectsOverCursorTarget"/> values by calculating if the <paramref name="TargetCursorScreenPosition"/> is inside
        /// the Screen projected bounds.
        /// </summary>
        private void UpdateCursorIntersection(Vector2 TargetCursorScreenPosition)
        {
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


        private void OnInteractiveObjectCreated(CoreInteractiveObject CoreInteractiveObject)
        {
            if (this.InteractiveObjectSelectionGuard.Invoke(CoreInteractiveObject.InteractiveObjectTag))
            {
                this.InteractiveObjectsListened.Add(CoreInteractiveObject, CoreInteractiveObject.InteractiveGameObject.IsVisible());
                this.InteractiveObjectsOverCursorTarget.Add(CoreInteractiveObject, new BoolVariable(false, () => this.OnCursorOverObject(CoreInteractiveObject), () => { this.OnCursorNoMoveOverObject(CoreInteractiveObject); }));
                CoreInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectDestroyed);
            }
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            this.InteractiveObjectsListened.Remove(CoreInteractiveObject);
            this.IntersectingInteractiveObjects.Remove(CoreInteractiveObject);
            this.InteractiveObjectsOverCursorTarget.Remove(CoreInteractiveObject);
        }
    }
}