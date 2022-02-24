# Simple H Chararacter Adjustment

This plugin move female character closer/away or up/down in scene with two characters.  
This is the easiest way I found to avoid some clipping situations.

This plugin is base on @DeathWeasel1337 KK_HCharaAdjustment 2.0 for now it can be a replacement 
for it.  The plugin has the added functionality using shortcuts by default:

**Female**
- L key to "Move Female Buttons"

**Male**
- LeftControl+L key to "Move Male Buttons"

When moving closer or apart the characters stay aligned the orientation in the map is not important.

If HCharaAdjustment is not loaded this plugin will provide the functionality of the guides.

**Dependencies**

* **IllusionMods.BepInEx 5.4.15** - The core BepInEx library that contains all important core API.
* **IllusionMods.BepInEx 2.5.4** - Harmony fork that is used in BepInEx.
* **KKAPI 1.24.0** - Modding API for Koitatu.
* **UnityEngine.Core 5.6.1** - Stubbed UnityEngine 5.6.1.

**Known Problems**

-If KK_HCharaAdjustment/KKS_HCharaAdjustment and you are going to use the guides for movements use
the guides before.  If not HCharaAdjusment won't be able to reset to the original position. The 
original position is sets when the guide activates.
-~~If you first use the shortcut keys when the 
guides are activated the original position saved will be wrong.~~

# Road-map

~~Extend the functionality to the male character.~~

~~Easier way to position the characters.~~
