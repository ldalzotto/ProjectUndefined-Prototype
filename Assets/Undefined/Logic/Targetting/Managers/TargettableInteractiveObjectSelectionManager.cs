using System.Collections.Generic;
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
        
        public void OnInteractiveObjectDestroyed(CoreInteractiveObject CoreInteractiveObject)
        {
            if (CoreInteractiveObject == this.CurrentlyTargettedInteractiveObject)
            {
                this.CurrentlyTargettedInteractiveObject = null;
            }
        }
    }
}