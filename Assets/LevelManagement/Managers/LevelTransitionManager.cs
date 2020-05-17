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

        public void OnPuzzleToPuzzleLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.PUZZLE_TO_PUZZLE);
        }

        public void OnStartMenuToLevel(LevelZonesID nextZone)
        {
            OnLevelChange(nextZone, LevelChangeType.FROM_STARTMENU);
        }

        private void OnLevelChange(LevelZonesID nextZone, LevelChangeType LevelChangeType)
        {
            isNewZoneLoading = true;

            List<AsyncOperation> chunkOperations = null;
            if (LevelChangeType == LevelChangeType.PUZZLE_TO_PUZZLE)
            {
                chunkOperations = LevelManagerEventManager.Get().OnPuzzleToPuzzleLevel(nextZone);
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
            SceneManager.LoadScene(nextZoneSceneName, LoadSceneMode.Single);
            yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextZoneSceneName));
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
            PUZZLE_TO_PUZZLE,
            FROM_STARTMENU
        }
    }
}