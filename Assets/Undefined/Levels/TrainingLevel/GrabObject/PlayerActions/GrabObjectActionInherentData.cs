using System;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "GrabObjectActionInherentData", menuName = "Test/GrabObjectActionInherentData", order = 1)]
    public class GrabObjectActionInherentData : PlayerActionInherentData
    {
        public PlayerActionInherentData AddedPlayerActionInherentData;

        public override PlayerAction BuildPlayerAction(IPlayerInteractiveObject PlayerInteractiveObject, Action OnPlayerActionStartedCallback = null,
            Action OnPlayerActionEndCallback = null)
        {
            return new GrabObjectAction(this.AddedPlayerActionInherentData.BuildPlayerAction(PlayerInteractiveObject, OnPlayerActionEndCallback), this.CorePlayerActionDefinition);
        }
    }
}