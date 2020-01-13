using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace PlayerObject
{
    public class PlayerSpeedAttenuationSystem : IObjectSpeedAttenuationValueSystem
    {
        private PlayerSpeedAttenuationConstraintLayer PlayerSpeedAttenuationConstraintLayer;
        private PlayerSpeedAttenuationLayer PlayerSpeedAttenuationLayer;

        public PlayerSpeedAttenuationSystem()
        {
            this.PlayerSpeedAttenuationConstraintLayer = new PlayerSpeedAttenuationConstraintLayer();
            this.PlayerSpeedAttenuationLayer = new PlayerSpeedAttenuationLayer();
        }

        public AIMovementSpeedAttenuationFactor CurrentMovementSpeedAttenuationFactor()
        {
            return ObjectSpeedAttenuationCalculation.CalculateAIMovementSpeedAttenuationFactor(this.PlayerSpeedAttenuationConstraintLayer, this.PlayerSpeedAttenuationLayer);
        }

        #region External Events

        public void OnLowHealthStarted()
        {
            this.PlayerSpeedAttenuationConstraintLayer.OnLowHealthStarted();
        }

        public void OnLowHealthEnded()
        {
            this.PlayerSpeedAttenuationConstraintLayer.OnLowHealthEnded();
        }

        public void StartAiming()
        {
            this.PlayerSpeedAttenuationLayer.StartTargetting();
        }

        public void StopTargetting()
        {
            this.PlayerSpeedAttenuationLayer.StopTargetting();
        }

        #endregion

        public void SetRule(IObjectSpeedAttenuationConstraint objectSpeedAttenuationConstraint)
        {
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor speedAttenuationFactor)
        {
        }
    }

    #region Speed constraint layer

    enum PlayerSpeedAttenuationConstraintLayerState
    {
        DEFAULT,
        LOW_ON_HEALTH
    }

    abstract class PlayerSpeedAttenuationContraintStateManager : SpeedAttenuationContraintStateManager
    {
        public virtual void OnLowHealthStarted()
        {
        }

        public virtual void OnLowHealthEnded()
        {
        }
    }

    class DefaultPlayerSpeedAttenuationContraintStateManager : PlayerSpeedAttenuationContraintStateManager
    {
        private PlayerSpeedAttenuationConstraintLayer PlayerSpeedAttenuationConstraintLayerRef;

        public DefaultPlayerSpeedAttenuationContraintStateManager(PlayerSpeedAttenuationConstraintLayer playerSpeedAttenuationConstraintLayerRef)
        {
            PlayerSpeedAttenuationConstraintLayerRef = playerSpeedAttenuationConstraintLayerRef;
            this.AssociatedIObjectSpeedAttenuationConstraint = new NoneSpeedAttenuationConstraint();
        }

        public override void OnLowHealthStarted()
        {
            this.PlayerSpeedAttenuationConstraintLayerRef.SetState(PlayerSpeedAttenuationConstraintLayerState.LOW_ON_HEALTH);
        }
    }

    class LowOnHealthPlayerSpeedAttenuationContraintStateManager : PlayerSpeedAttenuationContraintStateManager
    {
        private PlayerSpeedAttenuationConstraintLayer PlayerSpeedAttenuationConstraintLayerRef;

        public LowOnHealthPlayerSpeedAttenuationContraintStateManager(PlayerSpeedAttenuationConstraintLayer playerSpeedAttenuationConstraintLayerRef)
        {
            PlayerSpeedAttenuationConstraintLayerRef = playerSpeedAttenuationConstraintLayerRef;
            this.AssociatedIObjectSpeedAttenuationConstraint = new NotAboveSpeedAttenuationConstraint(AIMovementSpeedAttenuationFactor.WALK_INJURED);
        }

        public override void OnLowHealthEnded()
        {
            this.PlayerSpeedAttenuationConstraintLayerRef.SetState(PlayerSpeedAttenuationConstraintLayerState.DEFAULT);
        }
    }

    class PlayerSpeedAttenuationConstraintLayer : SpeedAttenuationConstraintLayer<PlayerSpeedAttenuationConstraintLayerState, PlayerSpeedAttenuationContraintStateManager>
    {
        public PlayerSpeedAttenuationConstraintLayer()
        {
            this.StateManagersLookup = new Dictionary<PlayerSpeedAttenuationConstraintLayerState, PlayerSpeedAttenuationContraintStateManager>()
            {
                {PlayerSpeedAttenuationConstraintLayerState.DEFAULT, new DefaultPlayerSpeedAttenuationContraintStateManager(this)},
                {PlayerSpeedAttenuationConstraintLayerState.LOW_ON_HEALTH, new LowOnHealthPlayerSpeedAttenuationContraintStateManager(this)}
            };
            base.Init(PlayerSpeedAttenuationConstraintLayerState.DEFAULT);
        }

        public void OnLowHealthStarted()
        {
            this.GetCurrentStateManager().OnLowHealthStarted();
        }

        public void OnLowHealthEnded()
        {
            this.GetCurrentStateManager().OnLowHealthEnded();
        }
    }

    #endregion

    #region Speed attenuation layer

    enum PlayerSpeedAttenuationLayerState
    {
        DEFAULT,
        TARGETTING
    }

    abstract class PlayerSpeedAttenuationValueStateManager : SpeedAttenuationValueStateManager
    {
        public virtual void StartTargetting()
        {
        }

        public virtual void StopTargetting()
        {
        }
    }

    class DefaultPlayerSpeedAttenuationValueStateManager : PlayerSpeedAttenuationValueStateManager
    {
        private PlayerSpeedAttenuationLayer PlayerSpeedAttenuationLayerRef;

        public DefaultPlayerSpeedAttenuationValueStateManager(PlayerSpeedAttenuationLayer PlayerSpeedAttenuationLayerRef)
        {
            this.PlayerSpeedAttenuationLayerRef = PlayerSpeedAttenuationLayerRef;
            this.AssociatedAIMovementSpeedAttenuationFactor = AIMovementSpeedAttenuationFactor.RUN;
        }

        public override void StartTargetting()
        {
            this.PlayerSpeedAttenuationLayerRef.SetState(PlayerSpeedAttenuationLayerState.TARGETTING);
        }
    }

    class TargettingPlayerSpeedAttenuationValueStateManager : PlayerSpeedAttenuationValueStateManager
    {
        private PlayerSpeedAttenuationLayer PlayerSpeedAttenuationLayerRef;

        public TargettingPlayerSpeedAttenuationValueStateManager(PlayerSpeedAttenuationLayer playerSpeedAttenuationLayerRef)
        {
            PlayerSpeedAttenuationLayerRef = playerSpeedAttenuationLayerRef;
            AssociatedAIMovementSpeedAttenuationFactor = AIMovementSpeedAttenuationFactor.WALK;
        }

        public override void StopTargetting()
        {
            this.PlayerSpeedAttenuationLayerRef.SetState(PlayerSpeedAttenuationLayerState.DEFAULT);
        }
    }

    class PlayerSpeedAttenuationLayer : SpeedAttenuationValueLayer<PlayerSpeedAttenuationLayerState, PlayerSpeedAttenuationValueStateManager>
    {
        public PlayerSpeedAttenuationLayer()
        {
            this.StateManagersLookup = new Dictionary<PlayerSpeedAttenuationLayerState, PlayerSpeedAttenuationValueStateManager>()
            {
                {PlayerSpeedAttenuationLayerState.DEFAULT, new DefaultPlayerSpeedAttenuationValueStateManager(this)},
                {PlayerSpeedAttenuationLayerState.TARGETTING, new TargettingPlayerSpeedAttenuationValueStateManager(this)}
            };

            base.Init(PlayerSpeedAttenuationLayerState.DEFAULT);
        }

        public void StartTargetting()
        {
            this.GetCurrentStateManager().StartTargetting();
        }

        public void StopTargetting()
        {
            this.GetCurrentStateManager().StopTargetting();
        }
    }

    #endregion
}