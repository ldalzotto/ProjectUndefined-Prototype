using System;
using System.Collections.Generic;
using CoreGame;
using PlayerActions_Interfaces;
using SequencedAction;

namespace Tutorial
{
    public class ActionWheelTutorialStepAction : AbstractTutorialTextAction
    {
        private ActionWheelTutorialStepActionDefinition ActionWheelTutorialStepActionDefinition;
        private PuzzlePlayerActionWheelOpenTutorialTextActionManager PuzzlePlayerActionWheelOpenTutorialTextActionManager;

        public ActionWheelTutorialStepAction(ActionWheelTutorialStepActionDefinition ActionWheelTutorialStepActionDefinition, Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
            this.ActionWheelTutorialStepActionDefinition = ActionWheelTutorialStepActionDefinition;
        }

        public override void FirstExecutionAction()
        {
            #region External Dependencies

            //    var discussionTextConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.DiscussionTextConfiguration;

            #endregion

            base.FirstExecutionAction();
            //TODO
            // this.DiscussionWindow.OnDiscussionWindowAwakeV2(discussionTextConfiguration.ConfigurationInherentData[this.ActionWheelTutorialStepActionDefinition.DiscussionTextID],
            //   DiscussionPositionManager.Get().GetDiscussionPosition(this.ActionWheelTutorialStepActionDefinition.DiscussionPositionMarkerID).transform.position, WindowPositionType.SCREEN);
        }

        protected override ITutorialTextActionManager GetTutorialTextManager()
        {
            this.PuzzlePlayerActionWheelOpenTutorialTextActionManager = new PuzzlePlayerActionWheelOpenTutorialTextActionManager();
            return this.PuzzlePlayerActionWheelOpenTutorialTextActionManager;
        }

        public override void AfterFinishedEventProcessed()
        {
            this.PuzzlePlayerActionWheelOpenTutorialTextActionManager.Destroy();
        }

        public override void Interupt()
        {
            base.Interupt();
            this.PuzzlePlayerActionWheelOpenTutorialTextActionManager.Destroy();
        }
    }

    internal class PuzzlePlayerActionWheelOpenTutorialTextActionManager : ITutorialTextActionManager
    {
        private bool isPlayerActionWheelAwaken;

        public void FirstExecutionAction()
        {
            PlayerActionsEventListenerManager.Get().RegisterOnPlayerActionSelectionWheelAwakeEventListener(this.OnPlayerActionWheelAwake);
        }

        public bool Tick(float d)
        {
            return isPlayerActionWheelAwaken;
        }

        public void OnPlayerActionWheelAwake()
        {
            isPlayerActionWheelAwaken = true;
        }

        public void Destroy()
        {
            PlayerActionsEventListenerManager.Get().UnRegisterOnPlayerActionSelectionWheelAwakeEventListener(this.OnPlayerActionWheelAwake);
        }
    }
}