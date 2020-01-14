using System.Collections.Generic;
using Input;
using InteractiveObjects;
using Skill;

namespace ProjectileDeflection
{
    /// <summary>
    /// Responsible of spawning <see cref="ProjectileDeflectionFeedbackIcon"/> for every deflectable InteractiveObject in the range of <see cref="ObjectsInsideDeflectionRangeSystem"/>.
    /// Icons are UI textures that follow the FiredProjectile deflected.
    /// </summary>
    public struct ProjectileDeflectionFeedbackIconSystem
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private Dictionary<CoreInteractiveObject, ProjectileDeflectionFeedbackIcon> ProjectileDeflectionFeedbackIcons;

        public ProjectileDeflectionFeedbackIconSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.ProjectileDeflectionFeedbackIcons = new Dictionary<CoreInteractiveObject, ProjectileDeflectionFeedbackIcon>();
        }

        public void Tick(float d)
        {
            if (this.ProjectileDeflectionFeedbackIcons != null)
            {
                foreach (var ProjectileDeflectionFeedbackIconEntry in this.ProjectileDeflectionFeedbackIcons)
                {
                    ProjectileDeflectionFeedbackIconEntry.Value.SetPositionFromWorld(ProjectileDeflectionFeedbackIconEntry.Key.InteractiveGameObject.InteractiveGameObjectParent.transform.position);
                }
            }
        }

        public void TickTimeFrozen(float d)
        {
            this.Tick(d);
        }

        #region External Events

        /// <summary>
        /// This event is called from <see cref="ObjectsInsideDeflectionRangeSystem"/>. Thus, interactive object has already bean filtered by it's. <see cref="InteractiveObjectTag"/>.
        /// </summary>
        public void OnInteractiveObjectJustInsideDeflectionRange(CoreInteractiveObject InsideInteractiveObject)
        {
            if (InsideInteractiveObject.AskIfProjectileCanBeDeflected(this.AssociatedInteractiveObject))
            {
                if (this.AssociatedInteractiveObject is IEM_SkillSystem_ExposedMethods IEM_SkillSystem_Interface)
                {
                    ProjectileDeflectionFeedbackIcons.Add(InsideInteractiveObject, ProjectileDeflectionFeedbackIcon.Build(
                        ProjectileDeflectionConfigurationGameObject.Get().ProjectileDeflectionGlobalConfiguration.DeflectionIconPrefab,
                        IEM_SkillSystem_Interface.GetInputIdAssociatedToTheInteractiveObjectAction(DeflectingProjectileInteractiveObjectAction.DeflectingProjectileInteractiveObjectActionUniqueID)
                    ));
                }
            }
        }

        public void OnInteractiveObjectJustOutsideDeflectionRange(CoreInteractiveObject OutsideInteractiveObject)
        {
            if (ProjectileDeflectionFeedbackIcons.ContainsKey(OutsideInteractiveObject))
            {
                this.ProjectileDeflectionFeedbackIcons[OutsideInteractiveObject].Destroy();
                this.ProjectileDeflectionFeedbackIcons.Remove(OutsideInteractiveObject);
            }
        }

        #endregion
    }
}