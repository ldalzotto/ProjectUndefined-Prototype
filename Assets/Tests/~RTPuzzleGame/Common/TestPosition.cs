using UnityEngine;

namespace Tests
{
    public class TestPosition : MonoBehaviour
    {
        [CustomEnum()]
        public TestPositionID aITestPositionID;
    }

    public enum TestPositionID
    {
        ATTRACTIVE_OBJECT_NOMINAL = 0,
        PROJECTILE_TARGET_1 = 1,
        PROJECTILE_TARGET_2 = 2,
        FAR_AWAY_POSITION_1 = 3,
        PITFAL_Z_POSITION_1 = 4,
        PITFAL_Z_POSITION_FAR_EDGE = 5,
        OBSTACLE_LISTENER_POSITION_1 = 6,
        OBSTACLE_LISTENER_POSITION_2 = 7,
        OBSTACLE_LISTENER_POSITION_3 = 8,
        OBSTACLE_LISTENER_POSITION_4 = 9,
        MOVE_TOWARDS_PLAYER_INSIGHT = 10,
        PROJECTILE_TOATTRACTIVE_NOMINAL = 11,
        MOVE_TOWARDS_PLAYER_OUTOFSIGHT = 12,
        AI_INITIAL_POSITION_1 = 13
    }

}
