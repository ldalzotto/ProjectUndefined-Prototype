using System;
using Firing;
using InteractiveObjects;
using PlayerActions;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    public class ProjectileFireActionDefinition : PlayerActionInherentData
    {
        public override string PlayerActionUniqueID
        {
            get { return ProjectileFireAction.ProjectileFireActionUniqueID; }
        }

        public override PlayerAction BuildPlayerAction(IPlayerActionInput IPlayerActionInput, Action OnPlayerActionStartedCallback = null, Action OnPlayerActionEndCallback = null)
        {
            if (IPlayerActionInput is ProjectileFireActionInputData ProjectileFireActionInputData)
            {
                return new ProjectileFireAction(ProjectileFireActionInputData, OnPlayerActionStartedCallback, OnPlayerActionEndCallback);
            }

            return default;
        }

        /// <param name="AssociatedInteractiveObject">Must implement <see cref="IEM_ProjectileFireActionInput_Retriever"/> and <see cref="IEM_WeaponHandlingSystem_Retriever"/>
        /// to work properly.</param>
        public override IPlayerActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
        {
            if (AssociatedInteractiveObject is IEM_ProjectileFireActionInput_Retriever IEM_ProjectileFireActionInput_Retriever)
            {
                if (IEM_ProjectileFireActionInput_Retriever.ProjectileFireActionEnabled())
                {
                    var ProjectileFireActionInputData = new ProjectileFireActionInputData();
                    ProjectileFireActionInputData.NormalizedWorldDirection = IEM_ProjectileFireActionInput_Retriever.GetCurrentTargetDirection().normalized;
                    ProjectileFireActionInputData.ProjectileFireActionDefinition = this;

                    if (AssociatedInteractiveObject is IEM_WeaponHandlingSystem_Retriever IEM_WeaponHandlingSystem_Retriever)
                    {
                        IEM_WeaponHandlingSystem_Retriever.WeaponHandlingSystem.PopulateProjectileFireActionInputData(ref ProjectileFireActionInputData);
                        return ProjectileFireActionInputData;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// This interface is used by <see cref="ProjectileFireActionDefinition"/> to retrieve the target driection <see cref="GetCurrentTargetDirection"/> to set to
    /// a <see cref="ProjectileFireActionInputData"/>.
    /// </summary>
    public interface IEM_ProjectileFireActionInput_Retriever
    {
        bool ProjectileFireActionEnabled();
        Vector3 GetCurrentTargetDirection();
    }
}