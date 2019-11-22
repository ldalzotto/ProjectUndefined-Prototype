using System;
using System.Collections.Generic;
using SequencedAction;
using Tests.TestScenario;

namespace Obstalce_Tests
{
    [Serializable]
    public class ObstacleFrustumOcclusionTestScenario : ATestScenarioDefinition
    {
        public override List<ASequencedAction> BuildScenarioActions()
        {
            //Handles by animator
            return new List<ASequencedAction>();
        }
    }
}