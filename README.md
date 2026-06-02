# GDIM33 Vertical Slice
## Milestone 1 Devlog
The BossAdds script machine allows the boss to summon minions at certain intervals. It has 8 Vector3 graph variables, each representing a position in one of the map's 8 compass direction (N, NE, NW, etc).
These Vector3s are used to determine the summoning locations for the three custom events in the graph: CardinalMageSummon, CardinalHealerSummon, and DiagonalTankSummon.
CardinalMageSummon calls the GameController singleton's SummonRisen() method, which summons an enemy of a given class type at a given Vector3 location. The Vector3 is set to the CenterNorth graph variable and the class is set to Mage, so the event summons a mage in the map's northern portion.
CardinalHealerSummon fuctions much the same, calling SummonRisen() 4 times, taking in 4 of the graph Vector3 variables, summoning enemy healers in North, East, South, and West positions on the map. DiagonalTankSummon summons enemy tanks in the 4 diagonal directions. 
This script machine is attached to the Boss gameobject, and the custom events are triggered by the Boss's state machine at either specific health thresholds or regular attack intervals. 

<img width="1728" height="1728" alt="GDIM 33 Game Breakdown (1)" src="https://github.com/user-attachments/assets/9fdf427f-3729-4ec9-a747-374e78496625" />


I added the Boss, Phase 1, Phase 2, and Phase 3 circles to my game breakdown. The Boss inherits from a basic enemy script, has multiple attacks and abilities, and 3 behavior states/phases. Each phase circle describes the boss's suite of abilities during said phase, targeting behavior, and movement behavior. 
Every state in the BossStateMachine works in a similar way. Upon entering the state, Object variables determining the seconds between each boss attack and target switch is set to a specific amount of seconds. As the phase number increases, that time gets shorter. Timer object variables tracking the number of seconds until the next attack or target switch are set to zero, and Time.FixedDeltaTime is subtracted from each on every Fixed Update.
If the target switch timer reaches zero, it is reset back to its cooldown value, and the graph calls a targeting script from the Boss script based on the current number of target switches so far (tracked through an index variable). For example, in phase 1, the boss will loop between targeting the nearest and farthest player characters.
If the attack timer reaches zero, it is also reset back to its cooldown value. The boss then activates an attack ability based on the current number of attacks so far (also tracked through an index). In phase 1, the boss alternates between 3 attacks: basic projectile, basic melee, and the summoning of mages. In phase 2 and 3 the boss loops between 10 attacks. 
At the end of every FixedUpdate in phases 1 and 2, the state machine checks if the boss's health has dropped below a certain threshold. If so, a custom event is triggered to transition the boss into the next phase. 

The state machine is directly attached to the Boss gameobject and closely related to it. Most attacks and every target switch is achieved by calling methods in the Boss script. When the boss activates a summoning ability, the script machine triggers an event from the BossAdds graph, also attached to the boss, which interacts with the GameObject singleton to summon additional enemies.
By controlling the Boss's behavior, the machine also relates to the player and allied NPC (which are ghost replays of the player) systems. The boss's behavior forces the player to respond. When the boss launches an attack, the player will usually attempt to dodge it. When the boss switches targets, the player may wish to protect the new target and adjust their own strategy. The player and their past selves will take damage and die as a result of attacks activated by the boss state machine.

