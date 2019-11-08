using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace CoreGame
{
    /*
 *     C5----C6
 *    / |    /|
 *   C1----C2 |
 *   |  C8  | C7   
 *   | /    |/     C3->C7  Forward
 *   C4----C3     
 */

    [Serializable]
    public struct FrustumPointsPositions
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;
        public Vector3 FC5;
        public Vector3 FC6;
        public Vector3 FC7;
        public Vector3 FC8;

        public Vector3 normal1;
        public Vector3 normal2;
        public Vector3 normal3;
        public Vector3 normal4;
        public Vector3 normal5;
        public Vector3 normal6;

        public FrustumPointsPositions(Vector3 fC1, Vector3 fC2, Vector3 fC3, Vector3 fC4, Vector3 fC5, Vector3 fC6, Vector3 fC7, Vector3 fC8)
        {
            FC1 = fC1;
            FC2 = fC2;
            FC3 = fC3;
            FC4 = fC4;
            FC5 = fC5;
            FC6 = fC6;
            FC7 = fC7;
            FC8 = fC8;

            float crossSign = Mathf.Sign(Vector3.Dot(FC5 - FC1, Vector3.Cross(FC2 - FC1, FC4 - FC1)));

            this.normal1 = crossSign * Vector3.Cross(FC2 - FC1, FC3 - FC1);
            this.normal2 = crossSign * Vector3.Cross(FC5 - FC1, FC2 - FC1);
            this.normal3 = crossSign * Vector3.Cross(FC6 - FC2, FC3 - FC2);
            this.normal4 = crossSign * Vector3.Cross(FC7 - FC3, FC4 - FC3);
            this.normal5 = crossSign * Vector3.Cross(FC8 - FC4, FC1 - FC4);
            this.normal6 = crossSign * Vector3.Cross(FC8 - FC5, FC6 - FC5);
        }

        public static int GetByteSize()
        {
            return (((8 * 3) + (6 * 3)) * sizeof(float));
        }


        public void DrawInScene(Color color)
        {
#if UNITY_EDITOR
            var oldColor = Handles.color;
            Handles.color = color;

            Handles.DrawLine(this.FC1, this.FC2);
            Handles.DrawLine(this.FC2, this.FC3);
            Handles.DrawLine(this.FC3, this.FC4);
            Handles.DrawLine(this.FC4, this.FC1);

            Handles.DrawLine(this.FC1, this.FC5);
            Handles.DrawLine(this.FC2, this.FC6);
            Handles.DrawLine(this.FC3, this.FC7);
            Handles.DrawLine(this.FC4, this.FC8);

            Handles.DrawLine(this.FC5, this.FC6);
            Handles.DrawLine(this.FC6, this.FC7);
            Handles.DrawLine(this.FC7, this.FC8);
            Handles.DrawLine(this.FC5, this.FC5);

            Handles.color = oldColor;
#endif
        }
    }

    public struct SingleFacePosition
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;

        public Vector2 normal1;

        public SingleFacePosition(Vector3 fc1, Vector3 fc2, Vector3 fc3, Vector3 fc4, Vector2 normal1)
        {
            FC1 = fc1;
            FC2 = fc2;
            FC3 = fc3;
            FC4 = fc4;
            this.normal1 = normal1;
        }
    }
}