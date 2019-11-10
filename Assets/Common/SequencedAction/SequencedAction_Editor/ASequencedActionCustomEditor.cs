using System.Reflection;
using SequencedAction;
using UnityEditor;

namespace SequencedAction_Editor
{
    [CustomEditor(typeof(ASequencedActionGraph), editorForChildClasses: true)]
    public class ASequencedActionCustomEditor : Editor
    {
        private SceneHandleDrawAttribute SceneHandleDrawAttribute;

        private void OnEnable()
        {
            this.SceneHandleDrawAttribute = this.target.GetType().GetCustomAttribute<SceneHandleDrawAttribute>(true);
            if (this.SceneHandleDrawAttribute != null)
            {
                DynamicEditorCreation.Get().CreatedEditors.Add(this);
                this.RegisterCallback();
            }
        }

        private void OnDisable()
        {
            if (this.SceneHandleDrawAttribute != null)
            {
                DynamicEditorCreation.Get().CreatedEditors.Remove(this);
                this.UnRegisterCallback();
            }
        }

        private void OnDestroy()
        {
            if (this.SceneHandleDrawAttribute != null)
            {
                DynamicEditorCreation.Get().CreatedEditors.Remove(this);
                this.UnRegisterCallback();
            }
        }

        private void RegisterCallback()
        {
            SceneView.duringSceneGui += this.SceneTick;
        }

        private void UnRegisterCallback()
        {
            SceneView.duringSceneGui -= this.SceneTick;
        }

        private void SceneTick(SceneView sceneView)
        {
            SceneHandlerDrawer.Draw(target, null, null);
        }
    }
}