using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using UnityEngine;

namespace RangeObjects
{
    public class RangeObjectV2Manager : GameSingleton<RangeObjectV2Manager>
    {
        public List<RangeObjectV2> RangeObjects { get; private set; } = new List<RangeObjectV2>();

        public void Init()
        {
            var rangeInitializers = GameObject.FindObjectsOfType<RangeObjectInitializer>();
            for (var rangeInitializerIndex = 0; rangeInitializerIndex < rangeInitializers.Length; rangeInitializerIndex++) rangeInitializers[rangeInitializerIndex].Init();

            #region Event Register

            RangeEventsManager.Get().RegisterOnRangeObjectCreatedEventListener(OnRangeObjectCreated);
            RangeEventsManager.Get().RegisterOnRangeObjectDestroyedEventListener(OnRangeObjectDestroyed);
            InteractiveObjectEventsManagerSingleton.Get().RegisterOnInteractiveObjectDestroyedEventListener(OnInteractiveObjectDestroyed);

            #endregion
        }

        public void Tick(float d)
        {
            for (var rangeObjectIndex = 0; rangeObjectIndex < RangeObjects.Count; rangeObjectIndex++) RangeObjects[rangeObjectIndex].Tick(d);
        }

        private void OnRangeObjectCreated(RangeObjectV2 rangeObjectV2)
        {
            RangeObjects.Add(rangeObjectV2);
        }

        private void OnRangeObjectDestroyed(RangeObjectV2 rangeObjectV2)
        {
            RangeObjects.Remove(rangeObjectV2);
        }

        private void OnInteractiveObjectDestroyed(CoreInteractiveObject InteractiveObject)
        {
            RangeObjectV2ManagerOperations.ClearAllReferencesOfInteractiveObject(InteractiveObject);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RangeObjects.Clear();
            RangeObjects = null;
        }
    }
}