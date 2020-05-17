using System;
using System.Collections.Generic;
using CoreGame;
using PlayerObject;
using SequencedAction;
using UnityEngine;

namespace Tutorial
{
    public class MovementTutorialTextAction : AbstractTutorialTextAction
    {
        private MovementTutorialTextActionDefinition MovementTutorialTextActionDefinition;

        public override void AfterFinishedEventProcessed()
        {
        }

        public MovementTutorialTextAction(MovementTutorialTextActionDefinition MovementTutorialTextActionDefinition,
            Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
            this.MovementTutorialTextActionDefinition = MovementTutorialTextActionDefinition;
        }

        public override void FirstExecutionAction()
        {
            #region External Dependencies

            //   var discussionTextConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.DiscussionTextConfiguration;

            #endregion

            base.FirstExecutionAction();
            //TODO
            //this.DiscussionWindow.OnDiscussionWindowAwakeV2(discussionTextConfiguration.ConfigurationInherentData[this.MovementTutorialTextActionDefinition.DiscussionTextID],
            //   DiscussionPositionManager.Get().GetDiscussionPosition(this.MovementTutorialTextActionDefinition.DiscussionPositionMarkerID).transform.position, WindowPositionType.SCREEN);
        }

        protected override ITutorialTextActionManager GetTutorialTextManager()
        {
            return new MovementTutorialTextActionmanager(this.MovementTutorialTextActionDefinition);
        }
    }

    class MovementTutorialTextActionmanager : ITutorialTextActionManager
    {
        private MovementTutorialTextActionDefinition MovementTutorialTextActionDefinition;

        public MovementTutorialTextActionmanager(MovementTutorialTextActionDefinition MovementTutorialTextActionDefinition)
        {
            this.MovementTutorialTextActionDefinition = MovementTutorialTextActionDefinition;
        }

        private float playerCrossedDistance = 0f;
        private Nullable<Vector3> lastFramePlayerPosition;

        private PlayerInteractiveObject PlayerInteractiveObject;


        public void FirstExecutionAction()
        {
            this.playerCrossedDistance = 0f;
            this.lastFramePlayerPosition = null;
            this.PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
        }

        public bool Tick(float d)
        {
            if (this.playerCrossedDistance >= 0f)
            {
                var currentPlayerPosition = this.PlayerInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
                if (lastFramePlayerPosition.HasValue)
                {
                    this.playerCrossedDistance += Vector3.Distance(this.lastFramePlayerPosition.Value, currentPlayerPosition);
                }

                this.lastFramePlayerPosition = currentPlayerPosition;

                if (this.playerCrossedDistance >= this.MovementTutorialTextActionDefinition.PlayerCrossedDistance)
                {
                    this.playerCrossedDistance = -1;
                    return true;
                }
            }

            return false;
        }
    }
}