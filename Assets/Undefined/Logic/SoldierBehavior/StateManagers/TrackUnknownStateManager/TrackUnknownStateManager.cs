using AIObjects;

namespace SoliderAIBehavior
{
    public enum TrackUnknownStateEnum
    {
        MOVE_TOWARDS_INTEREST_DIRECTION
    }

    public class TrackUnknownStateManager : SoldierStateManager
    {
        private TrackUnknownBehavior TrackUnknownBehavior;

        public TrackUnknownStateManager()
        {
            this.TrackUnknownBehavior = new TrackUnknownBehavior();
        }
        public override void Tick(float d)
        {
            base.Tick(d);
            this.TrackUnknownBehavior.Tick(d);
        }
    }

    /// <summary>
    /// The <see cref="SoldierAIBehavior"/> will move in direction of interset direction <see cref="TrackUnknownInterestDirectionSystem"/>.
    /// </summary>
    public class TrackUnknownBehavior : AIBehavior<TrackUnknownStateEnum, SoldierStateManager>
    {
        private TrackUnknownInterestDirectionSystem TrackUnknownInterestDirectionSystem;

        public TrackUnknownBehavior() : base(TrackUnknownStateEnum.MOVE_TOWARDS_INTEREST_DIRECTION)
        {
            this.TrackUnknownInterestDirectionSystem = new TrackUnknownInterestDirectionSystem();
        }
    }
}