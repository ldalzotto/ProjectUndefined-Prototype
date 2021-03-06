﻿using System;
using InteractiveObjects;
using OdinSerializer;
using Skill;
using UnityEngine;
using UnityEngine.Serialization;

namespace InteractiveObjectAction
{
    /// <summary>
    /// The definition object of a <see cref="AInteractiveObjectAction"/>.
    /// The <see cref="InteractiveObjectActionInherentData"/> exposed methods <see cref="BuildInteractiveObjectAction"/> and <see cref="BuildInputFromInteractiveObject"/> are used to build the associated <see cref="AInteractiveObjectAction"/>.
    /// </summary>
    [Serializable]
    public abstract class InteractiveObjectActionInherentData : SerializedScriptableObject
    {
        [FormerlySerializedAs("CorePlayerActionDefinition")]
        public CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition;

        /// <summary>
        /// The <see cref="InteractiveObjectActionUniqueID"/> is the reprensetation of the unique link between <see cref="InteractiveObjectActionInherentData"/> and one associated <see cref="AInteractiveObjectAction"/>.
        /// This id will be used by the <see cref="PlayerActionPlayerSystem"/> to ensure that will maintain one state per <see cref="InteractiveObjectActionUniqueID"/>.
        /// </summary>
        public abstract string InteractiveObjectActionUniqueID { get; }

        public bool AreTheSameID(string comparisonID)
        {
            return this.InteractiveObjectActionUniqueID == comparisonID;
        }

        public abstract AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput);

        /// <summary>
        /// Try to build a <see cref="IInteractiveObjectActionInput"/> from the associated <see cref="CoreInteractiveObject"/>.
        /// If this method returns null, then no <see cref="AInteractiveObjectAction"/> will be executed. (see <see cref="InteractiveObjectActionPlayerSystem.ExecuteActionV2"/>).
        /// </summary>
        public abstract IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject);
    }

    [Serializable]
    public struct CoreInteractiveObjectActionDefinition
    {
        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount;

        [HideInInspector] public bool CooldownEnabled;

        [FormerlySerializedAs("CorePlayerActionCooldownDefinition")]
        [Tooltip("Cooldown time indicates the minimum time interval between consequent execution of the same AInteractiveObjectAction.")] //
        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(CooldownEnabled))]
        public InteractiveObjectActionCooldownDefinition interactiveObjectActionCooldownDefinition;

        [Tooltip("Does the Player can move while this AInteractiveObjectAction is executing ?")]
        public bool MovementAllowed;

        [HideInInspector] [SerializeField] private bool SkillActionEnabled;

        [SerializeField] [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(SkillActionEnabled))] //
        private SkillActionDefinition SkillActionDefinition;

        public SkillActionDefinition GetSkillActionDefinition()
        {
            if (this.SkillActionEnabled)
            {
                return this.SkillActionDefinition;
            }

            return null;
        }
    }

    [Serializable]
    public struct InteractiveObjectActionCooldownDefinition
    {
        public float CoolDownTime;
    }
}