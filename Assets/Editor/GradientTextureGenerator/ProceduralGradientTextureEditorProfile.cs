using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class ProceduralGradientTextureEditorProfile : SerializedScriptableObject
    {
        [ReorderableList()] public List<Gradient> AllGradients;
    }
}