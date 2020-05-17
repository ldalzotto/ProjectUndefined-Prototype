using System;
using LevelManagement;
using RTPuzzle;
using UnityEngine;

namespace Editor_GameDesigner
{
    [Serializable]
    public class EditPuzzleLevel : EditScriptableObjectModule<LevelManager>
    {
        private LevelConfiguration levelConfiguration;

        protected override Func<LevelManager, ScriptableObject> scriptableObjectResolver
        {
            get { return (LevelManager LevelManager) => { return levelConfiguration.ConfigurationInherentData[LevelManager.LevelID]; }; }
        }

        public override void OnEnabled()
        {
            levelConfiguration = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration));
        }
    }
}