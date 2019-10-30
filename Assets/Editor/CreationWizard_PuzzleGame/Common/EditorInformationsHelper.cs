using System;
using System.Collections.Generic;
using System.Linq;
using ConfigurationEditor;
using CreationWizard;
using LevelManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor_MainGameCreationWizard
{
    public class EditorInformationsHelper
    {
        public static void InitProperties(ref CommonGameConfigurations CommonGameConfigurations)
        {
            #region Puzzle Common Prefabs

            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, "_GameManagers_Persistance_Instanciater");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, "CorePuzzleSceneElements");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics, "BasePuzzleLevelDynamics");
            AssetFinder.SafeSingleAssetFind(ref CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab, "BaseLevelprefab");

            #endregion

            //TODO configuration initialization
            if (CommonGameConfigurations.Configurations.Count == 0)
                foreach (var configurationType in TypeHelper.GetAllGameConfigurationTypes())
                    CommonGameConfigurations.Configurations.Add(configurationType, (IConfigurationSerialization) AssetFinder.SafeAssetFind("t:" + configurationType.Name)[0]);
        }

        public static string ComputeErrorState(ref CommonGameConfigurations CommonGameConfigurations)
        {
            return NonNullityFieldCheck(NonNullityFieldCheck(CommonGameConfigurations.PuzzleLevelCommonPrefabs))
                .ToList()
                .Find((s) => !string.IsNullOrEmpty(s));
        }

        private static List<string> NonNullityFieldCheck(object containerObjectToCheck)
        {
            return containerObjectToCheck.GetType().GetFields().ToList().ConvertAll(field => { return ErrorHelper.NonNullity((Object) field.GetValue(containerObjectToCheck), field.Name); });
        }
    }

    [Serializable]
    public class CommonGameConfigurations
    {
        public Dictionary<Type, IConfigurationSerialization> Configurations;

        public PuzzleLevelCommonPrefabs PuzzleLevelCommonPrefabs;

        public CommonGameConfigurations()
        {
            PuzzleLevelCommonPrefabs = new PuzzleLevelCommonPrefabs();
            Configurations = new Dictionary<Type, IConfigurationSerialization>();
        }

        public T GetConfiguration<T>() where T : IConfigurationSerialization
        {
            return (T) Configurations[typeof(T)];
        }

        public IConfigurationSerialization GetConfiguration(Type configurationType)
        {
            return Configurations[configurationType];
        }
    }


    [Serializable]
    public class PuzzleLevelCommonPrefabs
    {
        [MyReadOnly] public LevelChunkInteractiveObjectInitializer BaseLevelChunkPrefab;
        [MyReadOnly] public LevelManager BasePuzzleLevelDynamics;
        [MyReadOnly] public GameObject CorePuzzleSceneElements;
        [MyReadOnly] public GameManagerPersistanceInstance GameManagerPersistanceInstance;
    }
}