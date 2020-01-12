using System;
using InteractiveObjects;
using OdinSerializer;
using Skill;
using UnityEngine;

namespace PlayerActions
{
    /// <summary>
    /// The definition object of a <see cref="PlayerAction"/>.
    /// The <see cref="PlayerActionInherentData"/> exposed methods <see cref="BuildPlayerAction"/> and <see cref="BuildInputFromInteractiveObject"/> are used to build the associated <see cref="PlayerAction"/>.
    /// The <see cref="PlayerActionInherentData"/> can have only one associated <see cref="PlayerAction"/> implementation.
    /// </summary>
    [Serializable]
    public abstract class PlayerActionInherentData : SerializedScriptableObject
    {
        public CorePlayerActionDefinition CorePlayerActionDefinition;

        /// <summary>
        /// The <see cref="PlayerActionUniqueID"/> is the reprensetation of the unique link between <see cref="PlayerActionInherentData"/> and one associated <see cref="PlayerAction"/>.
        /// This id will be used by the <see cref="PlayerActionPlayerSystem"/> to ensure that will maintain one state per <see cref="PlayerActionUniqueID"/>.
        /// </summary>
        public abstract string PlayerActionUniqueID { get; }

        public abstract PlayerAction BuildPlayerAction(IPlayerActionInput IPlayerActionInput, Action OnPlayerActionStartedCallback = null, Action OnPlayerActionEndCallback = null);

        public abstract IPlayerActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject);
    }

    [Serializable]
    public struct CorePlayerActionDefinition
    {
        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount;

        [HideInInspector] public bool CooldownEnabled;

        [Tooltip("Cooldown time indicates the minimum time interval between consequent execution of the same PlayerAction.")] //
        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(CooldownEnabled))]
        public CorePlayerActionCooldownDefinition CorePlayerActionCooldownDefinition;

        [Tooltip("Does the Player can move while this PlayerAction is executing ?")]
        public bool MovementAllowed;

        [HideInInspector] [SerializeField] private bool SkillActionEnabled;

        [SerializeField]
        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(SkillActionEnabled))] //
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
    public struct CorePlayerActionCooldownDefinition
    {
        public float CoolDownTime;
    }
}