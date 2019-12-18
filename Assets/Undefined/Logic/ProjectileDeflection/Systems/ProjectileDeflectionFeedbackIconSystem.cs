using System.Collections.Generic;
using InteractiveObjects;

namespace ProjectileDeflection
{
    public struct ProjectileDeflectionFeedbackIconSystem
    {
        private Dictionary<CoreInteractiveObject, ProjectileDeflectionFeedbackIcon> ProjectileDeflectionFeedbackIcons;

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

        public void OnInteractiveObjectJustInsideDeflectionRange(CoreInteractiveObject InsideInteractiveObject)
        {
            if (ProjectileDeflectionFeedbackIcons == null)
            {
                ProjectileDeflectionFeedbackIcons = new Dictionary<CoreInteractiveObject, ProjectileDeflectionFeedbackIcon>();
            }

            ProjectileDeflectionFeedbackIcons.Add(InsideInteractiveObject, ProjectileDeflectionFeedbackIcon.Build(
                ProjectileDeflectionConfigurationGameObject.Get().ProjectileDeflectionGlobalConfiguration.DeflectionIconPrefab
            ));
        }

        public void OnInteractiveObjectJustOutsideDeflectionRange(CoreInteractiveObject OutsideInteractiveObject)
        {
            if (ProjectileDeflectionFeedbackIcons != null && ProjectileDeflectionFeedbackIcons.ContainsKey(OutsideInteractiveObject))
            {
                this.ProjectileDeflectionFeedbackIcons[OutsideInteractiveObject].Destroy();
                this.ProjectileDeflectionFeedbackIcons.Remove(OutsideInteractiveObject);
            }
        }

        #endregion
    }
}