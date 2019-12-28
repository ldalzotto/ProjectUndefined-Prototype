using System;
using System.Collections.Generic;
using SequencedAction;
using Tests.TestScenario;

namespace Obstalce_Tests
{
    [Serializable]
    public class ObstacleFrustumOcclusionTestScenario : ATestScenarioDefinition
    {
        public override ASequencedAction[] BuildScenarioActions()
        {
            //Handled by animator
            return new ASequencedAction[]{};
        }
    }
}