# NeoFPS and Blaze AI Demo Level

This is a simple little game to demonstrate the integration of [NeoFPS](https://assetstore.unity.com/packages/2d/gui/icons/pixel-cursors-109256?aid=1101l866w) and [Blaze AI](https://assetstore.unity.com/packages/tools/ai/blaze-ai-engine-194525?aid=1101l866w), along with Kubold [Rifle Animset Pro](https://assetstore.unity.com/packages/3d/animations/rifle-animset-pro-15098?aid=1101l866w) and [Movement Animset Pro](https://assetstore.unity.com/packages/3d/animations/movement-animset-pro-14047?aid=1101l866w) to create a simple, but effective Shooter. 

## Installation

Follow the steps below. Note the links to the asset store are affiliate links, clicking them helps keep the lights on around here - even if you are not buying now.

### Import Required Assets

1. Checkout the repo
2. Open it in Unity 2021.2.0f1 or later
3. Import [NeoFPS](https://assetstore.unity.com/packages/2d/gui/icons/pixel-cursors-109256?aid=1101l866w)
4. Apply all the required settings using the NeoFPS Hub
5. Import [Blaze AI](https://assetstore.unity.com/packages/tools/ai/blaze-ai-engine-194525?aid=1101l866w)
6. Open `Tools/Universal AI/AI Wizards/Integration Manager` and change `Use NeoFPS` to `Yes`
7. Import [Rifle Animset Pro](https://assetstore.unity.com/packages/3d/animations/rifle-animset-pro-15098?aid=1101l866w)
8. Remove (or exclude from import) the `Scripts` and `PlaymakerAdditions` folder from `RifleAnimsetPro`
9. Import [Movement Animset Pro](https://assetstore.unity.com/packages/3d/animations/movement-animset-pro-14047?aid=1101l866w)
10. Remove (or exclude from import) the `Scripts` and `PlaymakerAdditions` folder from `MovementAnimsetPro`

### Configure Animations

Universal AI requires that you add events to some weapon animations. Follow these steps:

1. Find the following animations in the Kubold pack and duplicate them: `Rifle_ShootOnce`
2. Create a folder `Assets/Animations` or similar and copy your duplicated animations into this folder
3. On `Rifle_ShootOnce` add `ShootFrame` event at frame 8

## Play the Game

1. Open the scene `_BlazeNeo/Scenes/MainMenu`
2. Hit Play 
3. Select Level 1 and shoot stuff... but don't get shot... you know the drill.

## Contributing

We welcome your contributions to this demo game created with Universal AI and NeoFPS. Help us make these assets shine. It would be truly awesome if this got to be so good that one or both publishers could use the game to sell their assets. Lets make it happen so they keep building what we need to make games.

### Navigating the Project

The following folders have content that are a part of the project:

`Assets/_Design` contains some prototyping assets that are used in the levels. These are all CC0 licensed or covered by the MIT license of this project.
`Assets/Dev` contains some dev scenes used for testing of the AI
`Assets/_BlazeNeo` contains the actual game scenes and assets

There are other folders, but you might as well ignore them as they require additional assets and if not documented are still not usable.
