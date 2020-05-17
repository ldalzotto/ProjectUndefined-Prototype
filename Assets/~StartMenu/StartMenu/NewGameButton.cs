using System.IO;
using LevelManagement;
using UnityEngine;
using UnityEngine.UI;

namespace StartMenu
{
    public class NewGameButton : MonoBehaviour
    {
        private void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                //Destroy all saved data
                var persistanceDirectory = new DirectoryInfo(Application.persistentDataPath);
                foreach (var directory in persistanceDirectory.GetDirectories())
                {
                    directory.Delete(true);
                }

                LevelTransitionManager.Get().OnStartMenuToLevel(StartLevelManager.Get().GetStartLevelID());
            });
        }
    }
}