#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    public interface IGenericConfigurationEditor
    {
        void GUITick();
        void SetSearchString(string searchString);
    }

    [System.Serializable]
    public class GenericConfigurationEditor<K, DATA> : IGenericConfigurationEditor where K : Enum where DATA : ScriptableObject
    {
        [SerializeField]
        private ConfigurationSerialization<K, DATA> LaunchProjectileInherentDataConfiguration;

        [SerializeField]
        private string assetSearchFilter;

        public GenericConfigurationEditor(string assetSearchFilter)
        {
            this.assetSearchFilter = assetSearchFilter;
        } 

        #region Projectile dictionary configuration
        [SerializeField]
        private DictionaryEnumGUI<K, DATA> projectilesConf = new DictionaryEnumGUI<K, DATA>();

        public DictionaryEnumGUI<K, DATA> ProjectilesConf { get => projectilesConf; }
        #endregion

        public void GUITick()
        {
            if (LaunchProjectileInherentDataConfiguration == null)
            {
                LaunchProjectileInherentDataConfiguration = AssetFinder.SafeSingleAssetFind<ConfigurationSerialization<K, DATA>>(this.assetSearchFilter);
            }

            EditorGUI.BeginChangeCheck();
            LaunchProjectileInherentDataConfiguration =
                EditorGUILayout.ObjectField(this.LaunchProjectileInherentDataConfiguration, typeof(ConfigurationSerialization<K, DATA>), false) as ConfigurationSerialization<K, DATA>;
            if (EditorGUI.EndChangeCheck())
            {
                this.projectilesConf.RequestClearEditorCache();
            }

            EditorGUI.BeginChangeCheck();
            if (LaunchProjectileInherentDataConfiguration != null)
            {
                this.projectilesConf.GUITick(ref this.LaunchProjectileInherentDataConfiguration.ConfigurationInherentData);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this.LaunchProjectileInherentDataConfiguration);
            }

        }

        public void SetSearchString(string searchString)
        {
            this.projectilesConf.SetSearchFilter(searchString);
        }
    }
}
#endif