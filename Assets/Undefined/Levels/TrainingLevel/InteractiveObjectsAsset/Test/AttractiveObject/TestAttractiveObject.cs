using InteractiveObjects_Interfaces;
using PlayerActions;
using PlayerObject;
using SelectableObject;
using UnityEngine;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class TestAttractiveObject : CoreInteractiveObject
    {
        private TestAttractiveObjectInitializerData TestAttractiveObjectInitializerData;

        [VE_Ignore] private PlayerAction AssociatedPlayerAction;

        [VE_Nested] private AttractiveObjectSystem AttractiveObjectSystem;
        [VE_Nested] [DrawNested] private DisarmObjectSystem DisarmObjectSystem;

        [VE_Nested] private SelectableObjectSystem SelectableObjectSystem;

        public TestAttractiveObject(IInteractiveGameObject interactiveGameObject, TestAttractiveObjectInitializerData InteractiveObjectInitializerData)
        {
            this.TestAttractiveObjectInitializerData = InteractiveObjectInitializerData;
            interactiveGameObject.CreateLogicCollider(this.TestAttractiveObjectInitializerData.InteractiveObjectLogicCollider);
            base.BaseInit(interactiveGameObject, true);
        }

        public override void Init()
        {
            interactiveObjectTag = new InteractiveObjectTag {IsAttractiveObject = true};

            AttractiveObjectSystem = new AttractiveObjectSystem(this, (InteractiveObjectTag) => InteractiveObjectTag.IsAi, this.TestAttractiveObjectInitializerData.AttractiveObjectSystemDefinition,
                OnAssociatedAttractiveSystemJustIntersected, OnAssociatedAttractiveSystemNoMoreIntersected, OnAssociatedAttractiveSystemInterestedNothing);

            DisarmObjectSystem = new DisarmObjectSystem(this, this.TestAttractiveObjectInitializerData.DisarmSystemDefinition, (InteractiveObjectPhysicsTriggerInfo) => InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsAi, OnAssociatedDisarmObjectTriggerEnter, OnAssciatedDisarmObjectTriggerExit);
            SelectableObjectSystem = new SelectableObjectSystem(this, this.TestAttractiveObjectInitializerData.SelectableObjectSystemDefinition,
                this.TestAttractiveObjectInitializerData.SelectableGrabActionDefinition.BuildPlayerAction(PlayerInteractiveObjectManager.Get().PlayerInteractiveObject));
        }

        public override void Tick(float d)
        {
            if (DisarmObjectSystem != null) DisarmObjectSystem.Tick(d);
            AttractiveObjectSystem.Tick(d);
            DisarmObjectSystem.Tick(d);
            isAskingToBeDestroyed = AttractiveObjectSystem != null && AttractiveObjectSystem.IsAskingTobedestroyed || DisarmObjectSystem != null && DisarmObjectSystem.IsTimeElasped();
        }

        public override void Destroy()
        {
            AttractiveObjectSystem.OnDestroy();
            DisarmObjectSystem.OnDestroy();
            SelectableObjectSystem.OnDestroy();
            base.Destroy();
        }

        #region Attractive Object Events

        private void OnAssociatedAttractiveSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemJustIntersected");
            IntersectedInteractiveObject.OnOtherAttractiveObjectJustIntersected(this);
        }

        private void OnAssociatedAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            //  Debug.Log("OnAssociatedAttractiveSystemInterestedNothing");
            IntersectedInteractiveObject.OnOtherAttractiveObjectIntersectedNothing(this);
        }

        private void OnAssociatedAttractiveSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("OnAssociatedAttractiveSystemNoMoreIntersected");
            IntersectedInteractiveObject.OnOtherAttractiveObjectNoMoreIntersected(this);
        }

        #endregion

        #region Disarm Object Events

        private void OnAssociatedDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmObjectTriggerEnter(this);
            DisarmObjectSystem.AddInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        private void OnAssciatedDisarmObjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            OtherInteractiveObject.OnOtherDisarmobjectTriggerExit(this);
            DisarmObjectSystem.RemoveInteractiveObjectDisarmingThisObject(OtherInteractiveObject);
        }

        #endregion
    }
}