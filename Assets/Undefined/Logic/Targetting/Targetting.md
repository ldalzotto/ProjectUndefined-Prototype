# Targetting

## Targetting

Targetting is done visualized by a target cursor on screen. <br/>
The TargetCursor is always displayed and moving according to mouse mouvement. <br/>
But it's position is not updated when camera movement occurs.

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

package PlayerObject {
    node FiringPlayerAction <<PlayerAction>>
}

node TargetCursorManager <<Manager>>
node TargettableInteractiveObjectScreenIntersectionManager <<Manager>>
node TargettableInteractiveObjectSelectionManager <<Manager>>
interface OnInteractiveObjectCreated

TargetCursorManager -> FiringPlayerAction : Provide data

OnInteractiveObjectCreated -down-> TargettableInteractiveObjectScreenIntersectionManager
TargetCursorManager -down-> TargettableInteractiveObjectScreenIntersectionManager : Cursor position
TargettableInteractiveObjectScreenIntersectionManager -> TargettableInteractiveObjectSelectionManager : OnCursorOverObject
TargettableInteractiveObjectSelectionManager -> FiringPlayerAction : Locked Target
@enduml
````