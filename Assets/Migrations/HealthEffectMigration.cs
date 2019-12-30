using Health;
using ParticleObjects;
using UnityEditor;

namespace DefaultNamespace
{
    public class HealthEffectMigration : EditorWindow
    {
        [MenuItem("Migration/HealthEffectMigration")]
        public static void Start()
        {
            var HealthSystemDefinitions = AssetFinder.SafeAssetFind<HealthSystemDefinition>("t:HealthSystemDefinition");
            var defaultParticle = AssetFinder.SafeSingleAssetFind<ParticleObjectDefinition>("GreenCross_HealthRecoveryParticleEffect");
            foreach (var HealthSystemDefinition in HealthSystemDefinitions)
            {
                HealthSystemDefinition.HealthRecoveryParticleEffect = defaultParticle;
                EditorUtility.SetDirty(defaultParticle);
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}