using SequencedAction;
using UnityEditor;

namespace SequencedAction_Editor
{
    [CustomEditor(typeof(ASequencedActionGraph), editorForChildClasses: true)]
    public class ASequencedActionCustomEditor : Editor
    {
        private void OnEnable()
        {
            this.RegisterCallback();
        }

        private void OnDisable()
        {
            this.UnRegisterCallback();
        }

        private void OnDestroy()
        {
            this.UnRegisterCallback();
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