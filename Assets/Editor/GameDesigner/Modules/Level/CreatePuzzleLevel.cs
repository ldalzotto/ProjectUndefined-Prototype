using UnityEngine;
using System.Collections;
using Editor_MainGameCreationWizard;
using Editor_PuzzleLevelCreationWizard;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreatePuzzleLevel : IGameDesignerModule
    {
        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            if(GUILayout.Button("CREATE LEVEL IN EDITOR"))
            {
                GameCreationWizard.InitWithSelected(nameof(PuzzleLevelCreationWizard));
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