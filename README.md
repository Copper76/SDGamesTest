This is the repository for the practical test for SD games. The demo consists three systems described below:

# Player 3C System
## Features:
- Ability to move around using WASD and jump using SPACE BAR.
- Ability to rotate camera using mouse with limitations and characters with them, working as a 3rd person overshoulder camera.
- Ability to use E to interact with objects in the world(including picking up collectibles).
- Prompt player to use E when they are in range of interaction.
- Use I to open the inventory menu, more on that in the inventory system.
- Use C to open the stat menu.
- Move into collectibles to pick them up as an alternative method.
- Dust VFX using particle system when player is walking on ground, SFX was not quite possible as I don't have any audio devices on my working PC for now.
- Seamlessly transition between different menus.

# Building System
## Features:
- Press E when close to building stations to open building menu.
- Building menu displays the current resources held by player and the amount needed for the next level of building.
- Upgrade and Destroy menu appearing when the player is able to upgrade or destroy building, the materials are automatically consumed when the building upgrades.
- An icon for building in the middle of the menu to reflect the level of building(palceholder for now)
- Visual changes to the building when it is upgraded or destroyed.
- To compensate for the need of resources, a script for regularly spawning copper and gold ingots is used to continuously generate resources in the scene.

# Inventory System
Doing this system was a bit cheating as I have already done a demo on it. However, I added more features here so it still includes new stuff that was never seen in my previous demo.
## Features
- Press I to open the inventory menu.
- Hover cursor on an occupied slot to show the related information.
- Click and drag items to place them in different slots.
- Drag item in slots with the same item to stack them up to an upper limit(currently the ingots stack up to 64 and equipments don't stack).
- Drag item outside of the menu to drop them. You can drop a stack of item as well.
- Release item into the menu would place them back into the original slot.
- Drag item into equipment slot that can hold it(e.g. Sword for weapon, boots for leg) would grant player stat boost. The changes are visible in stat menu, and currently player would move faster with boots equipped.
- Potentially more once I clarify what the item class should do.

# Credit
The 2D pixel art was from the Pixel Art Icon Pack - RPG created by Cainos (Asset Store Link)[https://assetstore.unity.com/packages/2d/gui/icons/pixel-art-icon-pack-rpg-158343#description].
