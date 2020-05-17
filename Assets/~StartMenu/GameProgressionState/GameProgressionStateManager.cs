using LevelManagement;

namespace StartMenu
{
    public interface IGameProgressionStateManagerDataRetriever
    {
        bool HasAlreadyPlayed();
    }

    public class GameProgressionStateManager : IGameProgressionStateManagerDataRetriever
    {
        #region External Dependencies

        private StartLevelManager StartLevelManager = StartLevelManager.Get();

        #endregion

        public void Init()
        {
        }

        #region IGameProgressionStateManagerDataRetriever

        public bool HasAlreadyPlayed()
        {
            return this.StartLevelManager.GetStartLevelID() != LevelZonesID.NONE;
        }

        #endregion
    }
}