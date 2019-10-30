using System.Collections.Generic;
using CoreGame;

namespace Obstacle
{
    /// <summary>
    /// Assign a unique ID to an Obstacle and tracking all Obstacles in game.
    /// </summary>
    public class ObstacleInteractiveObjectManager : GameSingleton<ObstacleInteractiveObjectManager>
    {
        public List<ObstacleInteractiveObject> AllObstacleInteractiveObjects { get; private set; } = new List<ObstacleInteractiveObject>();
        public int SquareObstacleSystemAddedCounter { get; private set; } = 0;

        public int OnSquareObstacleSystemCreated(ObstacleInteractiveObject obstacleInteractiveObject)
        {
            this.AllObstacleInteractiveObjects.Add(obstacleInteractiveObject);
            this.SquareObstacleSystemAddedCounter += 1;
            return this.SquareObstacleSystemAddedCounter;
        }

        public void OnSquareObstacleSystemDestroyed(ObstacleInteractiveObject obstacleInteractiveObject)
        {
            this.AllObstacleInteractiveObjects.Remove(obstacleInteractiveObject);
        }
    }
}