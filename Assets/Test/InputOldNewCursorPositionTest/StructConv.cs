using System;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IVector3Action
    {
        Action<Vector3> Vector3Action { get; }
    }

    public struct CallbacksStruct : IVector3Action
    {
        public Action<Vector3> Vector3Action { get; }
        public Action<BoxCollider> BoxColliderAction { get; }

        public CallbacksStruct(Action<Vector3> vector3Action, Action<BoxCollider> boxColliderAction)
        {
            Vector3Action = vector3Action;
            BoxColliderAction = boxColliderAction;
        }
    }

    public class StructConv : MonoBehaviour
    {
        private void Start()
        {
            var CallbacksStruct = new CallbacksStruct((delegate(Vector3 vector3) { Debug.Log(vector3.ToString()); }), null);
            var IVector3Action = CallbacksStruct as IVector3Action;
            IVector3Action.Vector3Action.Invoke(new Vector3(1, 1, 1));
        }
    }
}