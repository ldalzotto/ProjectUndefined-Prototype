using System;
using OdinSerializer;
using UnityEngine;

namespace Editor_GameDesigner
{
    [Serializable]
    [CreateAssetMenu(fileName = "GameDesignerEditorProfile", menuName = "GameDesigner/GameDesignerEditorProfile", order = 1)]
    public class GameDesignerEditorProfile : SerializedScriptableObject
    {
        public int GameDesignerProfileInstanceIndex = 0;
        public GameDesignerTreePickerProfile GameDesignerTreePickerProfile;
        public IGameDesignerModule CurrentGameDesignerModule;
        public Vector2 ScrollPosition;

        public void ChangeCurrentModule(IGameDesignerModule nextModule)
        {
            if (this.CurrentGameDesignerModule != null)
            {
                this.CurrentGameDesignerModule.OnDisabled();
            }

            this.CurrentGameDesignerModule = nextModule;
            this.CurrentGameDesignerModule.OnEnabled();
        }
    }

    [Serializable]
    public class GameDesignerTreePickerProfile
    {
        public string SelectedKey;
    }
}