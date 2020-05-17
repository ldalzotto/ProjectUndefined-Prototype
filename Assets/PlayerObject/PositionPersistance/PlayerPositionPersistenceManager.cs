using System;
using CoreGame;
using Persistence;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerPositionPersistenceManager : GameSingleton<PlayerPositionPersistenceManager>
    {
        private PlayerPositionPersister playerPositionPersister;

        public PlayerPosition PlayerPositionBeforeLevelLoad { get; private set; }

        public void Init(PlayerInteractiveObject PlayerInteractiveObject)
        {
            this.playerPositionPersister = new PlayerPositionPersister();

            if (!this.PlayerPositionBeforeLevelLoad.HasBeenInit)
            {
                var loadedPlayerPositionBeforeLevelLoad = this.playerPositionPersister.Load();
                if (!loadedPlayerPositionBeforeLevelLoad.HasBeenInit)
                {
                    this.OnAdventureToPuzzleLevel(PlayerInteractiveObject);
                }
                else
                {
                    this.PlayerPositionBeforeLevelLoad = loadedPlayerPositionBeforeLevelLoad;
                }
            }
        }

        #region External Events

        public void OnAdventureToPuzzleLevel(PlayerInteractiveObject PlayerInteractiveObject)
        {
            this.PlayerPositionBeforeLevelLoad = new PlayerPosition(PlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.position,
                PlayerInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation);
            this.playerPositionPersister.SaveAsync(this.PlayerPositionBeforeLevelLoad);
        }

        #endregion
    }


    class PlayerPositionPersister : AbstractGamePersister<PlayerPosition>
    {
        public PlayerPositionPersister() : base("PlayerPosition", ".pl", "PlayerPosition")
        {
        }
    }
}


namespace Persistence
{
    [Serializable]
    public struct PlayerPosition
    {
        [SerializeField] public bool HasBeenInit;
        [SerializeField] public Vector3Binarry Position;
        [SerializeField] public QuaternionBinarry Quaternion;

        public PlayerPosition(Vector3 position, Quaternion quaternion)
        {
            this.HasBeenInit = true;
            Position = new Vector3Binarry(position);
            Quaternion = new QuaternionBinarry(quaternion);
        }

        public Vector3 GetPosition()
        {
            return new Vector3(Position.x, Position.y, Position.z);
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(Quaternion.x, Quaternion.y, Quaternion.z, Quaternion.w);
        }
    }
}