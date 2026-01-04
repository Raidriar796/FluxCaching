# FluxCaching

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that speeds up ProtoFlux nodes that do repeated searching. 

Many ProtoFlux nodes search through slot hierarchies and components on slots every time they are queried. This is fine for reliability, but can cause slow downs with frequent use and when searching for through large hierarchies. This mod gives these nodes the abilty to "remember" what they found and simply repeat it unless something changes, providing significant speed boosts in some situations.

## ⚠️Warning⚠️
**DO NOT USE THIS MOD AS A CRUTCH FOR BAD PROGRAMMING PRACTICES.** This mod solves a symptom of a problem, not the problem itself.
Said problem is the misuse of continuously updating ProtoFlux nodes, such that ProtoFlux is updating every engine update when they don't need to be. This does NOT mean that continuously updating ProtoFlux is bad, but you should be careful about what you're doing when you are using continuously updating ProtoFlux nodes.

If you notice that the output of a node changes between the mod being enabled and disabled, that is considered a bug. Please open an issue about it.

This mod tries to address concerns seen [in this discussion](<https://github.com/Yellow-Dog-Man/Resonite-Issues/discussions/3927>).

## Supported ProtoFlux Nodes 

- BodyNodeSlot
  - Reduced from an at worst O((n+m)+(x+y)) to a near O(1) cost.
- BodyNodeSlotInChildren
  - Reduced from an at worst O((n+m)+(x+y)) to a near O(1) cost.
- FIndChildByName
  - Reduced from an at worst O(n) to a near O(1) cost.
- FIndChildByTag
  - Reduced from an at worst O(n) to a near O(1) cost.
- FIndParentByName
  - Reduced from an at worst O(n) to a near O(1) cost.
- FIndParentByTag
  - Reduced from an at worst O(n) to a near O(1) cost.

## Requirements
- [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader)

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [FluxCaching.dll](https://github.com/Raidriar796/FluxCaching/releases/latest/download/FluxCaching.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs. 
