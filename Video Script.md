
# Setting up Neo FPS + Universal AI

## Setup the Project

- Import `Neo FPS`
- Automatically apply settings

- Import `Universal AI 2.0`

- Import `Rifle Animset Pro`
- Remove `RifleAnimsetPro/Scripts`
- Remove `RifleAnimsetPro/PlaymakerAdditions`

- Import `Movement Animset Pro`
- Remove `MovementAnimsetPro/Scripts`
- Remove `MovementAnimsetPro/PlaymakerAdditions`

## Setup the Enemy Dev Scene

- Open `FeatureDemo_Template` scene
- Save As `_UniversalNep/Scenes/Enemy Dev Scene`

- Add `NeoFPS/Samples/Shared/Geomety/Character/Character_ThirdPerson`, reset to origin
- Open `Window/AI/Navigation` and bake the `NaveMesh`

- Hit `Play`

## Preparing the Animations

- Copy `EquipRifle` into `_UniversalNeo/Animations`
- Add `EnableWeapon` event to `EquipRifle` when one hand is on the weapon
- Add `EnableIK` event to `EquipRifle` when both hands are on the weapon

- Copy `HolsterRifle` into `_UniversalNeo/Animations`
- Add `DisableWeapon` event to `EquipRifle` when both hands leave the weapon

- Copy `Rifle_ShootOnce` into `_UniversalNeo/Animations`
- Add `UniversalAIAttack` event to `Rifle_ShootOnce`

## Setup Universal AI

- Open `Tools/Universal AI/AI Wizards/Integration Manager`
- Set `Neo FPS` to `Yes`

- Open `Tools/Universal AI/AI Wizards/Setup Manager`
- Setup the `Character_ThirdPerson` AI 

- Setup `Capsule Collider`
- Setup `Animations` and save the animation controller to `_UniversalNeo/Animations/Enemy OverrideController`

- Click `Open Detection` then `Open Detection Settings`, select `CharacterControllers` amd deselect `Default` in `Detection Layers`
- Click `Auto Find Head Transform`

- Hit Play
- Shoot the AI and he dies

## Setup AI Weapon

- Drop `NeoFPS/Samples/Shared/Geometry/Weapon_Low_AssaultRifle` in the right hand of the character
- Add `UniversalAIShooterWeapon` to the weapon model in the right hand

- Drop `NeoFPS/Samples/Shared/Geometry/Weapon_Low_AssaultRifle` in the holstered position on `FpsChar_Torso_Bone`

- Select `Weapon_Low_AssaultRifle` in the right hand
- Click `Weapon Settings` and select `Raycast` in `Fire Type`
- Set `Weapon Type` to `Single Shot`
- Set `Weapon Attack Distance` to `130`
- Set `Max Raycast Distance` to `130`
- Set `Raycast Hit Layers` to `CharacterPhysics`, disabling `Default`
- Set `Detection Distance` in the characters `Detection Settings` to 100

- Select the child `W Muszzle`
- Move it to the muzzle on the model

- On the character `Open Settings`, then `Open Attack`
- Set `Max Damage Distance` to 150   

- `Open Weapon Object Settings`
- Drag `Weapon_Low_AssaultRifle` on the back into `Holstered Weapon Object`
- Drag `Weapon_Low_AssaultRifle` in the right hand into `Main Weapon Object`

- Hit Play, Character mostly misses

- Select the `W Muzzle` object on the weapon in the right hand
- Adjust rotation so the debug line hits in the chest `1.7, 1.8, 0`

## Add IK

  - Open `Inverse Kinematics`
  - Set `Use Inverse Kinematics` to true
  - Set `Use Hand IK` to true
  - Set `Hand IK Type` to `Left Hand Only`
  - 
  - Enter play mode and adjust the `Right Hand IK` and `Left Hand IK` being sure to take a note of the settings
  - 

## Add Weapon SFX and VFX

  - Copy `NeoFPS/Samples/Shared/Prefabs/Weapons/MuzzleFlashes/RealisticMuzzleFlash_AssaultRifle` into the scene
  - Unpack the prefab and pull out the `Barrel Flare` child into its own object
  - Delete the original muzzle effect object
  - Rename `Barrel Flare` to `Muzzle Flash`
  - Make a prefab in `Prefabs/VFX/Muzzle Flash`

  - Move the `Muzzle Flash` to the tip of the Muzzle
  - Open `VFX Settings` on the weapon in the right hand
  - Open `Bullet Settings`
  - Drag `Muzzle Flash` drag in `NeoFPS/Samples/Shared/Prefabs/Weapons/MuzzleFlashes/RealisticMuzzleFlash_AssaultRifle`

  - Open `Bullet Settings`
  - Setup sounds using those supplied in `NeoFPS/Samples/Shared/Audio/SoundEffects/Weapons/`
  - Set the 3D sound on the Audio Source for the weapon to full 3D

