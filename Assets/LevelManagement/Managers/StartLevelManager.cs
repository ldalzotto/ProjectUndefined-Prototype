using System;
using CoreGame;
using LevelManagement;
using Persistence;

namespace LevelManagement
{
    public class StartLevelManager : GameSingleton<StartLevelManager>
    {
        private StartLevelPersistanceManager StartLevelPersistanceManager;
        private StartLevel StartLevel;

        public void Init()
        {
            this.StartLevelPersistanceManager = new StartLevelPersistanceManager();
            this.StartLevel = this.StartLevelPersistanceManager.Load();
            if (this.StartLevel == null)
            {
                this.StartLevel = new StartLevel(LevelZonesID.NONE);
                this.StartLevelPersistanceManager.SaveAsync(this.StartLevel);
            }
        }

        #region Exnternal Events

        public void OnStartLevelChange(LevelZonesID startLevelID)
        {
            this.StartLevel.StartLevelID = startLevelID;
            this.StartLevelPersistanceManager.SaveAsync(this.StartLevel);
        }

        #endregion

        public LevelZonesID GetStartLevelID()
        {
            return this.StartLevel.StartLevelID;
        }
    }


    public class StartLevelPersistanceManager : AbstractGamePersister<StartLevel>
    {
        public const string FolderName = "StartLevel";
        public const string FileExtension = ".lvl";
        public const string FileName = "StartLevel";

        public StartLevelPersistanceManager() : base(FolderName, FileExtension, FileName)
        {
        }
    }
}

namespace Persistence
{
    [Serializable]
    public class StartLevel
    {
        public LevelZonesID StartLevelID;

        public StartLevel()
        {
        }

        public StartLevel(LevelZonesID startLevelID)
        {
            StartLevelID = startLevelID;
        }
    }
}