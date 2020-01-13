using System.Collections.Generic;
using InteractiveObjectAction;
using InteractiveObjects;

namespace ProjectileDeflection
{
    public struct DeflectingProjectileInteractiveObjectActionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject { get; private set; }
        public ProjectileDeflectionSystem ProjectileDeflectionSystemRef { get; private set; }
        public DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData { get; private set; }

        public DeflectingProjectileInteractiveObjectActionInput(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionSystem projectileDeflectionSystemRef,
            DeflectingProjectileInteractiveObjectActionInherentData DeflectingProjectileInteractiveObjectActionInherentData)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            ProjectileDeflectionSystemRef = projectileDeflectionSystemRef;
            this.DeflectingProjectileInteractiveObjectActionInherentData = DeflectingProjectileInteractiveObjectActionInherentData;
        }
    }

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

            foreach (var insideInteractiveObject in this.DeflectingProjectileInteractiveObjectActionInput.ProjectileDeflectionSystemRef.GetInsideDeflectableInteractiveObjects())
            {
                if (insideInteractiveObject.AskIfProjectileCanBeDeflected(this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject))
                {
                    insideInteractiveObject.InteractiveObjectDeflected(this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject);
                }
            }
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