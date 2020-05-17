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

* Custom Player controller (PlayerObject\Lib\PlayerMovement)
* Skill management for Player and AI. (Undefined\Logic\Skill)
  * Firing projectiles. (Undefined\Logic\Projectile, Undefined\Logic\PlayerAim) 
  * Deflecting projectiles when an incoming projectile is on range and health is low. (Assets\Undefined\Logic\ProjectileDeflection)
  * Teleporting to a location in range. (Undefined\Logic\PlayerDash)
* AI Behavior driven by fsm. (Common\~CoreGame\Behavior)
  * Only one AI Behavior is implemented for soldier. (Undefined\Logic\Soldier\SoldierBehavior)
* Interactive objects entirely defined via scriptable object that interact with the environment. (Common\InteractiveObjects)
  * Health globe interactive objects that give health on trigger. ()
  * Player and AI are also InteractiveObjects.
* 3D Physics shape that takes into account obstacle occlusion to trigger game logic like AI line of sight for example. (Common\RangeObjects)
* Start/Stop time. (Common\TimeManagement)
* Custom Animation system based on the Playable API. ()

## Dependencies


## License

Feel free to steal it.
