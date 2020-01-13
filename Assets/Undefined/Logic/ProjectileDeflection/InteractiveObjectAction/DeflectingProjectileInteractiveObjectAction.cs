using System.Collections.Generic;
using InteractiveObjectAction;
using InteractiveObjects;

namespace ProjectileDeflection
{
    public struct DeflectingProjectileInteractiveObjectActionInput : IInteractiveObjectActionInput
    {
        public CoreInteractiveObject AssociatedInteractiveObject { get; private set; }
        public ProjectileDeflectionSystem ProjectileDeflectionSystemRef { get; private set; }

        public DeflectingProjectileInteractiveObjectActionInput(CoreInteractiveObject associatedInteractiveObject, ProjectileDeflectionSystem projectileDeflectionSystemRef)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            ProjectileDeflectionSystemRef = projectileDeflectionSystemRef;
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
                IEM_DeflectingProjectileAction_WorkflowEventListener.OnDeflectingProjectileInteractiveObjectActionExecuted();
            }

            List<CoreInteractiveObject> SuccessfullyProjectileDeflectedPropertiesBuffered = null;

            foreach (var insideInteractiveObject in this.DeflectingProjectileInteractiveObjectActionInput.ProjectileDeflectionSystemRef.GetInsideDeflectableInteractiveObjects())
            {
                if (insideInteractiveObject.AskIfProjectileCanBeDeflected(this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject))
                {
                    if (SuccessfullyProjectileDeflectedPropertiesBuffered == null)
                    {
                        SuccessfullyProjectileDeflectedPropertiesBuffered = new List<CoreInteractiveObject>();
                    }

                    SuccessfullyProjectileDeflectedPropertiesBuffered.Add(insideInteractiveObject);
                }
            }

            /// SuccessfullyProjectileDeflectedProperties are buffered because
            if (SuccessfullyProjectileDeflectedPropertiesBuffered != null && SuccessfullyProjectileDeflectedPropertiesBuffered.Count > 0)
            {
                for (var i = SuccessfullyProjectileDeflectedPropertiesBuffered.Count - 1; i >= 0; i--)
                {
                    SuccessfullyProjectileDeflectedPropertiesBuffered[i].InteractiveObjectDeflected(this.DeflectingProjectileInteractiveObjectActionInput.AssociatedInteractiveObject);
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

    public interface IEM_DeflectingProjectileAction_WorkflowEventListener
    {
        void OnDeflectingProjectileInteractiveObjectActionExecuted();
    }
}