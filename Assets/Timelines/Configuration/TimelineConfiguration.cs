using System;
using ConfigurationEditor;
using UnityEngine;

namespace Timelines
{
    [Serializable]
    [CreateAssetMenu(fileName = "TimelineConfiguration", menuName = "Configuration/CoreGame/TimelineConfiguration/TimelineConfiguration", order = 1)]
    public class TimelineConfiguration : ConfigurationSerialization<TimelineID, TimelineInitializerScriptableObject>
    {
    }
}