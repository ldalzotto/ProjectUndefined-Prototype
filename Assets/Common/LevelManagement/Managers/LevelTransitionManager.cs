using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelManagement
{
    public class LevelTransitionManager : GameSingleton<LevelTransitionManager>
    {
        private bool isNewZoneLoading;

        #region External Events

        public virtual void OnPuzzleToPuzzleLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.LEVEL_TO_ANOTHER_LEVEL);
        }

        public virtual void OnStartMenuToLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.FROM_STARTMENU);
        }

        public virtual void RestartCurrentLevel()
        {
            OnLevelChange(default, LevelChangeType.RESTART_CURRENT_LEVEL);
        }

        private void OnLevelChange(LevelZonesID nextZone, LevelChangeType LevelChangeType)
        {
            isNewZoneLoading = true;

            List<AsyncOperation> chunkOperations = null;
            if (LevelChangeType == LevelChangeType.LEVEL_TO_ANOTHER_LEVEL)
            {
                chunkOperations = LevelManagerEventManager.Get().OnLevelToAnotherLevel(nextZone);
            }
            else if (LevelChangeType == LevelChangeType.RESTART_CURRENT_LEVEL)
            {
                nextZone = LevelManager.Get().LevelID;
                chunkOperations = LevelManagerEventManager.Get().RestartCurrentLevel();
            }
            else if (LevelChangeType == LevelChangeType.FROM_STARTMENU)
            {
                chunkOperations = LevelManagerEventManager.Get().OnStartMenuToLevel(nextZone);
            }

            foreach (var chunkOperation in chunkOperations)
            {
                chunkOperation.allowSceneActivation = false;
            }

            Coroutiner.Instance.StopAllCoroutines();
            Coroutiner.Instance.StartCoroutine(this.SceneTrasitionOperation(chunkOperations, nextZone));
        }

        private IEnumerator SceneTrasitionOperation(List<AsyncOperation> chunkOperations, LevelZonesID nextZone)
        {
            yield return new WaitForListOfAsyncOperation(chunkOperations);
            foreach (var chunkOperation in chunkOperations)
            {
                chunkOperation.allowSceneActivation = true;
            }

            isNewZoneLoading = false;
            var nextZoneSceneName = LevelManagementConfigurationGameObject.Get().LevelZonesSceneConfiguration.GetSceneName(nextZone);
            LoadNextLevelScene(nextZoneSceneName);
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextZoneSceneName));
        }

        /// <summary>
        /// This will destroy the GameManager object (thus calling it's destroy method). Then will initialize the new one via the awake method.
        /// </summary>
        private static void LoadNextLevelScene(string nextZoneSceneName)
        {
            SceneManager.LoadScene(nextZoneSceneName, LoadSceneMode.Single);
        }

        #endregion

        #region Logical Conditions

        public bool IsNewZoneLoading()
        {
            return isNewZoneLoading;
        }

        #endregion

        enum LevelChangeType
        {
            LEVEL_TO_ANOTHER_LEVEL,
            RESTART_CURRENT_LEVEL,
            FROM_STARTMENU
        }
    }
}