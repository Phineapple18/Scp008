using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Log = LabApi.Features.Console.Logger;
using Object = UnityEngine.Object;

using Footprinting;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using static PlayerStatsSystem.Scp049DamageHandler;
using Scp008.Features;

namespace Scp008
{
    public class EventHandler : CustomEventsHandler
    {
        public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player.IsScp008())
            {
                ev.Player.TryCureOf008(100, false);
            }
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                Timing.CallDelayed(1f, () => ev.Player.SendHint(Translation.SpawnHint, 10f));
            }
        }

        public override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player.IsScp008() && ev.Player.TrySpawnAsZombie(ev.DamageHandler))
            {
                ev.IsAllowed = false;
            }
        }

        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.DamageHandler is AttackerDamageHandler adh && ev.Attacker.CanInfect() && ev.Player.TryInfectWith008(Config.InfectionChance) && Config.ZombieDamage >= 0)
            {    
                Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Player.Damage(new Scp049DamageHandler(new Footprint(ev.Attacker.ReferenceHub), Config.ZombieDamage, AttackType.Scp0492)));
                ev.Attacker.SendHitMarker();
                ev.IsAllowed = false;
            }
            if (ev.Player.IsScp008() && ev.DamageHandler.IsEffectDamage(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        public override void OnPlayerLeavingPocketDimension(PlayerLeavingPocketDimensionEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                ev.IsSuccessful = true;
                Log.Debug($"Player {ev.Player.Nickname} exited safely Pocket Dimension as SCP-0492.", Config.Debug);
            }
        }

        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            if (ev.Player != null && ev.Player.ReferenceHub.TryGetComponent(out Scp008Component component))
            {
                Object.Destroy(component);
                Log.Debug($"Destroyed a Scp008Component for player {ev.Player.Nickname}.", Config.Debug);
            }
        }

        public override void OnPlayerUsingItem(PlayerUsingItemEventArgs ev)
        {
            if (ev.Player.IsScp008() && ev.UsableItem.Type == ItemType.Scp021J && ev.IsAllowed)
            {
                ev.Player.TryCureOf008(100, false);
            }
        }

        public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
        {
            if (ev.Player.IsScp008() && (ev.UsableItem.Category == ItemCategory.Medical || ev.UsableItem.Type == ItemType.SCP500) && Config.CureItems.TryGetValue(ev.UsableItem.Type, out int chance))
            {
                ev.Player.TryCureOf008(chance);
            }
        }

        private Config Config => MainClass.Instance.pluginConfig;
        private Translation Translation => MainClass.Instance.pluginTranslation;
    }
}
