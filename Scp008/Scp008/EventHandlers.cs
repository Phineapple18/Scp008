using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Footprinting;
using Hints;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp106;
using PlayerStatsSystem;
using static PlayerStatsSystem.Scp049DamageHandler;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Scp008
{
    internal class EventHandlers
    {
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        internal void OnPlayerChangeRole(PlayerChangeRoleEvent ev)
        {
            if (ev.Player.IsScp008())
            {
                ev.Player.TryCureOf008(100, false);
            }
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        internal bool OnPlayerDamage(PlayerDamageEvent ev)
        {
            if (ev.DamageHandler is AttackerDamageHandler adh && ev.Player != null && ev.Player.Role == RoleTypeId.Scp0492)
            {
                if (ev.Target.TryInfectWith008(config.InfectionChance) && config.ZombieDamage >= 0)
                {
                    Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Target.Damage(new Scp049DamageHandler(new Footprint(ev.Player.ReferenceHub), config.ZombieDamage, AttackType.Scp0492)));
                    return false;
                }
            }
            if (ev.Target.IsScp008())
            {
                return !Scp008Extensions.IsInfectionEffectDamage(ev.Target, ev.DamageHandler);
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerDying)]
        internal bool OnPlayerDying(PlayerDyingEvent ev)
        {
            if (ev.Player.IsScp008())
            {
                return !ev.Player.TrySpawnAs0492(ev.DamageHandler);
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerExitPocketDimension)]
        internal bool OnPlayerExitPocketDimension(PlayerExitPocketDimensionEvent ev)
        {
            if (!ev.IsSuccessful && ev.Player.Role == RoleTypeId.Scp0492)
            {
                IFpcRole fpcRole = ev.Player.RoleBase as IFpcRole;
                ev.Player.Position = Scp106PocketExitFinder.GetBestExitPosition(fpcRole);
                Log.Debug($"Player {ev.Player.Nickname} exited safely Pocket Dimension as SCP-0492.", config.Debug, pluginName);
                return false;
            }
            return true;
        }

        [PluginEvent(ServerEventType.PlayerLeft)]
        internal void OnPlayerLeft(PlayerLeftEvent ev)
        {
            if (ev.Player.TryGetComponent<Scp008Component>(out Scp008Component scp008Component))
            {
                UnityEngine.Object.Destroy(scp008Component);
                Log.Debug($"Destroyed a Scp008Component for player {ev.Player.Nickname}.", config.Debug, pluginName);
            }
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        internal void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            if (ev.Role == RoleTypeId.Scp0492)
            {
                Timing.CallDelayed(1f, delegate()
                {
                    ev.Player.ReceiveHint(translation.SpawnHint, new[] {HintEffectPresets.FadeIn(0.01f), HintEffectPresets.FadeOut(0.15f, 0.85f)}, 14f);
                });
            }
        }

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        internal void OnPlayerUsedItem(PlayerUsedItemEvent ev)
        {
            if ((ev.Item.Category == ItemCategory.Medical || ev.Item.ItemTypeId == ItemType.SCP500) && config.CureItems.ContainsKey(ev.Item.ItemTypeId) && ev.Player.IsScp008())
            {
                ev.Player.TryCureOf008(config.CureItems[ev.Item.ItemTypeId]);
            }
        }

        private static readonly Config config = Plugin.Singleton.pluginConfig;

        private readonly Translation translation = Plugin.Singleton.pluginTranslation;

        private readonly string pluginName = Plugin.Singleton.pluginHandler.PluginName;
    }
}
