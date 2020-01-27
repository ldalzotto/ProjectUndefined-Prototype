using ParticleObjects;
using SightVisualFeedback;
using TrainingLevel;
using UnityEditor;

namespace DefaultNamespace
{
    public class SightVisualFeedbackMigration : EditorWindow
    {
        [MenuItem("Migration/SightVisualFeedbackMigration")]
        public static void Init()
        {  
            var SoliderEnemyDefinitions = AssetFinder.SafeAssetFind<SoliderEnemyDefinition>("t:SoliderEnemyDefinition");
            var defaultSightVisualFeedback = AssetFinder.SafeSingleAssetFind<SightVisualFeedbackSystemDefinition>("SightVisualFeedbackSystemDefinition");
            foreach (var soliderEnemyDefinition in SoliderEnemyDefinitions)
            {
                soliderEnemyDefinition.SightVisualFeedbackSystemDefinition = defaultSightVisualFeedback;
                EditorUtility.SetDirty(defaultSightVisualFeedback);
            }

            AssetDatabase.SaveAssets();
        }
    }
}