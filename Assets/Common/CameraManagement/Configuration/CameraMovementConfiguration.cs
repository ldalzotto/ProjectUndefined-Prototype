using System;
using OdinSerializer;

namespace CameraManagement
{
    
    [Serializable]
    public class CameraFollowManagerComponent
    {
        public float DampTime;
    }
    
    [Serializable]
    public class CameraMovementConfiguration : SerializedScriptableObject
    {
        public CameraFollowManagerComponent CameraFollowManagerComponent;
    }
}