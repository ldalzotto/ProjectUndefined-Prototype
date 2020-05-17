using static CoreGame.CoreGameSingletonInstances;

namespace StartMenu
{
    public static class StartMenuSingletonInstances
    {
        private static GameProgressionStateManager gameProgressionStateManager;
        private static StartMenuStaticConfigurationManager startMenuStaticConfigurationManager;

        public static GameProgressionStateManager GameProgressionStateManager { get => NewInstanceIfNull(gameProgressionStateManager, (obj) => gameProgressionStateManager = obj); }
        public static StartMenuStaticConfigurationManager StartMenuStaticConfigurationManager { get => FindAndSetInstanceIfNull(startMenuStaticConfigurationManager, (obj) => startMenuStaticConfigurationManager = obj); }
    }
}
