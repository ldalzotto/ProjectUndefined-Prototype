using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;

namespace Obstacle
{
    public class ObstaclesListenerManager : GameSingleton<ObstaclesListenerManager>
    {
        private int ObstacleListenerAddedCounter = 0;

        private List<ObstacleListenerSystem> obstacleListeners = new List<ObstacleListenerSystem>();

        public int OnObstacleListenerCreation(ObstacleListenerSystem obstacleListener)
        {
            this.obstacleListeners.Add(obstacleListener);
            this.ObstacleListenerAddedCounter += 1;
            return this.ObstacleListenerAddedCounter;
        }

        public void OnObstacleListenerDestroyed(ObstacleListenerSystem obstacleListener)
        {
            this.obstacleListeners.Remove(obstacleListener);
        }

        #region Debug Display

        public void GizmoTick()
        {
            ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
            foreach (var obstacleListener in this.obstacleListeners)
            {
                ObstacleOcclusionCalculationManagerV2.TryGetCalculatedOcclusionFrustumsForObstacleListener(obstacleListener, out Dictionary<int, List<FrustumPointsPositions>> allCalculatedFrustumPositions);
                if (allCalculatedFrustumPositions != null)
                {
                    foreach (var calculatedFrustumPositions in allCalculatedFrustumPositions.Values)
                    {
                        foreach (var calculatedFrustumPosition in calculatedFrustumPositions)
                        {
                            calculatedFrustumPosition.DrawInScene(MyColors.GetColorOnIndex(obstacleListener.ObstacleListenerUniqueID));
                        }
                    }
                }
            }
        }

        #endregion

        #region Data Retrieval

        public List<ObstacleListenerSystem> GetAllObstacleListeners()
        {
            return this.obstacleListeners;
        }

        #endregion
    }
}