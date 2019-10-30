using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace LevelManagement
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager Instance;

        public static LevelManager Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<LevelManager>();
            }

            return Instance;
        }

        [SerializeField] private LevelZonesID levelID;
        [SerializeField] private LevelZoneChunkID currentLevelZoneChunkWherePlayerIsID = LevelZoneChunkID.NONE;
        [SerializeField] private List<LevelZoneChunkID> allLoadedLevelZonesChunkID;

        public LevelZonesID LevelID
        {
            get => levelID;
            set => levelID = value;
        }

        public List<LevelZoneChunkID> AllLoadedLevelZonesChunkID
        {
            get => allLoadedLevelZonesChunkID;
        }

        public LevelZoneChunkID CurrentLevelZoneChunkWherePlayerIsID
        {
            get => currentLevelZoneChunkWherePlayerIsID;
        }

        #region Internal Managers

        private EnvironmentSceneLevelManager EnvironmentSceneLevelManager;

        #endregion

        #region Internal State

        private LevelType currentLevelType;

        public LevelType CurrentLevelType
        {
            get => currentLevelType;
        }

        #endregion

        public void Init(LevelType currentLevelType)
        {
            this.currentLevelType = currentLevelType;
            this.EnvironmentSceneLevelManager = new EnvironmentSceneLevelManager(
                LevelAvailabilityManager.Get(), this, LevelManagementConfigurationGameObject.Get(), LevelManagerEventManager.Get());
        }

        #region External Event

        public List<AsyncOperation> OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            return this.EnvironmentSceneLevelManager.OnAdventureToPuzzleLevel(nextPuzzleLevel);
        }

        public List<AsyncOperation> OnStartMenuToLevel(LevelZonesID nextLevel)
        {
            return this.EnvironmentSceneLevelManager.LoadAllLevelsAsync(nextLevel);
        }

        public void OnChunkLevelEnter(LevelChunkInteractiveObject NextLevelChunk)
        {
            Debug.Log(MyLog.Format("LevelManager OnChunkLevelEnter"));
            this.currentLevelZoneChunkWherePlayerIsID = NextLevelChunk.GetLevelZoneChunkID();
        }

        public void OnLevelChunkLoaded(LevelZoneChunkID levelZoneChunkID)
        {
            if (this.allLoadedLevelZonesChunkID == null)
            {
                this.allLoadedLevelZonesChunkID = new List<LevelZoneChunkID>();
            }

            this.allLoadedLevelZonesChunkID.Add(levelZoneChunkID);
        }

        #endregion


        #region Data Retrieval

        public LevelZonesID GetCurrentLevel()
        {
            return levelID;
        }

        #endregion
    }

    class EnvironmentSceneLevelManager
    {
        #region External Dependencies

        private LevelAvailabilityManager LevelAvailabilityManager;
        private LevelManager LevelManagerRef;
        private LevelManagerEventManager LevelManagerEventManager;
        private LevelManagementConfigurationGameObject LevelManagementConfigurationGameObject;

        #endregion

        public EnvironmentSceneLevelManager(LevelAvailabilityManager levelAvailabilityManager, LevelManager LevelManagerRef,
            LevelManagementConfigurationGameObject LevelManagementConfigurationGameObject, LevelManagerEventManager LevelManagerEventManager)
        {
            LevelAvailabilityManager = levelAvailabilityManager;
            this.LevelManagerRef = LevelManagerRef;
            this.LevelManagementConfigurationGameObject = LevelManagementConfigurationGameObject;
            this.LevelManagerEventManager = LevelManagerEventManager;

            LoadAllLevels(this.LevelManagerRef.GetCurrentLevel(), async: false);
        }

        private List<AsyncOperation> LoadAllLevels(LevelZonesID levelZonesID, bool async = true)
        {
            List<AsyncOperation> sceneLoadOperations = new List<AsyncOperation>();
            foreach (var levelChunk in this.LevelManagementConfigurationGameObject.LevelHierarchyConfiguration.GetLevelHierarchy(levelZonesID))
            {
                if (this.LevelAvailabilityManager.IsLevelAvailable(levelChunk))
                {
                    var sceneLoadAsyncOperation = SceneLoadingHelper.SceneLoadWithoutDuplicates(this.LevelManagementConfigurationGameObject.ChunkZonesSceneConfiguration.GetSceneName(levelChunk), async);
                    if (sceneLoadAsyncOperation != null)
                    {
                        sceneLoadAsyncOperation.completed += (asyncOperation) => { this.LevelManagerEventManager.OnLevelChunkLoaded(levelChunk); };
                        sceneLoadOperations.Add(sceneLoadAsyncOperation);
                    }
                    else if (!async)
                    {
                        this.LevelManagerEventManager.OnLevelChunkLoaded(levelChunk);
                    }
                }
            }

            return sceneLoadOperations;
        }

        public List<AsyncOperation> OnAdventureToPuzzleLevel(LevelZonesID nextPuzzleLevel)
        {
            List<AsyncOperation> sceneUnloadOperations = new List<AsyncOperation>();
            foreach (var referenceChunk in this.LevelManagementConfigurationGameObject.LevelHierarchyConfiguration.GetLevelHierarchy(LevelManagerRef.GetCurrentLevel()))
            {
                if (!this.LevelManagementConfigurationGameObject.LevelHierarchyConfiguration.GetLevelHierarchy(nextPuzzleLevel).Contains(referenceChunk))
                {
                    var unloadAsyncOperation = this.SceneUnloadIfNecessary(referenceChunk);
                    if (unloadAsyncOperation != null)
                    {
                        sceneUnloadOperations.Add(unloadAsyncOperation);
                    }
                }
            }

            return sceneUnloadOperations;
        }

        public List<AsyncOperation> LoadAllLevelsAsync(LevelZonesID nextPuzzleLevel)
        {
            return this.LoadAllLevels(nextPuzzleLevel);
        }

        private AsyncOperation SceneUnloadIfNecessary(LevelZoneChunkID levelChunk)
        {
            return SceneLoadingHelper.SceneUnLoadWIthoutDuplicates(this.LevelManagementConfigurationGameObject.ChunkZonesSceneConfiguration.GetSceneName(levelChunk));
        }
    }

    public enum LevelType
    {
        GAME,
        STARTMENU
    }
}