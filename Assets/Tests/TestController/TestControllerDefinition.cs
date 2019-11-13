using System;
using OdinSerializer;
using Tests.TestScenario;

namespace Tests
{
    /// <summary>
    /// The ScriptableObject that the user will use to select test to play and change global test variables.
    /// /!\ This ScriptableObject is used as a GUI for test scene. It's values mays be subjected to change at runtime.
    /// </summary>
    [Serializable]
    public class TestControllerDefinition : SerializedScriptableObject
    {
        [Inline()] public TestGameControlDefinition TestGameControlDefinition;
        [Inline()] public ATestScenarioDefinition aTestScenarioDefinition;
        public bool StartTest;
    }
}