using System;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;
using UnityEngine.UI;

namespace TimeManagement
{
    [Serializable]
    public class TimeManagementConfiguration : SerializedScriptableObject
    {
        public Image TimePausedIconPrefab;
    }
}