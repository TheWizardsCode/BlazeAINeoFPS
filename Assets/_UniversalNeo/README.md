# Setting up Neo FPS + Universal AI

## Setup the Project

- Import `Neo FPS`
- Import `Universal AI 2.0`
- Import `Rifle Animset Pro`

## Setup the Enemy Dev Scene

- Open `FeatureDemo_Template` scene
- Save As `_UniversalNep/Scenes/Enemy Dev Scene`
- Add `NeoFPS/Samples/Shared/Geomety/Character/Character_ThirdPerson`, reset to origin
- Open `Window/AI/Navigation` and bake the `NaveMesh`
- Hit `Play`

## Preparing the Animations

- Copy `EquipRifle` into `_UniversalNeo/Animations`
- Add `EnableWeapon` event to `EquipRifle`

## Setup Universal AI

- Open `Tools/Universal AI/AI Wizards/Integration Manager`
- Set `Neo FPS` to `Yes`
- Open `Tools/Universal AI/AI Wizards/Setup Manager`
- Setup the `Character_ThirdPerson` AI 
- Setup `Capsule Collider`
- Setup `Animations` and save the animation controller to `_UniversalNeo/Animations/Enemy OverrideController`
- Click `Open Detection` then `Open Detection Settings` and click `Auto Find Head Transform`
- Hit Play

