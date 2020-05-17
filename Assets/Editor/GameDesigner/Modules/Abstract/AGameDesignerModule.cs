using UnityEngine;
using System.Collections;

namespace Editor_GameDesigner
{
    public interface IGameDesignerModule 
    {
        void OnEnabled();
        void OnDisabled();
        void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile);
    }
}

