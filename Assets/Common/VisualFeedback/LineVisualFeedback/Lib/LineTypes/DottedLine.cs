using CoreGame;
using UnityEngine;

namespace VisualFeedback
{
    public class DottedLine
    {
        private DottedLineInherentData dottedLineInherentData;
        private DottedLineGameObject dottedLineGameObject;

        #region External Dependencies

        private DottedLineRendererManager DottedLineRendererManager = DottedLineRendererManager.Get();

        #endregion

        public DottedLine(DottedLineID dottedLineID)
        {
            this.dottedLineInherentData = VisualFeedbackConfigurationGameObject.Get().DottedLineConfiguration.ConfigurationInherentData[dottedLineID];
            this.dottedLineGameObject = new DottedLineGameObject(VisualFeedbackConfigurationGameObject.Get().DottedLineStaticConfiguration.BaseDottedLineShader, this.dottedLineInherentData);
        }

        public void OnDestroy()
        {
            if (this.dottedLineGameObject != null)
            {
                MonoBehaviour.Destroy(this.dottedLineGameObject.AssociatedGameObject);
            }
        }

        public Mesh GetMesh()
        {
            return this.dottedLineGameObject.GetMesh();
        }

        public int GetUniqueID()
        {
            return this.dottedLineGameObject.GetInstanceID();
        }

        #region State

        private BeziersControlPoints BeziersControlPoints;
        private float currentPosition = 0f;
        private Vector3 LastFrameWorldSpaceStartPoint;
        private Vector3 LastFrameWorldSpaceEndPoint;

        #endregion

        public void Tick(float d, Vector3 worldSpaceStartPoint, Vector3 worldSpaceEndPoint)
        {
            if ((worldSpaceStartPoint != LastFrameWorldSpaceStartPoint) || (worldSpaceEndPoint != LastFrameWorldSpaceEndPoint))
            {
                if (Vector3.Distance(worldSpaceStartPoint, worldSpaceEndPoint) <= 2.5f)
                {
                    this.ClearLine();
                }
                else
                {
                    this.RePositionLine(worldSpaceStartPoint, worldSpaceEndPoint, Vector3.up);
                }
            }

            this.currentPosition += d;
            if (this.currentPosition > 1)
            {
                this.currentPosition = 1 - this.currentPosition;
            }

            //The beziers control points can be null because DottedLine can be instanciated and immediately clear line. Thus, this.RePositionLine is not called,
            //thus, beziers calculation is not triggered.
            if (this.BeziersControlPoints != null)
            {
                this.dottedLineGameObject.SetColorPointPosition(this.BeziersControlPoints.ResolvePoint(this.currentPosition));
            }

            this.LastFrameWorldSpaceStartPoint = worldSpaceStartPoint;
            this.LastFrameWorldSpaceEndPoint = worldSpaceEndPoint;
        }

        public ComputeBeziersInnerPointEvent BuildComputeBeziersInnerPointEvent()
        {
            return new ComputeBeziersInnerPointEvent(this.dottedLineGameObject.GetInstanceID(), this.BeziersControlPoints, this.dottedLineInherentData.ModelScale, this.dottedLineInherentData.DotPerUnitDistance);
        }

        private void RePositionLine(Vector3 worldSpaceStartPoint, Vector3 worldSpaceEndPoint, Vector3 normal)
        {
            BeziersControlPointsShape BeziersControlPointsShape = BeziersControlPointsShape.CURVED;
            if (this.dottedLineInherentData.DottedLineType == DottedLineType.STRAIGHT)
            {
                BeziersControlPointsShape = BeziersControlPointsShape.STRAIGHT;
            }

            this.BeziersControlPoints = BeziersControlPoints.Build(worldSpaceStartPoint, worldSpaceEndPoint, Vector3.up, BeziersControlPointsShape);
            this.DottedLineRendererManager.OnComputeBeziersInnerPointEvent(this);
        }

        private void ClearLine()
        {
            this.dottedLineGameObject.ClearMesh();
        }

        private void OnDrawGizmos()
        {
            if (this.BeziersControlPoints != null)
            {
                Gizmos.DrawWireSphere(this.BeziersControlPoints.P0, 0.5f);
                Gizmos.DrawWireSphere(this.BeziersControlPoints.P3, 0.5f);
            }
        }
    }
}