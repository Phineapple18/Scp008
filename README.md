# Scp008
A plugin for the "SCP: Secret Laboratory" game, that allows Zombies (SCP-049-2) to infect human players with SCP-008, causing their gradual loss of health. Depending on the config, infected players can have effects applied to them, heal the infection or become Zombies upon death.

## Features
- Zombies have a configurable chance to infect human players with SCP-008.
- Infected player can:
  * have effects applied to them
  * have messages shown to them after their health drops below configurable thresholds
  * be hurt by players from the same faction after their health drops below a configurable threshold (only on servers with Friendly Fire disabled)
  * become a Zombie upon death. Whether or not a player becomes a Zombie, depends on the reason(s) of death set in the config file (infection, SCP-049 attack, Zombie attack or any reason).
- If infected player dies from SCP-049 attack and Scp049 is listen in config "DeathReasons", their death will count as a revival.

## Required plugins and dependencies (1.1.0)
- [NWAPIPermissionSystem](https://github.com/CedModV2/NWAPIPermissionSystem/releases/tag/0.0.6) by ced777ric - plugin
- [Harmony 2.2.2.0](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0) by pardeike - dependency

## Installation
Place *Scp008* and *NWAPIPermissionSystem* dlls in "...\AppData\Roaming\SCP Secret Laboratory\PluginAPI\plugins\global OR port_number".

Place the *Harmony* dll (net48) in "...\AppData\Roaming\SCP Secret Laboratory\PluginAPI\plugins\global OR port_number\dependencies".

## Config
|Name|Type|Default value|Description|
|---|---|---|---|
|is_enabled|bool|true|Should the plugin be enabled?|
|debug|bool|false|Should debug be enabled?|
|infection_chance|int|50|Infection chance per Zombie attack. Set between 0-100.|
|infection_damage|float|5f|Damage per infection tick.|
|infection_interval|float|5f|Interval between infection damage ticks.|
|zombie_damage|float|5f|Damage of a Zombie attack leading to infection. Set below 0 to leave unchanged.|
|scp008_effects|Dictionary\<string, float>|Bleeding, 90<br/> Concussed, 50<br/> Deafened, 20|Effect(s), that will be enabled once player health drops below set values.|
|cure_items|Dictionary\<ItemType, int>|SCP500: 100<br/> Medkit: 50|Item(s), that can cure the infection and their cure chance. Set between 0-100.|
|ff_health_cutoff|float|20f|Health threshold of an infected player, below which they can be killed by a player from the same faction (only on servers with Firendly Fire disabled).|
|death_reasons|List\<string>|- Infection<br/>- Scp0492<br/>- Scp049<br/>- Any|Cause(s) of death, that will lead an infected player to become a Zombie upon death. Leave empty to disable.|

## Translation
The translation file is in the same folder as the config file and allows you to customize e.g:
- messages shown to infected players
- command names, their aliases and descriptons
- command responses

*IMPORTANT:* Make sure not to duplicate command names and/or aliases, if you translate them.

## Remote Admin commands
### scp008
Parent command. Type an empty command for more information. Subcommands:
- cure - Cure selected player(s) of Scp008. Separate entries with space. Usage: PlayerId/PlayerNickname/all
- list - Print list of all players infected with Scp008.
- infect - Infect selected player(s) with Scp008. Separate entries with space. Usage: PlayerId/PlayerNickname/all

## Permissions
- 008.infect - allows a player to use *cure* and *infect* commands
- 008.list - allows a player to use *list* command
