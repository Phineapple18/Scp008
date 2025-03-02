using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Footprinting;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using Log = LabApi.Features.Console.Logger;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp106;
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
        }

        public override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player.IsScp008() && ev.Player.TrySpawnAs0492(ev.DamageHandler))
            {
                ev.IsAllowed = false;
            }
        }

        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.DamageHandler is AttackerDamageHandler adh && ev.Player.CanInfect() && ev.Target.TryInfectWith008(config.InfectionChance) && config.ZombieDamage >= 0)
            {    
                Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Target.Damage(new Scp049DamageHandler(new Footprint(ev.Player.ReferenceHub), config.ZombieDamage, AttackType.Scp0492)));
                Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1f);
                ev.IsAllowed = false;
                return;
            }
            if (ev.Target.IsScp008() && ev.DamageHandler.IsEffectDamage(ev.Target))
            {
                ev.IsAllowed = false;
            }
        }

        public override void OnPlayerLeavingPocketDimension(PlayerLeavingPocketDimensionEventArgs ev)
        {
            if (!ev.IsSuccessful && ev.Player.Role == RoleTypeId.Scp0492)
            {
                ev.Player.Position = Scp106PocketExitFinder.GetBestExitPosition(ev.Player.RoleBase as IFpcRole);
                Log.Debug($"Player {ev.Player.Nickname} exited safely Pocket Dimension as {ev.Player.Role}.", config.Debug);
                ev.IsAllowed = false;
            }
        }

        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            if (ev.Player != null && ev.Player.ReferenceHub.TryGetComponent(out Scp008Component component))
            {
                UnityEngine.Object.Destroy(component);
                Log.Debug($"Destroyed a Scp008Component for player {ev.Player.Nickname}.", config.Debug);
            }
        }

        public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                Timing.CallDelayed(1f, delegate ()
                {
                    ev.Player.SendHint(translation.SpawnHint, 10f);
                });
            }
        }

        public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
        {
            if ((ev.Item.Category == ItemCategory.Medical || ev.Item.Type == ItemType.SCP500) && config.CureItems.TryGetValue(ev.Item.Type, out int chance) && ev.Player.IsScp008())
            {
                ev.Player.TryCureOf008(chance);
            }
        }

        private readonly Config config = MainClass.Instance.pluginConfig;
        private readonly Translation translation = MainClass.Instance.pluginTranslation;
    }
}
