using UnityEngine;
using System.Collections;
using Editor_MainGameCreationWizard;

namespace Editor_GameDesigner
{
    public class CreateInEditorModule<CREATION_WIZARD> : IGameDesignerModule
    {
        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            if (GUILayout.Button("CREATE IN EDITOR"))
            {
               // GameCreationWizard.InitGenericCreator(typeof(CREATION_WIZARD));
                GameCreationWizard.InitWithSelected(typeof(CREATION_WIZARD).Name);
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