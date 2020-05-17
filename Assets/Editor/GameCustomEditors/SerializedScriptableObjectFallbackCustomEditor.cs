using UnityEngine;
using System.Collections;
using UnityEditor;
using OdinSerializer;

[CustomEditor(typeof(SerializedScriptableObject), editorForChildClasses: true, isFallback = true)]
public class SerializedScriptableObjectFallbackCustomEditor : Editor
{
}
