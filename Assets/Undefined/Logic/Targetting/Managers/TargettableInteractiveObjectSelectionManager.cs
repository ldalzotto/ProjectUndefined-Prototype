using CoreGame;
using InteractiveObjects;

namespace Targetting
{
    public class TargettableInteractiveObjectSelectionManager : GameSingleton<TargettableInteractiveObjectSelectionManager>
    {
        public CoreInteractiveObject CurrentlyTargettedInteractiveObject { get; private set; }

        public void OnCursorOverObject(CoreInteractiveObject CoreInteractiveObject)
        {
            this.CurrentlyTargettedInteractiveObject = CoreInteractiveObject;
        }

        public void OnCursorNoMoveOverObject(CoreInteractiveObject coreInteractiveObject)
        {
            this.OnInteractiveObjectDestroyed(coreInteractiveObject);
        }

        #region Logical Conditions

        public bool IsCurrentlyTargetting()
        {
            return this.CurrentlyTargettedInteractiveObject != null;
        }

        #endregion

        /// <summary>
        /// /!\ Called only from <see cref="TargettableInteractiveObjectScreenIntersectionManager"/> when a Targettable listened InteractiveObject is destroyed.
        /// </summary>
        public void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject == this.CurrentlyTargettedInteractiveObject)
            {
                this.CurrentlyTargettedInteractiveObject = null;
            }
        }
    }
}