using System;
using System.Collections.Generic;
using InteractiveObjects;

namespace InteractiveObjects
{
    /// <summary>
    /// Responsible of holding a list of <see cref="CoreInteractiveObject"/> while subscribing to <see cref="CoreInteractiveObject.RegisterInteractiveObjectDestroyedEventListener"/>
    /// to dynamically clear the list when the InteractiveObject is destroyed.
    /// Also, callbacks can be provided to be notified when an interactive object is added or removed. 
    /// </summary>
    public struct InteractiveObjectLocalContainerSystem
    {
        public HashSet<CoreInteractiveObject> InsideInteractiveObjects { get; private set; }

        private Action<CoreInteractiveObject> OnInteractiveObjectJustAdded;
        private Action<CoreInteractiveObject> OnInteractiveObjectJustRemoved;

        public InteractiveObjectLocalContainerSystem(
            Action<CoreInteractiveObject> onInteractiveObjectJustAdded = null,
            Action<CoreInteractiveObject> onInteractiveObjectJustRemoved = null) : this()
        {
            this.InsideInteractiveObjects = new HashSet<CoreInteractiveObject>();
            OnInteractiveObjectJustAdded = onInteractiveObjectJustAdded;
            OnInteractiveObjectJustRemoved = onInteractiveObjectJustRemoved;
        }

        public void AddInteractiveObject(CoreInteractiveObject interactiveObject)
        {
            if (this.InsideInteractiveObjects.Add(interactiveObject))
            {
                interactiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
                this.OnInteractiveObjectJustAdded?.Invoke(interactiveObject);
            }
        }

        public void RemoveInteractiveObject(CoreInteractiveObject interactiveObject)
        {
            if (this.InsideInteractiveObjects.Remove(interactiveObject))
            {
                ClearReferencesOfRemovedInteractiveObject(interactiveObject);
            }
        }

        public void OnDestroy()
        {
            foreach (var insideInteractiveObject in InsideInteractiveObjects)
            {
                this.ClearReferencesOfRemovedInteractiveObject(insideInteractiveObject);
            }

            this.InsideInteractiveObjects.Clear();
        }

        private void ClearReferencesOfRemovedInteractiveObject(CoreInteractiveObject interactiveObject)
        {
            interactiveObject.UnRegisterInteractiveObjectDestroyedEventListener(this.OnInteractiveObjectdestroyed);
            this.OnInteractiveObjectJustRemoved?.Invoke(interactiveObject);
        }

        private void OnInteractiveObjectdestroyed(CoreInteractiveObject interactiveObject)
        {
            if (this.InsideInteractiveObjects.Remove(interactiveObject))
            {
                this.ClearReferencesOfRemovedInteractiveObject(interactiveObject);
            }
        }
    }
}