# Scp008 (2.2.0)
Plugin for the "SCP: Secret Laboratory" game, that allows Zombies (SCP-049-2) to infect human players with SCP-008, causing them to gradually lose health. Infected players can have effects applied to them, cure the infection or turn into Zombies upon death.

## Features
- Zombies have a configurable chance to infect human players with SCP-008, dependent on Zombies count.
- Infected player can:
  * have effects applied to them
  * have hints displayed after their health drops below configurable thresholds
  * be hurt by players from the same faction after their health drops below configurable threshold (only on servers with Friendly Fire disabled)
  * become a Zombie upon death, depending on the death cause.
- If infected player dies from SCP-049 attack and Scp049 is listed as a death reason in the config, it will count as a revival.

*<ins>Christmas Event Only</ins>*
- Flamingos (SCP-1507) can optionally be infected (except Alpha Flamingo) with SCP-008.
- Zombie Flamingos (SCP-1507-049) can optionally infect others with SCP-008.
- Flamingos can't hurt infected flamingo.
- Infected Flamingos don't have any hints displayed to them.
- Drinking SCP-021J cures of SCP-008.

## Required plugins and dependencies
- [Harmony 2.2.2.0 (net48)](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0) by pardeike - dependency

## Installation
Place *Scp008* dll in "...\AppData\Roaming\SCP Secret Laboratory\LabAPI-Beta\plugins\global OR port_number".<br/>
Place the *Harmony* dll in "...\AppData\Roaming\SCP Secret Laboratory\LabAPI-Beta\dependencies\global OR port_number".

## Config
|Name|Type|Default value|Description|
|---|---|---|---|
|debug|bool|false|Should debug be enabled?|
|infection_chance|Dictionary<int,int>|1: 30<br/>5: 50|Infection chance per Zombie attack dependent on Zombie count. The largest chance less than or equal to the zombie number will be used.|
|infection_damage|float|5f|Damage per infection tick.|
|infection_interval|float|5f|Interval between infection ticks.|
|infect_if_no049|bool|false|Can zombies infect only, if there is no SCP-049?|
|zombie_damage|float|5f|Damage of a Zombie attack leading to infection. Set below 0 to leave unchanged.|
|scp008_effects*|Dictionary\<string, float>|Bleeding:<br/>-health: 90<br/>&nbsp;&nbsp;intensity: 1<br/>Blindness:<br/>-health: 60<br/>&nbsp;&nbsp;intensity: 10<br/>-health: 30<br/>&nbsp;&nbsp;intensity: 20|Effects and their intensity, that will be enabled once player health drops below certain values.|
|cure_items|Dictionary\<ItemType, int>|SCP500: 100<br/> Medkit: 50|Items, that can cure the infection and their cure chance. Set between 0-100.|
|friendly_fire_health|float|20f|Health threshold of infected player below which they can be attacked by their factionmates with no conseqences.|
|death_reasons**|List\<string>|-Infection<br/>-Scp0492<br/>-ZombieFlamingo|Death causes, that will turn an infected player into a Zombie upon death. Leave empty to disable.|
|can_flamingo_be_infected|bool|false|Can Flamingos (except Alpha Flamingo) be infected with SCP-008?|
|can_flamingo_infect|bool|false|Can Zombie Flamingos infect with SCP-008?|

**Allowed effects:*
- AmnesiaItems
- Bleeding
- Blindness (won't work if player wears SCP-1344 or has severed eyes)
- Blurriness
- Burned
- CardiacArrest
- Concussed
- Deafened
- Disabled
- Exhausted
- FogControl
- HeavyFooted
- Hemorrhage
- Lightweight
- MovementBoost
- NightVision
- Poisoned
- Slowness

**Possible death causes, leading to infection:*
- Infection
- Scp0492
- Scp049
- ZombieFlamingo
- Explosion
- MicroHID
- Any (except warhead and disruptor on disintegration mode)

## Translation
The translation file is in the same folder as the config file and allows you to customize information shown to players, such as hints and command names, aliases, descriptons and responses.

*IMPORTANT:* If you translate command names and/or aliases (except subcommands), make sure not to duplicate them.

## Remote Admin commands
1. scp008 - allows managing infection. Subcommands: cure, list, infect

## Permissions
- 008.infection - allows a player to use *cure* and *infect* commands
- 008.list - allows a player to use *list* command
