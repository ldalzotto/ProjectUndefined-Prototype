using System;
using OdinSerializer;
using UnityEngine;

namespace Tutorial
{
    [Serializable]
    [CreateAssetMenu(fileName = "TutorialStepConfigurationInherentData", menuName = "Configuration/CoreGame/TutorialStepConfiguration/TutorialStepConfigurationInherentData", order = 1)]
    public class TutorialStepConfigurationInherentData : SerializedScriptableObject
    {
        [Inline(createAtSameLevelIfAbsent: true)]
        public TutorialStepActionDefinition TutorialStepActionDefinition;
    }

    [Serializable]
    public abstract class TutorialStepActionDefinition : SerializedScriptableObject
    {
        public abstract AbstractTutorialTextAction BuildTutorialAction();
    }
}