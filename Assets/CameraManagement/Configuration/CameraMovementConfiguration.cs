﻿using System;
using OdinSerializer;

namespace CameraManagement
{
    [Serializable]
    public class CameraMovementConfiguration : SerializedScriptableObject
    {
        public CameraFollowManagerComponent CameraFollowManagerComponent;
    }
}