using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace LevelManagement
{
    [SceneHandleDraw]
    [Serializable]
    public class LevelChunkInteractiveObjectDefinition
    {
        [CustomEnum()] public LevelZoneChunkID LevelZoneChunkID;

        [WireBox(R = 0f, G = 1f, B = 0f, CenterFieldName = nameof(LevelChunkInteractiveObjectDefinition.LocalCenter), SizeFieldName = nameof(LevelChunkInteractiveObjectDefinition.LocalSize))]
        public Vector3 LocalCenter;

        public Vector3 LocalSize;
    }

    public class LevelChunkInteractiveObject : CoreInteractiveObject
    {
        private LevelChunkInteractiveObjectDefinition LevelChunkInteractiveObjectDefinition;

        private LevelChunkTrackerSystem LevelChunkTrackerSystem;
        private LevelChunkTransitionFXSystem LevelChunkTransitionFXSystem;

        #region Data Retrieval

        public TransitionableLevelFXType GetTransitionableLevelFXType()
        {
            return this.LevelChunkTransitionFXSystem.TransitionableLevelFXType;
        }

        public LevelZoneChunkID GetLevelZoneChunkID()
        {
            return this.LevelChunkInteractiveObjectDefinition.LevelZoneChunkID;
        }

        #endregion

        public LevelChunkInteractiveObject(IInteractiveGameObject interactiveGameObject, LevelChunkInteractiveObjectDefinition LevelChunkInteractiveObjectDefinition)
        {
            this.LevelChunkInteractiveObjectDefinition = LevelChunkInteractiveObjectDefinition;
            base.BaseInit(interactiveGameObject, false);
        }

        public override void Init()
        {
            this.LevelChunkTrackerSystem = new LevelChunkTrackerSystem(this, LevelChunkInteractiveObjectDefinition, this.OnLevelChunkTriggerEnter, this.OnLevelChunkTriggerExit);
            this.LevelChunkTransitionFXSystem = new LevelChunkTransitionFXSystem(this);
        }

        private void OnLevelChunkTriggerEnter(CoreInteractiveObject Other)
        {
            LevelManagerEventManager.Get().OnChunkLevelEnter(this);
        }

        private void OnLevelChunkTriggerExit(CoreInteractiveObject Other)
        {
            LevelManagerEventManager.Get().OnChunkLevelExit(this);
        }

        public static void DestroyAllDestroyOnStartObjects()
        {
            foreach (var objectToDestroy in GameObject.FindGameObjectsWithTag(TagConstants.TO_DESTROY_ON_START)) MonoBehaviour.Destroy(objectToDestroy);
        }
    }
}