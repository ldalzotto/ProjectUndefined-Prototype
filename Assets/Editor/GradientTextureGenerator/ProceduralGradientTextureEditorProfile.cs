using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public class ProceduralGradientTextureEditorProfile : SerializedScriptableObject
    {
        public Gradient MyGradient;
        [ReorderableList()]
        public List<Gradient> AllGradients;
    }
}