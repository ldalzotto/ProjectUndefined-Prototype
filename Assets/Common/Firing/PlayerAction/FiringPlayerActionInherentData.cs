using System;
using PlayerActions;
using PlayerObject_Interfaces;
using UnityEngine;

namespace Firing
{
    [Serializable]
    [SceneHandleDraw]
    public class FiringPlayerActionInherentData : PlayerActionInherentData
    {
        public float RecoilTime;
        public float TargetCursorInitialOffset;

        [WireCircleWorldAttribute(PositionFieldName = nameof(FiringPlayerActionInherentData.ProjectileSpawnLocalPosition))]
        public Vector3 ProjectileSpawnLocalPosition;

        public FiredProjectileInitializer FiringProjectileInitializerPrefab;
        public GameObject TargetCursorPrefab;
        public GameObject FiringHorizontalPlanePrefab;

        public override PlayerAction BuildPlayerAction(IPlayerInteractiveObject PlayerInteractiveObject)
        {
            return new FiringPlayerAction(this, PlayerInteractiveObject);
        }
    }
}