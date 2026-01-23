# Phace-Multiplayer-Bullet-Hell
## Overview
Phace is a Top-Down Bullet-Hell with Multiplayer feature. 
It is constructed as a school project and is still subject to change. The idea is a fast paced **arcade bullet hell** in top-down perspective in which players may or may not _cooperate_ to beat wave after wave of enemies, beat bosses and reach highscores.
Phace takes place in unfathomable space, each player controlling their own spacecraft that we intend to make customizable and adjust gameplay accordingly.
+ Genre: Arcade Bullet-Hell
+ Engine: unity6 Version: 6000.0.62f1
+ Network: FishNet
+ Platform: PC
+ Controls: keyboard/mouse, controller
+ Target group: 12+ yrs
+ Contributors: Christof Kloninger, Daniel Keidel
## General
### Backstory
In the vast emptiness of space, there are many evil enemies!!

### Description
You play as a space hero that has to kill the evil mothership and it's minions.

### How to Start
Open the PlayMode in unity and select Multiplayer. This enables a second editor instance on runtime.
If not, goolge how to enable Multiplayer Editor in unity.

In main menu, please insert a name if you want it to save that profile. If you input nothing into it or Ip Adress, it assumes the values "NewPilot" and "localhost".
First instance presses Host to open a lobby. The second just needs to press Join if both players are present on the same network.
If you accidentally wrote an invalid ip-Adress into the input field, please write localhost into it. that should connet you to the started instance of the player 1 lobby.
If you can not connect into the game you think you should, please look up how to find your ipAdress and tell it your friend.
Both players have to press "Ready" in the lobby and then the host can start the game.

### Player
Each player controls a spacecraft

### Controls
WASD for movement, shift for speed-change,mouse for aiming/ship rotation,
mouse-left for single-shot, mouse-right for spreadshot, space for homing-missle.

### Bonus Features
Lobby, Homing missles, diffrent bullet patterns ( for example enemy wave shot, spreadshot ), Punktesystem

## Technical Details
Lobbysystem for the game start. Enemy prefabs are contained within scriptable objects.
Enemy waves are spawned. After defeating a wave by destroying all the spawned enemies, the next wave will spawn after a small delay. 
After three waves of diffrent enemy types are destroyed, the boss will appear.
The enemies utilise a NavMesh with a plugin for 2D environments. They can detect the nearest player via a 2D contact filter and a circle cast.
All attacks (players and enemies) have a cooldown system.

### Technichal Overview
It runs in Unity and uses fishnet.
### Rpcs
EnemyBossScript.cs [SERVER] → 9
EnemySpawnManager.cs [SERVER] → 4
EnemyTypeA.cs [SERVER] → 7
EnemyTypeB.cs [SERVER] → 8
EnemyTypeC.cs [SERVER] → 4
GameManager.cs[SERVER] → 3
Guns.cs [ServerRpc] → 4
Lobby.cs [Server] → 2
OwnLobbyManager.cs [Server] → 7
PlayerMovement.cs [ServerRpc] → 3
PlayerSession.cs [ServerRpc] → 1
PlayerStatsManager.cs [Server] → 2
ProjectileEnemyTypeA.cs [Server] → 1
ProjectileEnemyTypeB.cs [Server] → 1
ProjectileSpawnManager.cs [Server] → 7
Spacecraft.cs [Server] → 4
TestShipHoming.cs [Server] → 4
TestShipProjectile.cs [Server] → 1

### SyncVars
Various SyncVars are used.

### Bullet Logic
Player bullets hurt enemies and vice versa. Enemy and friendl bullets can collide with each other. Homingmissles do not collide with bullet.
There are variious attack patterns ( spreadshot, 360° shot, bullets that travel in a wave pattern etc.)

### Enemy Logic
3 regular enemy types + a boss. All have diffrent attacks. 
Some enemies can patrol while shooting at the nearest player, others can actively chase the nearest player. 
The boss can spawn smaller enemies. They all have diffrent attacks.

### Persistence Logic
Player profiles are saved and can be loaded.

#### Player
The players control spacecrafts with various weapon systems.

#### Scoreboard
In the post-game panel, players can look up their score

## Work Rules
These rules are for all contributors to follow and establish a healthy work environment. Tools we use for organization:
+ Discord
+ Miro
### Interpersonal Rules
+ Respect each other
+ Listen to each other
+ Open communication
+ We agree to not talk about the project between 0am - 8am
+ Coffee and cigarettes are each contributors own responsibility, sharing of tobacco is welcome
### Workflow rules
+ Each contributor works in their own Workspace branch
+ Overarching Issues are discussed and opened, assignment to an issue follows work capacity
+ Sub issues can be opened by the assigned contributor on their own without further discussion
+ We established a folder structure in unity to not disrupt each others workspace.
+ Scenes are established or copied to work on issues
+ Chris is authorative to version control, contributors commit to their workbranches after an issue is closed, merged project files are shared via the dev branch, contributors open their workbranch from that point on
### Naming conventions and commentary
+ Follow standard C# conventions
+ Summaries should describe the functionality and have a comment by the last editor of the codeblock, if the script is edited across the team
+ Comments written above a line are either to point out relevance in the flow or help keeping track for the editor
+ Comments written right of the lineshould point out issues, queries, bugs and other special cases
