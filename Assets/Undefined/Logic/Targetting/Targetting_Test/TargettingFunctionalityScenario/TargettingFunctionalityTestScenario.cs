using System;
using System.Collections;
using System.Collections.Generic;
using Firing;
using InteractiveObjects;
using PlayerActions;
using SequencedAction;
using Targetting;
using Tests;
using Tests.TestScenario;
using UnityEngine;

namespace Targetting_Test
{
    [Serializable]
    public class TargettingFunctionalityTestScenario : ATestScenarioDefinition
    {
        public const string TestTargettedObjectName = "Test_Targetted_Object";

        public override ASequencedAction[] BuildScenarioActions()
        {
            CoreInteractiveObject targettedObject = null;
            foreach (var io in InteractiveObjects.InteractiveObjectV2Manager.Get().InteractiveObjects)
            {
                if (io.InteractiveGameObject.GetAssociatedGameObjectName() == TestTargettedObjectName)
                {
                    targettedObject = io;
                }
            }

            return new ASequencedAction[]
            {
                new Target_FireInteractiveObject_AndWait_Action(targettedObject, new Target_FireInteractiveObject_AndWait_ActionDefintion(() => false))
            };
        }
    }
}