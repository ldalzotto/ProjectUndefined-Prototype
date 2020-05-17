using System;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace Obstacle
{
    public class ObstacleListenerSystem
    {
        private List<ObstacleInteractiveObject> nearSquareObstacles;

        public ObstacleListenerSystem(Func<TransformStruct> RangeTransformProvider)
        {
            this.AssociatedRangeTransformProvider = RangeTransformProvider;

            #region External Dependencies

            ObstaclesListenerManager = ObstaclesListenerManager.Get();

            #endregion

            nearSquareObstacles = new List<ObstacleInteractiveObject>();
            ObstacleListenerUniqueID = ObstaclesListenerManager.OnObstacleListenerCreation(this);
        }

        public int ObstacleListenerUniqueID { get; private set; }


        #region Internal Dependencies

        public Func<TransformStruct> AssociatedRangeTransformProvider { get; private set; }

        #endregion


        public List<ObstacleInteractiveObject> NearSquareObstacles => nearSquareObstacles;

        public void OnObstacleListenerDestroyed()
        {
            Debug.Log(MyLog.Format("OnObstacleListenerDestroyed"));
            ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
            nearSquareObstacles.Clear();
        }

        #region Data Retrieval

        public void ForEachCalculatedFrustum(Action<FrustumPointsPositions> action)
        {
            foreach (var obstacleInteractiveObject in nearSquareObstacles)
            foreach (var obstacleFrustumPositions in ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustums(this, obstacleInteractiveObject))
                action(obstacleFrustumPositions);
        }

        #endregion

        public void AddNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            nearSquareObstacles.Add(ObstacleInteractiveObject);
        }

        public void RemoveNearSquareObstacle(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            nearSquareObstacles.Remove(ObstacleInteractiveObject);
        }

        #region External Dependencies

        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        private ObstaclesListenerManager ObstaclesListenerManager;

        #endregion
    }
}