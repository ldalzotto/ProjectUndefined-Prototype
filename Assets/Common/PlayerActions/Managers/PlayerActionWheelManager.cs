using System.Collections.Generic;
using CoreGame;
using PlayerActions_Interfaces;
using SelectionWheel;
using UnityEngine;

namespace PlayerActions
{
    internal class PlayerActionWheelManager : GameSingleton<PlayerActionWheelManager>
    {
        private SelectionWheelObject PlayerActionSelectionWheel;

        public void Init()
        {
            this.PlayerActionSelectionWheel = new SelectionWheelObject();
        }

        public void Tick(float d)
        {
            this.PlayerActionSelectionWheel.Tick(d);
        }

        public void LateTick(float d)
        {
            this.PlayerActionSelectionWheel.LateTick(d);
        }

        #region External Events

        internal void PlayerActionWheelAwake(List<PlayerAction> availablePlayerActions, Transform followingWorldTransform)
        {
            this.PlayerActionSelectionWheel.AwakeWheel(availablePlayerActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData), followingWorldTransform);
            PlayerActionsEventListenerManager.Get().OnPlayerActionSelectionWheelAwake();
        }

        internal void PlayerActionWheelSleep(bool detroyImmediate)
        {
            this.PlayerActionSelectionWheel.SleepWheel(detroyImmediate);
        }

        internal void PlayerActionWheelRefresh(List<PlayerAction> availablePlayerActions)
        {
            if (this.PlayerActionSelectionWheel.IsWheelEnabled)
            {
                this.PlayerActionSelectionWheel.RefreshWheel(availablePlayerActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData));
            }
        }

        #endregion

        #region Data Retrieval

        public PlayerAction GetCurrentlySelectedPlayerAction()
        {
            if (this.PlayerActionSelectionWheel.IsWheelEnabled)
            {
                return ((PlayerSelectionWheelNodeData) this.PlayerActionSelectionWheel.GetSelectedNodeData()).Data as PlayerAction;
            }

            return null;
        }

        #endregion

        #region Logical Conditions

        public bool IsSelectionWheelEnabled()
        {
            return this.PlayerActionSelectionWheel.IsWheelEnabled;
        }

        #endregion
    }
}