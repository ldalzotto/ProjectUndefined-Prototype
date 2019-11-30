# Soldier AI Behavior

## States Overview

````puml
            'default
            top to bottom direction

            skinparam node {
                backgroundColor<<start_state>> Lime
            }

            node SoldierAIbehavior {
                node PATROLLING
            }
           
            node TrackAndKillPlayerStateManager {
                node LISTENING<<start_state>>
                node MOVE_TOWARDS_PLAYER
                node GO_ROUND_PLAYER
                node SHOOTING_AT_PLAYER
                node MOVE_TO_LAST_SEEN_PLAYER_POSITION
            }

            node TrackUnknownStateManager {
                node MOVE_TOWARDS_INTEREST_DIRECTION
            }

            SoldierAIbehavior -> TrackAndKillPlayerStateManager : PlayerObject in sight
            TrackAndKillPlayerStateManager -> SoldierAIbehavior : Nothing happened

            LISTENING -> MOVE_TOWARDS_PLAYER : On TrackAndKillPlayerStateManager start 
            MOVE_TOWARDS_PLAYER -> LISTENING : On TrackAndKillPlayerStateManager exit 
            MOVE_TOWARDS_PLAYER -down-> SHOOTING_AT_PLAYER : PlayerObject in sight and close enough
            MOVE_TOWARDS_PLAYER -> GO_ROUND_PLAYER : PlayerObject lost sight \n behind an Obstacle
            MOVE_TOWARDS_PLAYER -> MOVE_TO_LAST_SEEN_PLAYER_POSITION : PlayerObject lost sight \n not behind an Obstacle

            SHOOTING_AT_PLAYER -> GO_ROUND_PLAYER : PlayerObject lost sight \n behind an Obstacle
            SHOOTING_AT_PLAYER -> MOVE_TO_LAST_SEEN_PLAYER_POSITION : PlayerObject lost sight

            GO_ROUND_PLAYER -> SHOOTING_AT_PLAYER : PlayerObject has been found
            GO_ROUND_PLAYER -up-> MOVE_TO_LAST_SEEN_PLAYER_POSITION : A SightDirection has not been found \n PlayerObject cannot be found

            MOVE_TO_LAST_SEEN_PLAYER_POSITION -down-> MOVE_TOWARDS_PLAYER : PlayerObject just in sight

            SoldierAIbehavior -> TrackUnknownStateManager : A damage has been received \n PlayerObject not in sight
            TrackUnknownStateManager --> SoldierAIbehavior : Nothing happened
````