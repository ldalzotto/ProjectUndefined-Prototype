# ProjectUndefined-Prototype

This repository is the source code of a proof of concept of a Unity game.

ProjectUndefined is a prototype of a top down 3D shooter. The Player can stop time whenever he wants and cast skill to progress trhough levels.

This project contains source code only, there is no asset files. I decided to strip assets from git history as they were taking space for nothing.

## Purpose

The purpose of this repository is to share pieces of code that can be inspiring for people who wants to develop similar features.

This project has no external documentation, your guide to understand how things work together will be comments (when there are some 😋). Feel free to dig into code to get a grasp of what is happening.

If you want more details on a specific feature, feel free to post an issue and I will answer to you as best as I can.

## Demo

### Controls

Controls are always displayed in-game and are dynamic depending of the current state. 

## Features

The application features (links redirect to source code implementation) :

* [Custom Player controller](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Common/PlayerObject/Lib/PlayerMovement)
* [Skill management for Player and AI](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Undefined/Logic/Skill).
  * [Firing projectiles.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Undefined/Logic/Projectile) 
  * [Deflecting projectiles when an incoming projectile is on range and health is low.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Undefined/Logic/ProjectileDeflection)
  * [Teleporting to a location in range.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Undefined/Logic/PlayerDash)
* [AI Behavior driven by fsm.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Common/~CoreGame/Behavior)
  * [Only one AI Behavior is implemented for soldier.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Undefined/Logic/Soldier/SoldierBehavior)
* [Interactive objects entirely defined via scriptable object that interact with the environment.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Common/InteractiveObjects)
  * [Health globe interactive objects that give health on trigger.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Undefined/Logic/HealthGlobe)
  * Player and AI are also InteractiveObjects.
* [3D Physics shape that takes into account obstacle occlusion to trigger game logic like AI line of sight for example.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Common/RangeObjects)
* [Start/Stop time.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Common/TimeManagement)
* [Custom Animation system based on the Playable API.](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/Common/AnimatorPlayable)
* [Custom toon shader integrated with the URP](https://github.com/ldalzotto/ProjectUndefined-Prototype/tree/master/Assets/_Shader/ToonUnlit)

## Dependencies


## License

Feel free to steal it.
