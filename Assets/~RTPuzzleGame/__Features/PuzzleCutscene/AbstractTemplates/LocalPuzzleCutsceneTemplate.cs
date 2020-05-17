using System;
using System.Collections.Generic;
using InteractiveObjects;
using SequencedAction;

namespace RTPuzzle
{
    [Serializable]
    public abstract class LocalPuzzleCutsceneTemplate : ASequencedActionGraph
    {
        public abstract List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject);
    }
}