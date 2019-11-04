using System;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;

[Serializable]
public class DummyDamageTakerDefinition : AbstractInteractiveObjectV2Definition
{
    public InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition;

    public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
    {
        return new DummyDamageTakerInteractiveObject(InteractiveGameObjectFactory.Build(interactiveGameObject), this);
    }
}

class DummyDamageTakerInteractiveObject : CoreInteractiveObject
{
    public DummyDamageTakerInteractiveObject(IInteractiveGameObject IInteractiveGameObject, DummyDamageTakerDefinition DummyDamageTakerDefinition)
    {
        this.interactiveObjectTag = new InteractiveObjectTag() {IsTakingDamage = true};
        IInteractiveGameObject.CreateLogicCollider(DummyDamageTakerDefinition.InteractiveObjectBoxLogicColliderDefinition);
        base.BaseInit(IInteractiveGameObject, true);
    }

    public override void Init()
    {
    }
}