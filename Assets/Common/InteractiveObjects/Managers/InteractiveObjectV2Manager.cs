﻿using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace InteractiveObjects
{
    /// <summary>
    /// Holds references to ALL <see cref="CoreInteractiveObject"/>. Even if they are not <see cref="CoreInteractiveObject.IsAskingToBeDestroyed"/>, the
    /// reference is still added to <see cref="InteractiveObjects"/> and <see cref="InteractiveObjectsIndexedByLogicCollider"/>.
    /// </summary>
    public class InteractiveObjectV2Manager : GameSingleton<InteractiveObjectV2Manager>
    {
        /// <summary>
        /// References to all <see cref="CoreInteractiveObject"/>. Objects with <see cref="CoreInteractiveObject.IsAskingToBeDestroyed"/> set to false will not be updated
        /// in workflow methods.
        /// </summary>
        public List<CoreInteractiveObject> InteractiveObjects { get; private set; } = new List<CoreInteractiveObject>();
        
        /// <summary>
        /// <see cref="CoreInteractiveObject"/> indexed by it's LogicCollider (<see cref="InteractiveGameObject.LogicCollider"/>).
        /// This lookup table can be used on Physics event to check if the triggered collider is a <see cref="CoreInteractiveObject"/>.
        /// </summary>
        public Dictionary<Collider, CoreInteractiveObject> InteractiveObjectsIndexedByLogicCollider { get; private set; } = new Dictionary<Collider, CoreInteractiveObject>();

        public void Init()
        {
            #region Event Registering

            InteractiveObjectEventsManagerSingleton.Get().RegisterOnInteractiveObjectCreatedEventListener(OnInteractiveObjectCreated);
            InteractiveObjectEventsManagerSingleton.Get().RegisterOnInteractiveObjectDestroyedEventListener(OnInteractiveObjectDestroyed);

            #endregion

            this.InitializeAllInteractiveObjectsInitializer();
        }

        /// <summary>
        /// All <see cref="InteractiveObjectInitializer"/> are destroyed after their initialization (See <see cref="InteractiveObjectInitializer.Init"/>).
        /// So this method is idempotent.
        /// </summary>
        public void InitializeAllInteractiveObjectsInitializer()
        {
            var InteractiveObjectInitializers = GameObject.FindObjectsOfType<InteractiveObjectInitializer>();
            if (InteractiveObjectInitializers != null)
                for (var InteractiveObjectInitializerIndex = 0; InteractiveObjectInitializerIndex < InteractiveObjectInitializers.Length; InteractiveObjectInitializerIndex++)
                    InteractiveObjectInitializers[InteractiveObjectInitializerIndex].Init();
        }

        public void FixedTick(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < InteractiveObjects.Count; InteractiveObjectIndex++)
                if (InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                    InteractiveObjects[InteractiveObjectIndex].FixedTick(d);
        }

        public void Tick(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < InteractiveObjects.Count; InteractiveObjectIndex++)
                if (InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                    InteractiveObjects[InteractiveObjectIndex].Tick(d);
        }

        public void AfterTicks(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < InteractiveObjects.Count; InteractiveObjectIndex++)
                if (InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                    InteractiveObjects[InteractiveObjectIndex].AfterTicks(d);

            List<CoreInteractiveObject> InteractiveObjectsToDelete = null;
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < InteractiveObjects.Count; InteractiveObjectIndex++)
                if (InteractiveObjects[InteractiveObjectIndex].IsAskingToBeDestroyed)
                {
                    if (InteractiveObjectsToDelete == null) InteractiveObjectsToDelete = new List<CoreInteractiveObject>();

                    InteractiveObjectsToDelete.Add(InteractiveObjects[InteractiveObjectIndex]);
                }

            if (InteractiveObjectsToDelete != null)
                foreach (var InteractiveObjectToDelete in InteractiveObjectsToDelete)
                {
                    InteractiveObjectToDelete.Destroy();
                    OnInteractiveObjectDestroyed(InteractiveObjectToDelete);
                }
        }

        public void LateTick(float d)
        {
            for (var InteractiveObjectIndex = 0; InteractiveObjectIndex < InteractiveObjects.Count; InteractiveObjectIndex++)
                if (InteractiveObjects[InteractiveObjectIndex].IsUpdatedInMainManager)
                    InteractiveObjects[InteractiveObjectIndex].LateTick(d);
        }


        private void OnInteractiveObjectCreated(CoreInteractiveObject InteractiveObject)
        {
            InteractiveObjects.Add(InteractiveObject);
            var interactiveObjectLogicCollider = InteractiveObject.InteractiveGameObject.LogicCollider;
            if (interactiveObjectLogicCollider != null) InteractiveObjectsIndexedByLogicCollider.Add(interactiveObjectLogicCollider, InteractiveObject);
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject InteractiveObject)
        {
            InteractiveObjects.Remove(InteractiveObject);
            var interactiveObjectLogicCollider = InteractiveObject.InteractiveGameObject.LogicCollider;
            if (interactiveObjectLogicCollider != null) InteractiveObjectsIndexedByLogicCollider.Remove(interactiveObjectLogicCollider);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            for (var i = InteractiveObjects.Count - 1; i >= 0; i--) OnInteractiveObjectDestroyed(InteractiveObjects[i]);

            InteractiveObjects.Clear();
            InteractiveObjects = null;
            InteractiveObjectsIndexedByLogicCollider.Clear();
            InteractiveObjectsIndexedByLogicCollider = null;
        }
    }
}