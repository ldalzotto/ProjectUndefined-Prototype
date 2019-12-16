using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public struct CopyInteractiveObjectTransformConstraint
    {
        private IInteractiveGameObject FollowingIInteractiveGameObject;
        private Transform SourceTrasform;

        public CopyInteractiveObjectTransformConstraint(IInteractiveGameObject followingIInteractiveGameObject, Transform sourceTrasform)
        {
            FollowingIInteractiveGameObject = followingIInteractiveGameObject;
            SourceTrasform = sourceTrasform;
        }

        public void ApplyConstraint()
        {
            FollowingIInteractiveGameObject.InteractiveGameObjectParent.transform.position = this.SourceTrasform.position;
            FollowingIInteractiveGameObject.InteractiveGameObjectParent.transform.rotation = this.SourceTrasform.rotation;
        }
    }
}