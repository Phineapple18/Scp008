# Scp008
Plugin for SCP-SL game, that allows SCPs-0492 to infect human players with SCP-008. Infected players gradually lose health. Depending on the config, infected players can have effects applied to them, heal the infection or become SCP-0492 upon death.

## Features
- SCPs-0492 have a chance to infect human players with SCP-008.
- Infected player can have effects applied and messages shown to them after dropping to certain health amount.
- Infected player have a chance to cure themselves of infectioncan using medical items or SCP-500.
- Infected player can be hurt by teammates, once they drop to set health amount. It applies only, if friendly fire is disabled on server.
- Infected player can become SCP-0492 upon death, depending on manner of death (from infection, SCP-049 attack, SCP-0492 attack or any death).

## Required plugins and dependencies (1.0.0)
- [NWAPIPermissionSystem](https://github.com/CedModV2/NWAPIPermissionSystem/releases/tag/0.0.6) by ced777ric - plugin
- [Harmony 2.2.2.0](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0) by pardeike - dependency

## Config
|Name|Type|Default value|Description|
|---|---|---|---|
|is_enabled|bool|true|Should plugin be enabled?|
|debug|bool|false|Should debug be enabled?|
|zombie_damage|float|5f|Damage, that a zombie attack will deal, if it leads to infection. Set below 0 to disable modifying infection damage.|
|infection_chance|int|50|Infection chance. Set between 0-100.|
|infection_damage|float|5f|Damage per infection tick.|
|infection_interval|float|5f|Interval between infection damage ticks.|
|scp008_effects|Dictionary\<string, float>|Bleeding, 90<br/> Concussed, 50<br/> Deafened, 20|Effects, that will be enabled, if player health drops below set values.|
|cure_items|Dictionary\<ItemType, int>|SCP500: 100<br/> Medkit: 50|Items, that can cure the infection and the cure chance. Set between 0-100.|
|ff_health_cutoff|float|20f|Amount of HP of infected player, below which they can be killed by a player from the same faction, if Firendly Fire is disabled.|
|turn_into_zombie|List\<string>|- Infection<br/>- Scp0492<br/>- Scp049<br/>- Any|Causes of death, that will lead an infected player to become SCP-0492 upon death. Leave empty to disable.|

## Translation
The translation file is in the same folder as config file and allows you to customize:
- messages shown to infected players
- whether or not the debug for command registering should be enabled
- commands, their aliases and descripton
- command responses

## Remote Admin commands
### scp008
Parent command. Type empty command for more information regarding subcommands. Subcommands:
- cure - Cure selected player(s) of Scp008. Separate entries with space. Usage: PlayerId/PlayerNickname/all
- list - Print list of all players infected with Scp008.
- infect - Infect selected player(s) with Scp008. Separate entries with space. Usage: PlayerId/PlayerNickname/all

## Permissions
- 008.infect - allows player to use *cure* and *infect* commands
- 008.list - allows player to use *list* command
