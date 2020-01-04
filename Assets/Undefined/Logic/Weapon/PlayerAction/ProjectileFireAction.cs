﻿using System;
using Firing;
using PlayerActions;
using UnityEngine;

namespace Weapon
{
    public struct ProjectileFireActionInputData : IPlayerActionInput
    {
        public Weapon WeaponReference;
        public PlayerActionInherentData ProjectileFireActionDefinition;
        public Vector3 NormalizedWorldDirection;

        public ProjectileFireActionInputData(Weapon weaponReference, PlayerActionInherentData projectileFireActionDefinition, Vector3 normalizedWorldDirection)
        {
            WeaponReference = weaponReference;
            ProjectileFireActionDefinition = projectileFireActionDefinition;
            NormalizedWorldDirection = normalizedWorldDirection;
        }
    }
    
    public class ProjectileFireAction : PlayerAction
    {
        public const string ProjectileFireActionUniqueID = "ProjectileFireAction";
        private ProjectileFireActionInputData ProjectileFireActionInputData;

        public ProjectileFireAction(ProjectileFireActionInputData ProjectileFireActionInputData,
            Action OnPlayerActionStartedCallback = null, Action OnPlayerActionEndCallback = null) : base(ProjectileFireActionInputData.ProjectileFireActionDefinition.CorePlayerActionDefinition, OnPlayerActionStartedCallback, OnPlayerActionEndCallback)
        {
            this.ProjectileFireActionInputData = ProjectileFireActionInputData;
        }

        public override string PlayerActionUniqueID
        {
            get { return ProjectileFireActionUniqueID; }
        }

        public override bool FinishedCondition()
        {
            return true;
        }

        public override void FirstExecution()
        {
            FiringProjectilePathCalculation.CalculateProjectilePath_ToDirection(this.ProjectileFireActionInputData.WeaponReference.WeaponHolder, this.ProjectileFireActionInputData.NormalizedWorldDirection, out Vector3 firedProjectilePosition, out Quaternion firedProjectileRotation);
            this.ProjectileFireActionInputData.WeaponReference.SpawnFiredProjectile(new TransformStruct() {WorldPosition = firedProjectilePosition, WorldRotationEuler = firedProjectileRotation.eulerAngles});
        }

        public override void Tick(float d)
        {
        }

        public override void AfterTicks(float d)
        {
        }

        public override void TickTimeFrozen(float d)
        {
        }

        public override void LateTick(float d)
        {
        }

        public override void GUITick()
        {
        }

        public override void GizmoTick()
        {
        }
    }
}