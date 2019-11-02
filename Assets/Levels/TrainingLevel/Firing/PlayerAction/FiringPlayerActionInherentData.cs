using System;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;

namespace Firing
{
    [Serializable]
    public class FiringPlayerActionInherentData : PlayerActionInherentData
    {
        public float RecoilTime;
        public GameObject FiringProjectilePrefab;
        public GameObject TargetCursorPrefab;
        public GameObject FiringHorizontalPlanePrefab;

        public override PlayerAction BuildPlayerAction(IPlayerInteractiveObject PlayerInteractiveObject)
        {
            return new FiringPlayerAction(this, PlayerInteractiveObject);
        }
    }
}