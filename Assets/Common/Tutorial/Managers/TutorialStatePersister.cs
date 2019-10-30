using System;
using System.Collections.Generic;
using CoreGame;
using Persistence;
using Tutorial;
using UnityEngine;

namespace Tutorial
{
    public class TutorialStatePersister : AbstractGamePersister<TutorialState>
    {
        public TutorialStatePersister() : base("TutorialState", ".tut", "TutorialState")
        {
        }

        private TutorialState LoadedTutorialState;

        public bool GetTutorialState(TutorialStepID TutorialStepID)
        {
            if (this.LoadedTutorialState == null)
            {
                this.LoadedTutorialState = this.Load();
            }

            if (this.LoadedTutorialState == null)
            {
                this.InitTutorialState();
            }

            if (this.LoadedTutorialState.TutorialStepState.ContainsKey(TutorialStepID))
            {
                return this.LoadedTutorialState.TutorialStepState[TutorialStepID];
            }

            return false;
        }

        public void SetTutorialState(TutorialStepID tutorialStepID, bool value)
        {
            this.LoadedTutorialState.TutorialStepState[tutorialStepID] = value;
            this.SaveAsync(this.LoadedTutorialState);
        }

        private void InitTutorialState()
        {
            this.LoadedTutorialState = new TutorialState();
            this.SaveAsync(this.LoadedTutorialState);
        }
    }
}

namespace Persistence
{
    [Serializable]
    public class TutorialState
    {
        [SerializeField] public Dictionary<TutorialStepID, bool> TutorialStepState;

        public TutorialState()
        {
            this.TutorialStepState = new Dictionary<TutorialStepID, bool>();
        }
    }
}