# Welcome

This repository is the source code of a proof of concept of a Unity game.

ProjectUndefined is a prototype of a top down 3D shooter. The Player can stop time whenever he wants and cast skill to progress trough levels.

This project was made to challenge my knowledge acquired with the unity engine. This project emphasis my will of building custom solutions for features that I found too limiting in the engine : animation system, surface shader, prefabs. I am satisfied with the solutions that I brought, but the code is not very comprehensive.

This repository contains source code only, there is no asset files. I decided to strip assets from git history as they were taking space for nothing. The whole unity project with assets is available [here](//todo).

Some features are more detailed [here](https://ldalzotto.github.io/ProjectUndefined-Prototype/), these are some that I could reuse in future projects.

## Try it

![](https://github.com/ldalzotto/gif/blob/master/ezgif-1-80b82499a60c.gif)

A playable demo is [here](https://github.com/ldalzotto/ProjectUndefined-Prototype/releases/tag/0.0.178).

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