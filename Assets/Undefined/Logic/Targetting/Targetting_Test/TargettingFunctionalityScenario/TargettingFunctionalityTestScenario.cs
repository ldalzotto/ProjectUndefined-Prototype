using System;
using DefaultNamespace;
using InteractiveObjects;
using PlayerObject;
using SequencedAction;
using Targetting;
using Tests.TestScenario;
using UnityEngine;
using Weapon;

namespace Targetting_Test
{
    [Serializable]
    public class TargettingFunctionalityTestScenario : ATestScenarioDefinition
    {
        public const string TestTargettedObjectName0 = "Test_Targetted_Object_0";
        public const string TestTargettedObjectName1 = "Test_Targetted_Object_1";
        public const string TestTargettedObjectName2 = "Test_Targetted_Object_2";

        public const string AimPhysicsTestPoint0 = "AimPhysicsTestPoint0_0";
        public const string AimPhysicsTestPoint1 = "AimPhysicsTestPoint0_1";

        public override ASequencedAction[] BuildScenarioActions()
        {
            CoreInteractiveObject targettedObject0 = null;
            CoreInteractiveObject targettedObject1 = null;
            CoreInteractiveObject targettedObject2 = null;

            IEM_ProjectileFireActionInput_Retriever playerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            foreach (var io in InteractiveObjectV2Manager.Get().InteractiveObjects)
            {
                if (io.InteractiveGameObject.GetAssociatedGameObjectName() == TestTargettedObjectName0)
                {
                    targettedObject0 = io;
                }

                if (io.InteractiveGameObject.GetAssociatedGameObjectName() == TestTargettedObjectName1)
                {
                    targettedObject1 = io;
                }

                if (io.InteractiveGameObject.GetAssociatedGameObjectName() == TestTargettedObjectName2)
                {
                    targettedObject2 = io;
                }
            }

            GameObject AimPhysicsTestPoint0Object = GameObject.Find(AimPhysicsTestPoint0);
            GameObject AimPhysicsTestPoint1Object = GameObject.Find(AimPhysicsTestPoint1);


            return new ASequencedAction[]
            {
                new EnsureTargetLockAction(playerInteractiveObject, targettedObject0)
                    .Then(new Target_FireInteractiveObject_AndWait_Action(targettedObject0, new Target_FireInteractiveObject_AndWait_ActionDefintion(() => targettedObject0.IsAskingToBeDestroyed))
                        .Then(new EnsureTargetLockAction(playerInteractiveObject, targettedObject1)
                            .Then(new Target_FireInteractiveObject_AndWait_Action(targettedObject1, new Target_FireInteractiveObject_AndWait_ActionDefintion(() => targettedObject1.IsAskingToBeDestroyed))
                                .Then(new EnsureTargetLockAction(playerInteractiveObject, targettedObject2)
                                    .Then(new Target_FireInteractiveObject_AndWait_Action(targettedObject2, new Target_FireInteractiveObject_AndWait_ActionDefintion(() => targettedObject2.IsAskingToBeDestroyed))
                                        .Then(new WaitForSecondsAction(0.5f)
                                            .Then(new TimeFreezePressedAction()
                                                .Then(new StartAimingAction()
                                                    .Then(new MoveTargetCursorSmoothScreenPosition(
                                                            new MoveTargetCursorSmoothScreenPositionData(Camera.main.ScreenToViewportPoint(TargetCursorManager.Get().GetTargetCursorScreenPosition()), Camera.main.WorldToViewportPoint(AimPhysicsTestPoint0Object.transform.position), 0.1f))
                                                        .Then(new StopAimingAction()
                                                            .Then(new TimeFreezePressedAction()
                                                                .Then(new StartAimingAction()
                                                                    .Then(new MoveTargetCursorSmoothScreenPosition(
                                                                        new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.5f, 0.5f), Camera.main.WorldToViewportPoint(AimPhysicsTestPoint0Object.transform.position), 0.1f)))
                                                                )
                                                            )
                                                        )
                                                    )
                                                ))
                                        )
                                    )
                                )
                            )
                        )
                    )
            };
        }
    }
}