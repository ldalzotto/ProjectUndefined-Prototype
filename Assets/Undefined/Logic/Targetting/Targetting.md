# Targetting

## Locked targetting

Because the game camera is isometric, it is often hard to properly aim at a Target because perspective can be misleading.
<br/>
Thus, when the Player aim at a Target on screen, 
then the Player orientation is automatically adjusted to aim at the target in game space.
<br/>
A visual feedback (like an outline) can indicate to the Player what target he is currently aiming at.
<br/>
If multiple targets are overlapping at the cursor position, then the player can switch between them by pressing a Key.

````puml
@startuml
skinparam node {
    backgroundColor<<InteractiveObject>> orange
    backgroundColor<<System>> yellow
    backgroundColor<<PlayerAction>> cyan
    backgroundColor<<Manager>> plum
}

package Targetting {
    node FiringPlayerAction <<PlayerAction>>
    node TargetCursorSystem <<System>>
}

    node TargettableInteractiveObjectScreenIntersectionManager <<Manager>>
node TargettableInteractiveObjectSelectionManager <<Manager>>
interface OnInteractiveObjectCreated

FiringPlayerAction --> TargetCursorSystem : Update
TargetCursorSystem --> FiringPlayerAction : Provide data

OnInteractiveObjectCreated --> TargettableInteractiveObjectScreenIntersectionManager
TargetCursorSystem --> TargettableInteractiveObjectScreenIntersectionManager : Cursor position
TargettableInteractiveObjectScreenIntersectionManager -> TargettableInteractiveObjectSelectionManager : OnCursorOverObject
TargettableInteractiveObjectSelectionManager --> FiringPlayerAction : Locked Target
@enduml
````