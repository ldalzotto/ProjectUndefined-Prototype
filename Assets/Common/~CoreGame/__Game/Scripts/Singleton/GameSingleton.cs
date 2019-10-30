using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CoreGame
{
    public interface IGameSingleton
    {
        void OnDestroy();
    }

    public abstract class GameSingleton<T> : IGameSingleton where T : IGameSingleton, new()
    {
        private static T Instance;

        public static T Get()
        {
            if (Instance == null)
            {
                Instance = new T();
                GameSingletonManagers.Get().OnGameSingletonCreated(Instance);
            }

            return Instance;
        }

#if UNITY_EDITOR
        public static void SetInstance(T t)
        {
            Instance = t;
        }
#endif

        public virtual void OnDestroy()
        {
            Instance = default;
        }
    }

    public class GameSingletonManagers
    {
        private static GameSingletonManagers Instance;

        public static GameSingletonManagers Get()
        {
            if (Instance == null)
            {
                Instance = new GameSingletonManagers();
            }

            return Instance;
        }

        private List<IGameSingleton> AllGameSingletons = new List<IGameSingleton>();

        public void OnGameSingletonCreated(IGameSingleton GameSingleton)
        {
            this.AllGameSingletons.Add(GameSingleton);
        }

        public void OnDestroy()
        {
            for (var i = this.AllGameSingletons.Count - 1; i >= 0; i--)
            {
                var gameSingleton = this.AllGameSingletons[i];
                if (gameSingleton != null)
                {
                    gameSingleton.OnDestroy();
                }
            }

            Instance = null;
        }
    }
}