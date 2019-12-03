using UnityEngine;
using UnityEngine.Rendering;

namespace LevelManagement
{
    public class TransitionableLevelFXType : MonoBehaviour
    {
        private Volume postProcessVolume;
        private Light mainDirectionalLight;

        public Volume PostProcessVolume
        {
            get => postProcessVolume;
        }

        public Light MainDirectionalLight
        {
            get => mainDirectionalLight;
        }

        public void Init()
        {
            this.postProcessVolume = GetComponentInChildren<Volume>();
            this.postProcessVolume.gameObject.SetActive(false);
            this.mainDirectionalLight = GetComponentInChildren<Light>();
            this.mainDirectionalLight.gameObject.SetActive(false);
        }
    }
}