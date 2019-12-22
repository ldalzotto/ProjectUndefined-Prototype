using CoreGame;
using Health;
using InteractiveObjects;
using UnityEngine;
using UnityEngine.AI;

namespace HealthGlobe
{
    public static class HealthGlobeSpawnCalculation
    {
        public static void HealthGlobeSpawn(CoreInteractiveObject SourceInteractiveObject, HealthGlobeSpawnDefinition HealthGlobeSpawnDefinition)
        {
            /// Defining the number of globes generated
            int NumberOfGlobe = Random.Range(HealthGlobeSpawnDefinition.MinNumberOfGlobes, HealthGlobeSpawnDefinition.MaxNumberOfGlobes);

            /// We ensure that the source point is positioned on the navmesh. This is to avoid failure in navMeshRaycast where the source
            /// is not on the walkable navmesh

            NavMesh.SamplePosition(SourceInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition, out NavMeshHit hit, 10f, 1 << NavMesh.GetAreaFromName(NavMeshConstants.WALKABLE_LAYER));
            if (hit.hit)
            {
                Vector3 SourcePosition = hit.position;

                /// Begin Calculating positions

                for (var i = 0; i < NumberOfGlobe; i++)
                {
                    var HealthGlobeSpawnInitializationData = new HealthGlobeSpawnInitializationData();

                    float rotationAngle = Random.Range(0, 360f);
                    float distance = Random.Range(HealthGlobeSpawnDefinition.MinDistanceFromSource, HealthGlobeSpawnDefinition.MaxDistanceFromSource);

                    var TargetPosition = SourcePosition + (Quaternion.AngleAxis(rotationAngle, SourceInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.up.normalized)
                                                           * SourceInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.forward.normalized * distance);

                    NavMesh.Raycast(SourcePosition, TargetPosition, out hit, 1 << NavMesh.GetAreaFromName(NavMeshConstants.WALKABLE_LAYER));


                    HealthGlobeSpawnInitializationData.WorldPosition = hit.position;
                    HealthGlobeSpawnInitializationData.HealthRecovered = HealthGlobeSpawnDefinition.RecoveredHealth;
                    SpawnHealthGlobeGameObject.SpawnHealthGlobeWithDefaultModel(SourceInteractiveObject, HealthGlobeSpawnInitializationData);
                }

                /// End Calculating positions
            }
        }
    }

    public struct HealthGlobeSpawnInitializationData
    {
        public Vector3 WorldPosition;
        public float HealthRecovered;
    }

    public static class SpawnHealthGlobeGameObject
    {
        public static CoreInteractiveObject SpawnHealthGlobeWithDefaultModel(CoreInteractiveObject SourceInteractiveObject,
            HealthGlobeSpawnInitializationData HealthGlobeSpawnInitializationData)
        {
            var GlobalHealthGlobeConfiguration = GlobalHealthGlobeConfigurationGameObject.Get().GlobalHealthGlobeConfiguration;
            var healthGlobeGO = GameObject.Instantiate(GlobalHealthGlobeConfiguration.HealthGlobeDefaultModelPrefab);

            var BeziersControlPointsBuildInput = new BeziersControlPointsBuildInput(
                SourceInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().center, HealthGlobeSpawnInitializationData.WorldPosition, Vector3.up, BeziersControlPointsShape.CURVED, Random.Range(2.5f, 3f));

            //      healthGlobeGO.transform.position = HealthGlobeSpawnInitializationData.WorldPosition;
            healthGlobeGO.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            var IInteractiveGameObject = InteractiveGameObjectFactory.Build(healthGlobeGO);
            var HealthGlobeInteractiveObjectDefinition = new HealthGlobeInteractiveObjectDefinition();
            var RecoveringHealthEmitterSystemDefinition = new RecoveringHealthEmitterSystemDefinition();
            RecoveringHealthEmitterSystemDefinition.RecoveredHealth = HealthGlobeSpawnInitializationData.HealthRecovered;
            RecoveringHealthEmitterSystemDefinition.RecveringHealthTriggerDefinition = GlobalHealthGlobeConfiguration.HealthGlobeDefaultRangeDefinition;
            HealthGlobeInteractiveObjectDefinition.RecoveringHealthEmitterSystemDefinition = RecoveringHealthEmitterSystemDefinition;

            return new HealthGlobeInteractiveObject(HealthGlobeInteractiveObjectDefinition, IInteractiveGameObject, BeziersControlPointsBuildInput);
        }
    }
}