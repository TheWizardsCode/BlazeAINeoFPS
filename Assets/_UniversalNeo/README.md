
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
- Set `Detection Distance` in the characters `Detection Settings` to 50

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

  - Create a script `_UniversalNeo/Scripts/Runtime/AISoundDetection.cs`
  - Add the following code:
```
using NeoFPS;
using NeoFPS.ModularFirearms;
using UnityEngine;
using UniversalAI;

namespace WizardsCode.UniversalAIExtentions.NeoFPS
{
    public class AISoundDetection : MonoBehaviour, IPlayerCharacterSubscriber
    {
        [SerializeField, Tooltip("The radius within which the AI may detect sound.")]
        float radius = 25;
        
        private BaseShooterBehaviour shooter;
        private ICharacter player;

        private void OnDisable()
        {
            shooter.onShoot -= DetectSound;
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            shooter.onShoot -= DetectSound;

            shooter = GetComponent<BaseShooterBehaviour>();
            player = character;
            shooter.onShoot += DetectSound;
        }

        private void DetectSound(IModularFirearm source)
        {
            UniversalAIManager.SoundDetection(UniversalAIEnums.SoundType.ShootSound, radius, transform);
        }
    }
}
```
  - Add `AISoundDetection` component to your AI
  - Add `SoloPlayerCharacterEventWatcher` component to your AI


