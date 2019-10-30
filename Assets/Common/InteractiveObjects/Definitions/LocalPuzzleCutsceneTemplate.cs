using System;
using System.Collections.Generic;
using SequencedAction;

namespace InteractiveObjects
{
    [Serializable]
    public abstract class LocalPuzzleCutsceneTemplate : ASequencedActionGraph
    {
        public abstract List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject);
    }
}