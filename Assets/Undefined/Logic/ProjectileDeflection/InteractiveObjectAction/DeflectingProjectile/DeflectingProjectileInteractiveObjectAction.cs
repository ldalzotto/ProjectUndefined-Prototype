using System.Collections.Generic;
using InteractiveObjectAction;
using InteractiveObjects;
using PlayerObject_Interfaces;
using UnityEngine;

namespace ProjectileDeflection
{
    public struct DeflectingProjectileInteractiveObjectActionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject { get; private set; }
        public ProjectileDeflectionTrackingInteractiveObjectAction ProjectileDeflectionTrackingInteractiveObjectActionRef { get; private set; }
        public DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData { get; private set; }

        public DeflectingProjectileInteractiveObjectActionInput(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionTrackingInteractiveObjectAction projectileDeflectionTrackingInteractiveObjectActionRef,
            DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            ProjectileDeflectionTrackingInteractiveObjectActionRef = projectileDeflectionTrackingInteractiveObjectActionRef;
            this.DeflectingProjectileInteractiveObjectActionInherentData = DeflectingProjectileInteractiveObjectActionInherentData;
        }
    }

    /// <summary>
    /// Responsible of deflecting projectiles when conditions are met.
    /// It is up to the <see cref="DeflectingProjectileInteractiveObjectAction"/> to notify other <see cref="CoreInteractiveObject"/> that they have been deflected (by calling <see cref="CoreInteractiveObject.InteractiveObjectDeflected"/>
    /// to deflected projectiles.)
    /// Deflection trajectory calculations are handled by <see cref="DeflectionCalculations"/>.
    /// </summary>
    public class DeflectingProjectileInteractiveObjectAction : AInteractiveObjectAction
    {
        public const string DeflectingProjectileInteractiveObjectActionUniqueID = "DeflectingProjectileInteractiveObjectAction";

        private DeflectingProjectileInteractiveObjectActionInput DeflectingProjectileInteractiveObjectActionInput;

        public DeflectingProjectileInteractiveObjectAction(DeflectingProjectileInteractiveObjectActionInput DeflectingProjectileInteractiveObjectActionInput,
            CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition) : base(coreInteractiveObjectActionDefinition)
        {
            this.DeflectingProjectileInteractiveObjectActionInput = DeflectingProjectileInteractiveObjectActionInput;
        }

        public override string InteractiveObjectActionUniqueID
        {
            get { return DeflectingProjectileInteractiveObjectActionUniqueID; }
        }

        public override bool FinishedCondition()
        {
            return base.FinishedCondition() || true;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();

            if (this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject is IEM_DeflectingProjectileAction_WorkflowEventListener IEM_DeflectingProjectileAction_WorkflowEventListener)
            {
                IEM_DeflectingProjectileAction_WorkflowEventListener.OnDeflectingProjectileInteractiveObjectActionExecuted(
                    this.DeflectingProjectileInteractiveObjectActionInput.DeflectingProjectileInteractiveObjectActionInherentData);
            }

            foreach (var insideInteractiveObject in this.DeflectingProjectileInteractiveObjectActionInput.ProjectileDeflectionTrackingInteractiveObjectActionRef.GetInsideDeflectableInteractiveObjects())
            {
                if (insideInteractiveObject.AskIfProjectileCanBeDeflected(this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject))
                {
                    insideInteractiveObject.InteractiveObjectDeflected(this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject);
                    if (this.DeflectingProjectileInteractiveObjectActionInput.DeflectingProjectileInteractiveObjectActionInherentData.OnDeflectionParticles != null)
                    {
                        this.DeflectingProjectileInteractiveObjectActionInput.DeflectingProjectileInteractiveObjectActionInherentData.OnDeflectionParticles
                            .BuildParticleObject("ProjectileDeflectionParticles", null, insideInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition,
                                Quaternion.Euler(insideInteractiveObject.InteractiveGameObject.GetTransform().WorldRotationEuler));
                    }

                    if (this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject is IPlayerInteractiveObject IPlayerInteractiveObject)
                    {
                        IPlayerInteractiveObject.SetConstraintForThisFrame(new LookDirectionConstraint(Quaternion.Euler(insideInteractiveObject.InteractiveGameObject.GetTransform().WorldRotationEuler)));
                    }
                }
            }
        }
    }

    /// <summary>
    /// This interface is used by <see cref="DeflectingProjectileInteractiveObjectAction"/> and called when the <see cref="DeflectingProjectileInteractiveObjectAction"/> has just been
    /// executed.
    /// Any <see cref="CoreInteractiveObject"/> can implement this interface to be notified when the <see cref="DeflectingProjectileInteractiveObjectAction"/> has been played.
    /// </summary>
    public interface IEM_DeflectingProjectileAction_WorkflowEventListener
    {
        void OnDeflectingProjectileInteractiveObjectActionExecuted(DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData);
    }
}