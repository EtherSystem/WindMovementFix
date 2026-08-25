# WindMovementFix

This is a very simple mod aimed at fixing the wind movement speed debuff bug.

In vanilla, as soon as the wind is supposed to start slowing the player down, the strongest possible speed debuff is applied. After that, the stronger the wind gets, the weaker the speed debuff becomes.

This mod is aimed at fixing exactly that.

Now, this mod does not *only* fix this bug.

In vanilla, the maximum speed debuff is supposed to be reached at around 65 MPH winds, which is already among the strongest winds you can normally encounter. This mod raises that limit to 85 MPH, with a maximum movement speed debuff of 65%.

In vanilla, this should basically change nothing, since winds stronger than 65 MPH are almost never reached, if they are even reachable at all.

However, this mod exists because I'm also creating WeatherOverhaul, which includes custom weather stages capable of producing winds stronger than 65 MPH. Instead of maintaining multiple patches for the same system for no real reason, support for those stronger winds is included directly here.

Be aware that by installing this mod, and therefore actually fixing the wind movement bug, blizzards will now be deadlier since they will **really** slow you down.

Because yes: in vanilla, a blizzard basically doesn't slow you down at all.

Nothing more than `MelonLoader 0.7.2 non-nightly` is required for this mod to work.

Good luck.
