using GameLoop;
using Input;
using LevelManagement;
using UnityEngine;

namespace StartMenu
{
    public class StartMenuGameManager : AsbtractCoreGameManager
    {
        private void Awake()
        {
            FindObjectOfType<GameManagerPersistanceInstance>().Init();
            this.OnAwake(LevelType.STARTMENU);
            StartMenuSingletonInstances.GameProgressionStateManager.Init();
            StartMenuManager.Get().Init();
        }

        private void Update()
        {
            var d = Time.deltaTime;
            this.BeforeTick(d);
            StartMenuManager.Get().Tick(d);
        }
    }
}