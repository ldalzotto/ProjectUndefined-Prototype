using System;
using OdinSerializer;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    /// <summary>
    /// Holds <see cref="TimeScale"/> that change game delta time speed.
    /// /!\ This ScriptableObject is used as a GUI for test scene. It's values mays be subjected to change at runtime.
    /// </summary>
    [Serializable]
    public class TestGameControlDefinition : SerializedScriptableObject
    {
        [Range(0f, 2f)] public float TimeScale;
    }
}