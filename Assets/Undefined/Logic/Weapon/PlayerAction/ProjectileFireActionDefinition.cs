using System;
using InteractiveObjectAction;
using InteractiveObjects;
using UnityEngine;

namespace Weapon
{
    [Serializable]
    public class ProjectileFireActionDefinition : InteractiveObjectActionInherentData
    {
        public override string InteractiveObjectActionUniqueID
        {
            get { return ProjectileFireAction.ProjectileFireActionUniqueID; }
        }

        public override InteractiveObjectAction.AInteractiveObjectAction BuildInteractiveObjectAction(IInteractiveObjectActionInput interactiveObjectActionInput, Action OnInteractiveObjectActionStartedCallback = null, Action OnInteractiveObjectActionEndCallback = null)
        {
            if (interactiveObjectActionInput is ProjectileFireActionInputData ProjectileFireActionInputData)
            {
                return new ProjectileFireAction(ProjectileFireActionInputData, OnInteractiveObjectActionStartedCallback, OnInteractiveObjectActionEndCallback);
            }

            return default;
        }

        /// <param name="AssociatedInteractiveObject">Must implement <see cref="IEM_ProjectileFireActionInput_Retriever"/> and <see cref="IEM_WeaponHandlingSystem_Retriever"/>
        /// to work properly.</param>
        public override IInteractiveObjectActionInput BuildInputFromInteractiveObject(CoreInteractiveObject AssociatedInteractiveObject)
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