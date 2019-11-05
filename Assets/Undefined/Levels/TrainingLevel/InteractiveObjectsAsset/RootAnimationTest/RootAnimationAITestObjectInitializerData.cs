using System;
using AIObjects;
using UnityEngine;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public class RootAnimationAITestObjectInitializerData : AbstractAIInteractiveObjectInitializerData
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public RootAnimationCutsceneTemplate RootAnimationCutsceneTemplate;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new RootAnimationAITestObject(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
        }
    }
}