using Editor_LevelChunkCreationWizard;
using Editor_MainGameCreationWizard;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreateChunk : IGameDesignerModule
    {
        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            if (GUILayout.Button("CREATE CHUNK IN EDITOR"))
            {
                GameCreationWizard.InitWithSelected(nameof(LevelChunkCreationWizard));
            }
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}