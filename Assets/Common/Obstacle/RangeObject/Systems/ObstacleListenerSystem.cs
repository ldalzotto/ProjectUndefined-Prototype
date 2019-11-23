using System;
using System.Collections.Generic;
using GeometryIntersection;
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
            /// Debug.Log(MyLog.Format("OnObstacleListenerDestroyed"));
            ObstaclesListenerManager.OnObstacleListenerDestroyed(this);
            nearSquareObstacles.Clear();
        }

        #region Data Retrieval

        public void ForEachCalculatedFrustum(Action<FrustumPointsPositions> action)
        {
            foreach (var obstacleInteractiveObject in nearSquareObstacles)
            {
                var CalculatedOcclusionFrustums = ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustums(this, obstacleInteractiveObject);
                if (CalculatedOcclusionFrustums != null)
                {
                    foreach (var obstacleFrustumPositions in CalculatedOcclusionFrustums)
                    {
                        action(obstacleFrustumPositions);
                    } 
                }
            }
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

        public void RemoveReferencesToObstacleInteractiveObject(ObstacleInteractiveObject ObstacleInteractiveObject)
        {
            for (var i = this.nearSquareObstacles.Count - 1; i >= 0; i--)
            {
                if (this.nearSquareObstacles[i] == ObstacleInteractiveObject)
                {
                    this.nearSquareObstacles.RemoveAt(i);
                }
            }
        }

        #region External Dependencies

        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        private ObstaclesListenerManager ObstaclesListenerManager;

        #endregion
    }
}