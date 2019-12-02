using System;
using System.Collections.Generic;
using OdinSerializer;
using SequencedAction;
using UnityEngine;

namespace Tests.TestScenario
{
    [System.Serializable]
    [SceneHandleDraw]
    public abstract class ATestScenarioDefinition : ASequencedActionGraph
    {
        public string Title;
        [HideInInspector] public bool DescriptionFoldableEnabled;
        
        [TextArea]
        public string Description;

        [Foldable(canBeDisabled: false, disablingBoolAttribute: nameof(DescriptionFoldableEnabled))]
        public TestScenarioDescription DescriptionV2;
        /// <summary>
        /// The root prefab of all GameObjects that will be used for the test scenario.
        /// This prefab will be instanciated when the <see cref="TestScenarioManager"/> will start the
        /// test <see cref="TestScenarioManager.StartTest"/>.
        /// </summary>
        public GameObject TestEntitiesPrefab;

        public abstract List<ASequencedAction> BuildScenarioActions();
    }

    [Serializable]
    public class TestScenarioDescription
    {
        [TextArea(minLines: 15, maxLines: 15)] public string Description;
    }
}