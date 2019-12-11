using System;
using AnimatorPlayable;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Serialization;

namespace InteractiveObject_Animation
{
    [Serializable]
    public class CommonBipedAnimatorBehaviorDefinition : SerializedScriptableObject
    {
        [FormerlySerializedAs("LocomotionNoWeapon")] public TwoDBlendTreePlayableDefinition LocomotionTreeUpperBody;
        [FormerlySerializedAs("LocomotionWithWeapon")] public TwoDBlendTreePlayableDefinition LocomotionTreeLowerBody;
    }
}