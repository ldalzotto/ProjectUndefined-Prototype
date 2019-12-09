using System;
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

        private void Start()
        {
            base.OnStart();
        }

        private void FixedUpdate()
        {
            base.BeforeFixedTickGameLogic(out float d, out float unscaled);
        }

        private void Update()
        {
            base.BeforeTickGameLogic(out float d, out float unscaled);
            StartMenuManager.Get().Tick(d);
        }

        private void LateUpdate()
        {
            base.BeforeLateTickGameLogic(out float d, out float unscaled);
        }
    }
}