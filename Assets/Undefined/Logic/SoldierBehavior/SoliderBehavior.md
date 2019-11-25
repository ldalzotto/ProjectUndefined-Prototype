# Soldier AI Behavior

## States Overview

````puml
            node PATROLLING
        
            package TrackAndKillPlayerStateManager {
                node MOVE_TOWARDS_PLAYER
                node GO_ROUND_PLAYER
                node SHOOTING_AT_PLAYER
                node MOVE_TO_LAST_SEEN_PLAYER_POSITION
            }

            PATROLLING -> MOVE_TO_LAST_SEEN_PLAYER_POSITION : PlayerObject in sight
            MOVE_TO_LAST_SEEN_PLAYER_POSITION -> PATROLLING : Nothing happened

            MOVE_TOWARDS_PLAYER -down-> SHOOTING_AT_PLAYER : PlayerObject in sight and close enough
            MOVE_TOWARDS_PLAYER -> GO_ROUND_PLAYER : PlayerObject lost sight \n behind an Obstacle
            MOVE_TOWARDS_PLAYER -> MOVE_TO_LAST_SEEN_PLAYER_POSITION : PlayerObject lost sight \n not behind an Obstacle

            SHOOTING_AT_PLAYER -> GO_ROUND_PLAYER : PlayerObject lost sight \n behind an Obstacle
            SHOOTING_AT_PLAYER -> MOVE_TO_LAST_SEEN_PLAYER_POSITION : PlayerObject lost sight

            GO_ROUND_PLAYER -> SHOOTING_AT_PLAYER : PlayerObject has been found
            GO_ROUND_PLAYER -up-> MOVE_TO_LAST_SEEN_PLAYER_POSITION : A SightDirection has not been found \n PlayerObject cannot be found

            MOVE_TO_LAST_SEEN_PLAYER_POSITION -down-> MOVE_TOWARDS_PLAYER : PlayerObject just in sight
````