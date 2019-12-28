using System.Collections.Generic;
using CoreGame;
using SequencedAction;

namespace Tutorial
{
    public class TutorialManager : GameSingleton<TutorialManager>, TutorialStepManagerEventListener
    {
        private TutorialStepManager PlayingTutorialStepManager;

        #region Internal Managers

        private TutorialStatePersister TutorialStatePersister;

        #endregion

        #region State

        private List<TutorialStepID> TutorialStepFInishedThisFrame;

        #endregion

        public void Init()
        {
            #region Event Registering

            #endregion

            TutorialStatePersister = new TutorialStatePersister();
            TutorialStepFInishedThisFrame = new List<TutorialStepID>();
            this.PlayingTutorialStepManager = new TutorialStepManager(this);
        }

        public void Tick(float d)
        {
            this.PlayingTutorialStepManager.Tick(d);
        }

        #region Data Retrieval 

        public bool GetTutorialCurrentState(TutorialStepID TutorialStepID)
        {
            return TutorialStatePersister.GetTutorialState(TutorialStepID);
        }

        public bool IsTutorialStepPlaying()
        {
            return this.PlayingTutorialStepManager.IsPlaying();
        }

        #endregion

        #region External Events

        public void PlayTutorialStep(TutorialStepID tutorialStepID)
        {
            PlayingTutorialStepManager.Interrupt();

            if (!TutorialStatePersister.GetTutorialState(tutorialStepID))
            {
                PlayingTutorialStepManager.Play(TutorialConfigurationGameObject.Get().TutorialStepConfiguration.ConfigurationInherentData[tutorialStepID].TutorialStepActionDefinition.BuildTutorialAction(), tutorialStepID);
            }
        }

        public void Abort()
        {
            PlayingTutorialStepManager.Interrupt();
        }

        public void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID)
        {
            TutorialStatePersister.SetTutorialState(tutorialStepID, true);
        }

        #endregion
    }

    public interface TutorialStepManagerEventListener
    {
        void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID);
    }

    internal class TutorialStepManager
    {
        private SequencedActionManager tutorialPlayer;
        private TutorialStepManagerEventListener TutorialStepManagerEventListener;
        private TutorialStepID TutorialStepID;

        public TutorialStepManager(TutorialStepManagerEventListener TutorialStepManagerEventListener)
        {
            this.TutorialStepManagerEventListener = TutorialStepManagerEventListener;
        }

        public void Play(AbstractTutorialTextAction AbstractTutorialTextAction, TutorialStepID tutorialStepID)
        {
            this.TutorialStepID = tutorialStepID;
            if (this.tutorialPlayer != null && this.tutorialPlayer.IsPlaying())
            {
                this.Interrupt();
            }

            this.tutorialPlayer = new SequencedActionManager(this.OnTutorialStepFinished);
            tutorialPlayer.OnAddActions(new ASequencedAction[] {AbstractTutorialTextAction});
        }

        public void Tick(float d)
        {
            if (this.tutorialPlayer != null)
            {
                tutorialPlayer.Tick(d);
            }
        }

        private void OnTutorialStepFinished()
        {
            this.TutorialStepManagerEventListener.OnTutorialStepManagerEnd(this.TutorialStepID);
            this.tutorialPlayer = null;
        }

        public void Interrupt()
        {
            if (tutorialPlayer != null)
            {
                tutorialPlayer.InterruptAllActions();
            }
        }

        public bool IsPlaying()
        {
            return this.tutorialPlayer == null ? false : this.tutorialPlayer.IsPlaying();
        }
    }
}