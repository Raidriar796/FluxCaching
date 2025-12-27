# FastBodyNodeSlot

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that adds caching to BodyNodeSlot ProtoFlux nodes for significantly faster querying. 

BodyNodeSlot searches an entire user's hierarchy every update, even if the actual result is no different. This is reliable, but expensive. The goal here is to cache results and only search again if something changes. This effectively turns the node from an O(n) operation to an O(1) operation while nothing is changing. I've tried to match vanilla behavior as closely as possible but there may be some inconstencies. Performance gains depend on how BodyNodeSlot is used and the size of the hierarchies it searches, your mileage may vary.

This mod tries to address concerns seen [in this discussion](<https://github.com/Yellow-Dog-Man/Resonite-Issues/discussions/3927>).

## Requirements
- [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader)

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [FastBodyNodeSlot.dll](https://github.com/Raidriar796/FastBodyNodeSlot/releases/latest/download/FastBodyNodeSlot.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs. 
