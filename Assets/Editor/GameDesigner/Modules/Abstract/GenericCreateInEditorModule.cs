using UnityEngine;
using System.Collections;
using Editor_GameDesigner;
using Editor_MainGameCreationWizard;

namespace Editor_GameDesigner
{
    public class GenericCreateInEditorModule<CONFIGURATION> : IGameDesignerModule
    {
        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            if (GUILayout.Button("CREATE IN EDITOR"))
            {
                 GameCreationWizard.InitGenericCreator(typeof(CONFIGURATION));
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
