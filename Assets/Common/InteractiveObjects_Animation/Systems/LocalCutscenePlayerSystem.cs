using System;
using System.Collections.Generic;
using InteractiveObjects_Interfaces;
using SequencedAction;

namespace InteractiveObject_Animation
{
    public class LocalCutscenePlayerSystem : AInteractiveObjectSystem
    {
        private SequencedActionPlayer CurrentPlayingCutscene;

        public override void Tick(float d)
        {
            base.Tick(d);
            if (CurrentPlayingCutscene != null) CurrentPlayingCutscene.Tick(d);
        }

        public void PlayCutscene(List<ASequencedAction> SequencedActions, Action OnCutsceneEnded = null, Action OnCutsceneKilled = null)
        {
            //A local cutscene is already playing
            if (CurrentPlayingCutscene != null) CurrentPlayingCutscene.Kill();

            CurrentPlayingCutscene = new SequencedActionPlayer(SequencedActions,
                () =>
                {
                    OnCutsceneEndedOrKilled();
                    if (OnCutsceneEnded != null) OnCutsceneEnded.Invoke();
                },
                () =>
                {
                    OnCutsceneEndedOrKilled();
                    if (OnCutsceneKilled != null) OnCutsceneKilled.Invoke();
                });
            CurrentPlayingCutscene.Play();
        }

        public void KillCurrentCutscene()
        {
            if (this.CurrentPlayingCutscene != null)
            {
                CurrentPlayingCutscene.Kill();
            }
        }

        private void OnCutsceneEndedOrKilled()
        {
            CurrentPlayingCutscene = null;
        }

        public void OnAIDestinationReached()
        {
            if (this.CurrentPlayingCutscene != null)
            {
                foreach (var currentAction in this.CurrentPlayingCutscene.GetCurrentActions(true))
                {
                    var destinationReachedListeningNode = currentAction as IActionAbortedOnDestinationReached;
                    if (destinationReachedListeningNode != null) destinationReachedListeningNode.OnDestinationReached();
                }
            }
        }
    }
}