## Milestone 2 Devlog
1. The complicating gameplay feature I'm working on this milestone is the three pre-existing player classes (mage, tank, healer) as well as a new fourth class (rogue). Each class will have 2 abilities, be playable by the player, exist in ghost replay form as an NPC mimicking the player's past inputs, and exist as a zombie with 1 ability. The ghost replay & zombie form architecture already exists, so I will primarily be working on ability implementation. 
	1. Add a stationary AOE slow spell for the mage
		1. Create a trigger collider with a rigidbody2D with a script that detects when colliders enter/leave
		2. Check if each collision belongs to a living entity (not a projectile). 
		3. If a valid living entity enters/leaves, add/remove them to a list of GameObjects
		4. Every physicsupdate frame, set those gameobject's velocity additives (a Vector2 variable that is added to a living entity's velocity every frame) towards the center of the collider
		5. Assign the spell a lifespan, and have the spell sprite's transparency increase and scale decrease as its lifespan decreases. The spell is destroyed when its lifespan reaches 0. 
		6. Have the mage instantiate the AOE spell at a position by pressing RMB.
	
	2. Add a dash for the tank
		1. Create a coroutine which increases the Rogue's speed for X seconds
		2. Track the time remaining on a dash by awaiting for new physics frames to ensure that all dashes have the same duration
		3. Check the rogue's initial speed at the coroutine's start. After the timer is over, set it to that value.
		4. Trigger the coroutine by pressing RMB. 
	
	3. Add a summonable stationary turret for the healer
		1. Create a script which inherits from the "Ally" script (meaning that enemies will target it, and it has HP), attach it to a static capsule collider. Set its speed to zero.
		2. Have it periodically cast a circle of R radius around itself, checking all colliders in the circle for enemies 
		3. Sort through every enemy to find the closest
		4. Project a raycast from itself to that enemy, checking if any other living entities are in the way
		5. If nothing is in the way, have it instantiate a projectile gameobject facing that enemy
		6. Have healers instantiate the turret at mouse position with RMB.
	
	4. Add a rogue who fires projectiles that repel & attract. 
		1. Add a pentagon-shaped sprite with a rigidbody2d, collider, and the "Ally" script attached. Tune its HP, SPD, etc. 
		2. Create a rogue projectile script inheriting from the base projectile script. Tune its SPD, DMG, lifespan, etc. 
		3. When a rogue projectile hits a living target or wall, instead of destroying it, shut down its rigidbody and set itself as a child of the target. This way it becomes attached to the target. 
		4. Have a boolean determine the projectile's type. A pull hook attracts the rogue & projectile together, while a push hook repels them. 
		5. Add the pushing/pulling forces to the entity's velocity additives. Bosses and walls cannot be pushed/pulled.
		6. Have the rogue instantiate a pull hook with LMB and a push hook with RMB towards mouse position. 

2. The quiz question from week 5 did not help me build a feature for this milestone, as the feature it described (scriptable objects) was already complete by the end of milestone 1. The breakdowns also did not help, as I only read the instructions for milestone 2 after I had already finished adding all of the character classes. I think splitting steps into as many reasonable written substeps as possible is key for breakdowns; I mostly followed a few vague mental steps while creating the abilities/classes, and as a result I had to re-design the rogue class about half a dozen times. I will improve future breakdowns by actually writing them before programming. 

3. <img width="1897" height="1107" alt="Screenshot 2026-05-10 205910" src="https://github.com/user-attachments/assets/eb02d06a-b725-4847-9f1d-44d86bbefeaa" /> This is the BossAdds graph, attached to the Boss gameobject. It contains a series of custom events triggered by the Boss state machine graph that calls the SummonRisen() method from the GameController C# script, summoning zombie player characters of the given class at the given positions. Those positions are stored as Vector3 graph variables. Different triggers summon a different number of zombie NPCs at different positions. This graph serves the architectural purpose of keeping separate things separate, as I decided to build most of the Boss logic within graphs. Boss graphs largely interact with each other, calling methods from scripts when necessary. Using graphs also helps me visualize the number of summons each event trigger would cause. 

4. I implemented the ScriptableObject Unity System for feature 3. The player creates up to 100+ InputData.cs scriptable objects each life, each one representing a single player input. A list of those InputData objects is added to a CloneTemplate.cs scriptable object, representing a past player run in its entirety -- which can then be replayed in future runs. 



