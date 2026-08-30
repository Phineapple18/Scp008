using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GhostSpectator.Features.Extensions;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp1507;
using PlayerStatsSystem;
using Utils.NonAllocLINQ;
using static PlayerStatsSystem.Scp049DamageHandler;
using Log = LabApi.Features.Console.Logger;

namespace Scp008.Features
{
    public static class Scp008Extensions
    {
        public static bool TryInfectWith008(this Player player, int chance)
        {
            if (!player.CanBeInfected())
            {
                Log.Debug($"Player {player.Nickname} can't be infected with Scp008.", Config.Debug);
                return false;
            }
            try
            {
                if (player.IsGhost())
                {
                    Log.Debug($"Player {player.Nickname} is a Ghost and can't be infected with Scp008.", Config.Debug);
                    return false;
                }
            }
            catch (FileNotFoundException)
            {
                Log.Debug("GhostSpectator not found, continuing.", Config.Debug);
            }
            if (random.Next(99) < chance)
            {
                if (player.ReferenceHub.TryGetComponent(out Scp008Component component))
                {
                    component.enabled = true;
                }
                else
                {
                    player.GameObject.AddComponent<Scp008Component>();
                }
                Log.Debug($"Player {player.Nickname} has been infected with Scp008.", Config.Debug);
                return true;
            }
            Log.Debug($"Infection chance failed for player {player.Nickname}.", Config.Debug);
            return false;
        }

        public static bool TryCureOf008(this Player player, int chance, bool showCureMessage = true)
        {
            if (!player.IsScp008())
            {
                Log.Debug($"Player {player.Nickname} is not infected with Scp008, therefore can't be cured.", Config.Debug);
                return false;
            }
            if (random.Next(99) < chance)
            {
                player.ReferenceHub.GetComponent<Scp008Component>().enabled = false;
                if (player.IsHuman && showCureMessage)
                {
                    player.SendHint(Translation.CuredMessage, 5);
                }
                Log.Debug($"Player {player.Nickname} has been cured of Scp008.", Config.Debug);
                return true;
            }
            Log.Debug($"Cure chance failed for player {player.Nickname}.", Config.Debug);
            return false;
        }

        internal static bool TrySpawnAsZombie(this Player player, DamageHandlerBase damageHandler)
        {
            bool canSpawn = false;
            bool isRevival = false;
            player.TryCureOf008(100, false);
            if (Config.DeathReasons != null)
            {
                switch (damageHandler)
                {
                    case CustomReasonDamageHandler customHandler:
                        canSpawn = customHandler.DeathScreenText.Contains(Translation.InfectionDeathReason) && Config.DeathReasons.Contains("Infection");
                        goto default;
                    case DisruptorDamageHandler disruptorHandler:
                        if (disruptorHandler.Disintegrate)
                        {
                            return false;
                        }
                        goto default;
                    case ExplosionDamageHandler:
                        canSpawn = Config.DeathReasons.Contains("Explosion");
                        goto default;
                    case MicroHidDamageHandler:
                        canSpawn = Config.DeathReasons.Contains("MicroHID");
                        goto default;
                    case Scp049DamageHandler scp049Handler:
                        if (scp049Handler.Attacker.Hub != null)
                        {
                            isRevival = scp049Handler.DamageSubType != AttackType.Scp0492 && Config.DeathReasons.Contains("Scp049");
                            canSpawn = isRevival || scp049Handler.DamageSubType == AttackType.Scp0492 && Config.DeathReasons.Contains("Scp0492");
                        }
                        goto default;
                    case Scp1507DamageHandler scp1507Handler:
                        canSpawn = scp1507Handler.Attacker.Role == RoleTypeId.ZombieFlamingo && Config.DeathReasons.Contains("ZombieFlamingo");
                        goto default;
                    case UniversalDamageHandler universalHandler:
                        if (universalHandler.TranslationId == DeathTranslations.Crushed.Id)
                        {
                            return false;
                        }
                        goto default;
                    case WarheadDamageHandler:
                        return false;
                    default:
                        canSpawn = canSpawn || Config.DeathReasons.Contains("Any");
                        break;
                }
            }
            if (canSpawn)
            {
                RoleTypeId newRole = player.IsHuman ? RoleTypeId.Scp0492 : RoleTypeId.ZombieFlamingo;
                RoleChangeReason changeReason = isRevival ? RoleChangeReason.Revived : RoleChangeReason.RemoteAdmin;
                player.DropEverything();
                player.SetRole(newRole, changeReason, RoleSpawnFlags.None);
                Log.Debug($"Player {player.Nickname} has been turned into {newRole}.", Config.Debug);
                return true;
            }
            return false;
        }

        internal static bool IsEffectDamage(this DamageHandlerBase damageHandler, Player player)
        {
            if (Config.Scp008Effects == null)
            {
                return false;
            }
            if (damageHandler is Scp049DamageHandler s049dh && s049dh.Attacker.Hub == null && s049dh.DamageSubType == AttackType.CardiacArrest)
            {
                return Config.Scp008Effects.TryGetValue("CardiacArrest", out List<EffectParameters> parameters) && parameters.Any(p => player.Health <= p.Health);
            }
            if (damageHandler is UniversalDamageHandler udh && damageEffects.TryGetValue(udh.TranslationId, out List<string> effects))
            {
                foreach (string effect in effects)
                {
                    if (Config.Scp008Effects.TryGetValue(effect, out List<EffectParameters> parameters) && player.ActiveEffects.Select(e => e.name).Contains(effect) && parameters.Any(p => player.Health <= p.Health))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CanBeInfected(this Player player)
        {
            return !player.IsScp008() && (player.IsHuman || player.Role == RoleTypeId.Flamingo && Config.FlamingoInfected);
        }

        private static bool IsGhost(this Player player)
        {
            return GhostExtensions.IsGhost(player);
        }

        internal static bool CanInfect(this Player player)
        {
            return player != null && (player.Role == RoleTypeId.Scp0492 || player.Role == RoleTypeId.ZombieFlamingo && Config.FlamingoInfect) && !(Config.InfectNo049Only && Player.ReadyList.Any(p => p.Role == RoleTypeId.Scp049));
        }

        public static bool IsScp008(this Player player)
        {
            return player?.ReferenceHub.IsScp008() ?? false;
        }

        public static bool IsScp008(this ReferenceHub hub)
        {
            return (hub?.TryGetComponent(out Scp008Component component) ?? false) && component.isActiveAndEnabled;
        }

        private static readonly Random random = new();
        private static readonly Dictionary<byte, List<string>> damageEffects = new()
        {
            { DeathTranslations.Bleeding.Id, new() { "Bleeding", "Hemorrhage" }},
            { DeathTranslations.Poisoned.Id, new() { "Poisoned" }}
        };

        public static IEnumerable<Player> List => Player.List.Where(p => p.IsScp008());
        private static Config Config => MainClass.Instance.pluginConfig;
        private static Translation Translation => MainClass.Instance.pluginTranslation;
    }
}
