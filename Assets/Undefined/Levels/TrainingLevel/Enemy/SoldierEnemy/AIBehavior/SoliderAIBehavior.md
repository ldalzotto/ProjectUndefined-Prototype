# Soldier AI Behavior

## States Overview

````puml
            node PATROLLING
            node MOVE_TOWARDS_PLAYER
            node GO_ROUND_PLAYER
            node SHOOTING_AT_PLAYER
        
            PATROLLING -> MOVE_TOWARDS_PLAYER : PlayerObject in sight

            MOVE_TOWARDS_PLAYER -down-> SHOOTING_AT_PLAYER : PlayerObject in sight and close enough
            MOVE_TOWARDS_PLAYER -> GO_ROUND_PLAYER : PlayerObject lost sight \n behind an Obstacle

            SHOOTING_AT_PLAYER -> GO_ROUND_PLAYER : PlayerObject lost sight \n behind an Obstacle
            SHOOTING_AT_PLAYER -> MOVE_TOWARDS_PLAYER : PlayerObject lost sight

            GO_ROUND_PLAYER -> PATROLLING : PlayerObject cannot be found
            GO_ROUND_PLAYER -> MOVE_TOWARDS_PLAYER : A SightDirection has not been found 
            GO_ROUND_PLAYER -> SHOOTING_AT_PLAYER : PlayerObject has been found
````