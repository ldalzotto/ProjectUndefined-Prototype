using System;
using ConfigurationEditor;
using UnityEngine;

namespace Tutorial
{
    [Serializable]
    [CreateAssetMenu(fileName = "TutorialStepConfiguration", menuName = "Configuration/CoreGame/TutorialStepConfiguration/TutorialStepConfiguration", order = 1)]
    public class TutorialStepConfiguration : ConfigurationSerialization<TutorialStepID, TutorialStepConfigurationInherentData>
    {
    }
}