## Milestone 3 Devlog
1. <img width="952" height="536" alt="shadergraph" src="https://github.com/user-attachments/assets/b6a1b318-d4da-4f39-931c-20c67807e51b" /> The screenshot is of my Flooring shader, located in Art Assets/Materials. It is attached to a large rectangular floor sprite in the boss battle scene, and it applies a shifting marble / watery texture to the floor's solid color.
The shader is primarily built using a blend of simple noise and voronoise which add some texture to the floor. Voronoise creates noticeables lines in the floor while simple noise makes countless small adjustments to the darkness of the floor. I add sin(time) plus a constant to the scale of the simple noise and to the angle offset & cell density of the voronoise, slightly altering their noise textures. Because sin(time) constantly changes every frame, the noise patterns blended to form the texture change every frame. 
Also, because sin(time) oscillates between the same range of values, both of the noises are shifting in repeatable patterns, creating a sort of ebb-and-flow of moving water. I add a sample texture 2D using MainTex, which stores the sprite's default texture, to the blended noise to ensure that the floor sprite is not entirely overriden by the noise texture. 
To add a dash of extra color to the final texture, I've created a UV node modified by sin(time) such that it cycles between pure black and the standard red/green/yellow UV node texture. I blend the modified UV with the noise mixture to colorize the noise, and finally pass the result into the fragment color. 

2. Since no playtester has been able to defeat the boss, I've applied significant nerfs to it to make the combat mechanic easier. The boss's health has been nerfed by 80%, its guaranteed-hit AOE spell does less damage, and it is now 100% defeatable. The boss now advances through phases quicker, since its 2nd and especially 1st phases have quite limited abilities. While the central mechanic of past player lives repeating themselves works as intended, some playtesters noted that enemies would behave differently between attempts, always targeting the most recent player character and thus completely avoiding attacks by past player incarnations. This reduced players' willingness to rely on their past/future characters, so I have
altered the spawn mechanic to spawn players in vertical rows rather than horizontal columns. The boss, which begins by targeting the nearest player, now no longer targets the most recent player character but the oldest player character, meaning that past actions will now repeat themselves with greater accuracy. Playtesting also showed a significant drop-off in player character numbers during the boss's third phase due to an epidemic caused by its infectious projectiles (the large green ones). I built the new player class whose mechanics directly counteract the disease mechanic. The player can now build and launch walls which destroy the infectious projectiles on contact, and the infection now deals less damage. The boss's summoning mechanic now gradually summons enemies rather than instantly dropping them down, which caused random, unfair collision damage to players. Now the player sees enemies gradually fade in to view and have ample time to prepare. Finally, the tank's (square) dash now makes it ignore all enemy collisions, meaning the tank can escape even if surrounded on all sides.  

3. I've added a a main menu and a new level accessible by pressing "Waves" in the menu which pits the player against several dozen normal enemy NPCs rather than a single boss. The fifth player class, the architect (in the shape of an X) is here, and it can construct destructible walls and launch moving walls during battle. 
For visual effects, I added trails to every projectile, raindrop and splash particles to every scene, a scanlines shader graph, 2 floor shader graphs for the boss and wave-clear stages, and 2 character shader graphs affecting the Boss and all enemies. Now the player can do battle against 5 base enemy classes + the boss.




## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- [Homing Projectile](https://assetstore.unity.com/packages/3d/characters/humanoids/lowpoly-mushroomman-character-287820)
- [Fireball Projectile](https://bdragon1727.itch.io/fire-pixel-bullet-16x16)
- [Boss Healthbar Font](https://freefonts.co/fonts/fot-matisse-pro-eb)
- [UI Font](https://font.download/font/fixedsys-excelsior-301)
- [Music](https://rayhimmel.itch.io/b-dsm5)
- [Menu UI SFX](https://ambroggiomusic.itch.io/cute-interactions-sfx)
- [Character select UI SFX](https://ateliermagicae.itch.io/be-not-afraid-uimenu-sfx)
- [Boss, Cross, Hexagon, Circle Ability SFX](https://thesoundrack.itch.io/freemagicspellsfx)
- [Triangle Ability SFX](https://timothyadan.itch.io/magic-attacks-bundle)
- [Square Ability, Enemy SFX](https://slowdiger.itch.io/magic-and-monster-sfx)
