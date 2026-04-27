# GDIM33 Vertical Slice
## Milestone 1 Devlog
The BossAdds script machine allows the boss to summon minions at certain intervals. It has 8 Vector3 graph variables, each representing a position in one of the map's 8 compass direction (N, NE, NW, etc).
These Vector3s are used to determine the summoning locations for the three custom events in the graph: CardinalMageSummon, CardinalHealerSummon, and DiagonalTankSummon.
CardinalMageSummon calls the GameController singleton's SummonRisen() method, which summons an enemy of a given class type at a given Vector3 location. The Vector3 is set to the CenterNorth graph variable and the class is set to Mage, so the event summons a mage in the map's northern portion.
CardinalHealerSummon fuctions much the same, calling SummonRisen() 4 times, taking in 4 of the graph Vector3 variables, summoning enemy healers in North, East, South, and West positions on the map. DiagonalTankSummon summons enemy tanks in the 4 diagonal directions. 
This script machine is attached to the Boss gameobject, and the custom events are triggered by the Boss's state machine at either specific health thresholds or regular attack intervals. 

<img width="1728" height="1728" alt="GDIM 33 Game Breakdown" src="https://github.com/user-attachments/assets/924bc11f-614e-48c2-9054-6ce35f601cef" />


I added the Boss, Phase 1, Phase 2, and Phase 3 circles to my game breakdown. The Boss inherits from a basic enemy script, has multiple attacks and abilities, and 3 behavior states/phases. Each phase circle describes the boss's suite of abilities during said phase, targeting behavior, and movement behavior. 
Every state in the BossStateMachine works in a similar way. Upon entering the state, Object variables determining the seconds between each boss attack and target switch is set to a specific amount of seconds. As the phase number increases, that time gets shorter. Timer object variables tracking the number of seconds until the next attack or target switch are set to zero, and Time.FixedDeltaTime is subtracted from each on every Fixed Update.
If the target switch timer reaches zero, it is reset back to its cooldown value, and the graph calls a targeting script from the Boss script based on the current number of target switches so far (tracked through an index variable). For example, in phase 1, the boss will loop between targeting the nearest and farthest player characters.
If the attack timer reaches zero, it is also reset back to its cooldown value. The boss then activates an attack ability based on the current number of attacks so far (also tracked through an index). In phase 1, the boss alternates between 3 attacks: basic projectile, basic melee, and the summoning of mages. In phase 2 and 3 the boss loops between 10 attacks. 
At the end of every FixedUpdate in phases 1 and 2, the state machine checks if the boss's health has dropped below a certain threshold. If so, a custom event is triggered to transition the boss into the next phase. 

The state machine is directly attached to the Boss gameobject and closely related to it. Most attacks and every target switch is achieved by calling methods in the Boss script. When the boss activates a summoning ability, the script machine triggers an event from the BossAdds graph, also attached to the boss, which interacts with the GameObject singleton to summon additional enemies.
By controlling the Boss's behavior, the machine also relates to the player and allied NPC (which are ghost replays of the player) systems. The boss's behavior forces the player to respond. When the boss launches an attack, the player will usually attempt to dodge it. When the boss switches targets, the player may wish to protect the new target and adjust their own strategy. The player and their past selves will take damage and die as a result of attacks activated by the boss state machine.





## Milestone 2 Devlog
Milestone 2 Devlog goes here.
## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- [Homing Projectile](https://assetstore.unity.com/packages/3d/characters/humanoids/lowpoly-mushroomman-character-287820)
- [Fireball Projectile](https://bdragon1727.itch.io/fire-pixel-bullet-16x16)
- [Boss Healthbar Font](https://freefonts.co/fonts/fot-matisse-pro-eb)
- [UI Font](https://font.download/font/fixedsys-excelsior-301)
- [Rat](https://rurr.itch.io/rat)
