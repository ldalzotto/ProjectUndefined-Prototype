using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CoreGame
{
    public interface IGameSingleton
    {
        void OnDestroy();
        void ClearInstance();
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
            if (Instance != null)
            {
                Instance.OnDestroy();
            }
            Instance = t;
        }
#endif

        public virtual void OnDestroy(){}

        /// <summary>
        /// Unreferencing the singleton instance so that a new one is created on load. This method is called when <see cref="GameSingletonManagers.OnDestroy"/> is called
        /// usually when a level is unloaded.
        /// </summary>
        public void ClearInstance()
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
                    /// First calling the destroy callback
                    gameSingleton.OnDestroy();
                    
                    /// Unreferencing the singleton instance so that a new one is created on load
                    gameSingleton.ClearInstance();
                }
            }

            Instance = null;
        }
    }
}