## Add sound detection

  - Add `AINEOSoundDetection` component to your AI
  - Add `SoloPlayerCharacterEventWatcher` component to your AI

## Make a prefab

  - Make a prefab in `_UniversalNeo/Prefabs/Enemy`

# Boxing out a Level

In this section we will create a basic level in which the player must fight their way to a specific point, grab an item, then get to the extraction point.

## Scene Setup

  - Using a pen and paper or drawing tablet sketch out the level (see `_Design/Level Sketches/Scratchpad Level 1`)
  - Create a new scene `_UniversalNeo/Scenes/Scratchpad/Scratchpad Level 1`
  - Install `Probuilder`
  - Import the `Wizards Code Prototyping` Package

## Rough level layout

  - Add a `Terrain`
  - Adjust size to 250 x 500
  - Position at -125, 0, -250
  - Build up hills around the edge
  - Draw the road network 
  - Block out the buildings using ProBuilder
  - Dig out a lake and throw in a plane of water
  - Make a tree in ProBuilder
  - Paint some trees in

## Explore the level with NeoFPS

- Open `FeatureDemo_Template` scene additively
- Drag in all elements intot the Scratchpad scene
- Remove the old Camera and the test environment
- Position the `SimpleSpawnerAndGameMode` at the start point

- Hit `Play`

## Add Enemies

  - Drag a number of enemy prefabs into place based on your design
  - Mark all the buildings as `Not Walkable`
  - Bake the NavMesh

  - Hit `Play`

  - Double the number of Enemies
  - Increase enemy hearing range to 75

## AI Behaviour Tuning

  - In `Stats` set `Start Health` to 100
  - In `General Settings/Movement` set `Destory Delay` to 15
  - In  `General Settings/Attack` set `Min Attack Delay` to 0.1 and `Max Attack Delay` to 0.4
  - And more

## Create Squads

  - Create `Squad Alpha` Add in a bunch of AI
  - Create `Squad Beta` Add in a bunch of AI
  - Repeat until `Squad Hotel`

## Add Weapon Sounds

  - Set `Audio Source` on the weapon to have full `3D` sound
  - Open `VFX Settings`
  - Add `Audio_Weapon_AssaultRifle_Gunshot01` to `Fire Sound Effect`
  - Add `Audio_Weapon_AssaultRifle_Reload` to `Reload Sound Effect`
  - Add `Audio_Weapon_AssaultRifle_Draw` to `Equip Sound Effect`
  - Add `Audio_Weapon_AssaultRifle_Draw` to `UnEquip Sound Effect`
  - Make the distance drop off in the audio source of the weapon less steep

## Add Character Sounds

  - Set `Audio Source` on the enemy to have full `3D` sound
  - Open `Unversal AI Sounds`
  - Add `Audio_Footsteps_Dirt_Std01` and a few more (02, 03 etc.) to `Walk Sounds`
  - Add `Audio_Footsteps_Dirt_Fast01` and a few more (02, 03 etc.) to `Run Sounds`
  - Add `Audio_VoiceMale_HitReaction01` and a few more (02, 03 etc.) to `On Take Damage`
  - Add `Audio_VoiceMale_Pain01` and 02 to `On Death`

## Add Player Sounds

  - Copy `NeoFpsSoloPlayerCharacter` into your `Prefabs` Folder
  - Rename it to `UniversalNeoPlayer`
  - Use it in `SimpleSpawnerAndGameMode`
  - Change `Damage Audio Threshold` on `UniversalNeoPlayer/FpsSoloCharacter` to 5

## Add Health Pickups, dropped by the AI on death

  - Create a `Items/LootDrop` script
  - Add `ContactPickup_Health` as a health item
  - Set to `Spawn as Child`
  - Add `OnDropLoot` to the AI `OnDeath` event
  
## And the Render Texture Scope to the Sniper

  - Copy `DemoFacility_FirearmSniperRifle` to `Prefabs/Weapon/FirearmSniperRifle`
  - Replce default sniper in `UniversalNeoPlayer` with the new weapon
  - Disable all optics except the `Optics_ScopeSniper_RenderTexture`
  - Copy the `FpsInventoryWiedlable` script from the original sniper quickswitch
  - Remove the `FpsInventoryWieldableSwapppable` script from the new weapon
  - Paste as new the new inventory script
  
## Need more cover in the scene for the player

  - Add a whole bunch of small buildings and barrels onto the scene

## Weapon pickups

  - Remove all weapons except the pistol from `UniversalNeoPlayer`
  - Add an enemy in view when the player spawns
  - Replace the health pickup with `ContactPickup_AssaultRifle_Quickswitch` as a loot drop for that first enemy
  - On the `LootDrop` script add a second category of pickups called `RandomPickip`
  - On the enemy prefab add the various weapon pickups
  - Create a new Sniper pickup and use our modified sniiper item



