# NeoFPS and Universal AI 2.0 Demo Level

This is a simple little game to demonstrate the integration of NeoFPS and Universal AI, along with Kubold Rifle and Movement Animset Pro to create a simple, but effective Shooter. 

## Installation

Follow the steps below. Note the links to the asset store are affiliate links, clicking them helps keep the lights on around here - even if you are not buying now.

### Import Required Assets

1. Checkout the repo
2. Open it in Unity 2021.2.0f1 or later
3. Import [NeoFPS](https://assetstore.unity.com/packages/2d/gui/icons/pixel-cursors-109256?aid=1101l866w)
4. Apply all the required settings using the NeoFPS Hub
5. Import [Universal AI 2.0](https://assetstore.unity.com/packages/tools/ai/universal-ai-2-0-204185?aid=1101l866w)
6. Open `Tools/Universal AI/AI Wizards/Integration Manager` and change `Use NeoFPS` to `Yes`
7. Import [Rifle Animset Pro](https://assetstore.unity.com/packages/3d/animations/rifle-animset-pro-15098?aid=1101l866w)
8. Remove (or exclude from import) the `Scripts` and `PlaymakerAdditions` folder from `RifleAnimsetPro`
9. Import [Movement Animset Pro](https://assetstore.unity.com/packages/3d/animations/movement-animset-pro-14047?aid=1101l866w)
10. Remove (or exclude from import) the `Scripts` and `PlaymakerAdditions` folder from `MovementAnimsetPro`

### Configure Animations

Universal AI requires that you add events to some weapon animations. Follow these steps:

1. Find the following animations in the Kubold pack and duplicate them `EquipRifle`, `HolsterRifle` and `Rifle_ShootOnce`
2. Create a folder `Assets/Kubold - Added Events` or similar and copy your duplicated animations into this folder
3. On `EquipRifle` add `EnableWeapon` event at frame 16
4. On `EquipRifle` add `EnableIK` event at frame 29
5. On `EquipRifle` add `EnableWeapon` event at frame 16
6. On `HolsterRifle` add `DisableWeapon` event at frame 36
7. On `Rifle_ShootOnce` add `UniversalAIAttack` event at frame 2
8. Add these three animations to the AI prefab `Assets/_UniversalNeo/Prefabs/Enemy`

## Play the Game

1. Open the scene `_UniversalNeo/Scenes/Level 1`
2. Hit Play and shoot stuff... but don't get shot... you know the drill.

## Contributing

We welcome your contributions to this demo game created with Universal AI and NeoFPS. Help us make these assets shine. It would be truly awesome if this got to be so good that one or both publishers could use the game to sell their assets. Lets make it happen so they keep building what we need to make games.

### Navigating the Project

The following folders have content that are a part of the project:

`Assets/_Design` contains some prototyping assets that are used in the levels. These are all CC0 licensed or covered by the MIT license of this project.
`Assets/Dev` contains some dev scenes used for testing of the AI
`Assets/UniversalNeo` contains the actual game scenes and assets
