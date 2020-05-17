using ConfigurationEditor;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    public interface IConfigurationModule {
        void SetSearchString(string searchString);
    }

    public class ConfigurationModule<CONFIGURATION, ID, VALUE> : IGameDesignerModule, IConfigurationModule where ID : Enum where VALUE : ScriptableObject where CONFIGURATION : IConfigurationSerialization
    {
        [SerializeField]
        private IGenericConfigurationEditor configurationEditor;

        public IGenericConfigurationEditor ConfigurationEditor { get => configurationEditor; }

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.configurationEditor.GUITick();
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
            Debug.Log("t:" + typeof(CONFIGURATION).Name);
            this.configurationEditor = new GenericConfigurationEditor<ID, VALUE>("t:" + typeof(CONFIGURATION).Name);
        }

        public void SetSearchString(string searchString)
        {
            this.configurationEditor.SetSearchString(searchString);
        }
    }
}