using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace Persistence
{
    public class AutoSaveIcon
    {
        private static AutoSaveIcon Instance;

        public static AutoSaveIcon Get()
        {
            if (Instance == null)
            {
                Instance = new AutoSaveIcon();
            }

            return Instance;
        }


        #region Internal Dependencies

        private GameObject AutoSaveIconGameObject;
        private Image autoSaveIcon;

        #endregion

        private AutoSaveIcon()
        {
            if (this.AutoSaveIconGameObject == null)
            {
                this.AutoSaveIconGameObject = MonoBehaviour.Instantiate(PersistanceConfigurationGameObject.Get().PersistanceConfiguration.AutoSaveIconPrefab, CoreGameSingletonInstances.PersistantCanvas().transform);
            }

            this.autoSaveIcon = this.AutoSaveIconGameObject.GetComponent<Image>();
            this.autoSaveIcon.enabled = false;
        }

        #region External events

        public void OnSaveStart()
        {
            this.autoSaveIcon.enabled = true;
        }

        public void OnSaveEnd()
        {
            this.autoSaveIcon.enabled = false;
        }

        #endregion
    }
}