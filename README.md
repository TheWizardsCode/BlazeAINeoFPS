# NeoFPS and Blaze AI Demo 
Level

This is a simple little game to demonstrate the integration of [NeoFPS](https://assetstore.unity.com/packages/2d/gui/icons/pixel-cursors-109256?aid=1101l866w) and [Blaze AI](https://assetstore.unity.com/packages/tools/ai/blaze-ai-engine-194525?aid=1101l866w) to create a simple, but effective Shooter. 

## Installation

Follow the steps below. Note the links to the asset store are affiliate links, clicking them helps keep the lights on around here - even if you are not buying now.

### Import Required Assets

1. Checkout the repo
2. Open it in Unity 2021.3.0f1 or later
3. Import [NeoFPS](https://assetstore.unity.com/packages/2d/gui/icons/pixel-cursors-109256?aid=1101l866w) - when the NEO FPS Hub pops up you do NOT need to apply the recommended settings
4. Import `NeoFPSExtension_Cinemachine` which you can find in `Assets/NeoFPS/Extensions`
5. Import `NeoFPSExtension_InputSystem_V3` which you can find in `Assets/NeoFPS/Extensions`
6. Import [Blaze AI](https://assetstore.unity.com/packages/tools/ai/blaze-ai-engine-194525?aid=1101l866w)

NOTE when the NEO FPS Hub pops up you do NOT need to apply the recommended settings

## Play the Game

1. Open the scene `_BlazeNeo/Scenes/MainMenu`
2. Hit Play 
3. Select Level 1 and shoot stuff... but don't get shot... you know the drill.

### Controls

You can configure the keys in the setup menu. But the essential defaults are:

WASD - Forward, back, left and right
Mouse - Look
Left Mouse Button - fire
1-9 - select from inventory
Mouse scroll wheel - switch in inventory
E - pickup / interact
G - drop item in hands
Shift - run / walk (toggle)
CTRL - crouch
SPACE - jump

## Contributing

We welcome your contributions to this demo game created with [Blaze AI](https://assetstore.unity.com/packages/tools/ai/blaze-ai-engine-194525?aid=1101l866w) and [NeoFPS](https://assetstore.unity.com/packages/2d/gui/icons/pixel-cursors-109256?aid=1101l866w). Help us make these assets shine. It would be truly awesome if this got to be so good that one or both publishers could use the game to sell their assets. Lets make it happen so they keep building what we need to make games.

### Navigating the Project

`Assets/_BlazeNeo` contains the actual game scenes and assets using only BlazeAI and Neo FPS
`Assets/_Dev` not required at runtime - contains some dev scenes used for testing of the AI and game mechanics

There are other folders, but you might as well ignore them as they require additional assets and if not documented are still not usable. That said, feel free to poke around.
