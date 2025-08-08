using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using CustomPlayerEffects;
using GhostSpectator.Features.Extensions;
using Log = LabApi.Features.Console.Logger;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp1507;
using PlayerStatsSystem;
using static PlayerStatsSystem.Scp049DamageHandler;
using Utils.NonAllocLINQ;

namespace Scp008.Features
{
    public static class Scp008Extensions
    {
        public static bool TryInfectWith008(this Player player, int chance)
        {
            bool canBeInfected = player.CanBeInfected();
            try
            {
                canBeInfected = canBeInfected && !Scp008Extensions.IsGhost(player);
            }
            catch (FileNotFoundException)
            {
                Log.Debug($"GhostSpectator not found, continuing.", Config.Debug);
            }
            if (!canBeInfected)
            {
                Log.Debug($"Player {player.Nickname} can't be infected with Scp008.", Config.Debug);
                return false;
            }
            if (randInt.Next(99) < chance)
            {
                try
                {
                    player.ReferenceHub.GetComponent<Scp008Component>().enabled = true;
                }
                catch (NullReferenceException)
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
            if (randInt.Next(99) < chance)
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
            bool configNull = Config.DeathReasons == null;
            player.TryCureOf008(100, false);
            switch (damageHandler)
            {
                case CustomReasonDamageHandler customHandler:
                    canSpawn = customHandler.DeathScreenText.Contains(Translation.InfectionDeathReason) && !configNull && Config.DeathReasons.Contains("Infection");
                    goto default;
                case DisruptorDamageHandler disruptorHandler:
                    if (disruptorHandler.Disintegrate)
                    {
                        return false;
                    }
                    goto default;
                case ExplosionDamageHandler:
                    canSpawn = !configNull && Config.DeathReasons.Contains("Explosion");
                    goto default;
                case MicroHidDamageHandler:
                    canSpawn = !configNull && Config.DeathReasons.Contains("MicroHID");
                    goto default;
                case Scp049DamageHandler scp049Handler:
                    if (scp049Handler.Attacker.Hub != null)
                    {
                        isRevival = scp049Handler.DamageSubType != AttackType.Scp0492 && !configNull && Config.DeathReasons.Contains("Scp049");
                        canSpawn = isRevival || scp049Handler.DamageSubType == AttackType.Scp0492 && !configNull && Config.DeathReasons.Contains("Scp0492");
                    }
                    goto default;
                case Scp1507DamageHandler scp1507Handler:
                    canSpawn = scp1507Handler.Attacker.Role == RoleTypeId.ZombieFlamingo && !configNull && Config.DeathReasons.Contains("ZombieFlamingo");
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
                    canSpawn = canSpawn || !configNull && Config.DeathReasons.Contains("Any");
                    break;
            }
            if (canSpawn)
            {
                RoleTypeId newRole = player.IsHuman ? RoleTypeId.Scp0492 : RoleTypeId.ZombieFlamingo;
                RoleChangeReason changeReason = isRevival ? RoleChangeReason.Revived : RoleChangeReason.RemoteAdmin;
                player.DropEverything();
                Timing.CallDelayed(0.2f, () => player.SetRole(newRole, changeReason, RoleSpawnFlags.None));
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
            if (damageHandler is UniversalDamageHandler udh && effectDamageType.TryGetValue(udh.TranslationId, out List<string> effects))
            {
                bool result = false;
                effects.ForEach<string>(effect =>
                {
                    if (Config.Scp008Effects.TryGetValue(effect, out List<EffectParameters> parameters) && player.ActiveEffects.Any(e => e.name == effect && e.Intensity > 0) && parameters.Any(p => player.Health < p.Health))
                    {
                        result = true;
                    }
                });
                return result;
            }           
            if (damageHandler is Scp049DamageHandler s049dh && s049dh.Attacker.Hub == null && s049dh.DamageSubType == AttackType.CardiacArrest)
            {
                return Config.Scp008Effects.TryGetValue(nameof(CardiacArrest), out List<EffectParameters> parameters) && parameters.Any(p => player.Health < p.Health);
            }
            return false;
        }

        private static bool CanBeInfected(this Player player)
        {
            return !player.IsScp008() && (player.IsHuman || player.Role == RoleTypeId.Flamingo && Config.CanFlamingoBeInfected);
        }

        private static bool IsGhost(Player player)
        {
            return player.IsGhost();
        }

        internal static bool CanInfect(this Player player)
        {
            return player != null && (player.Role == RoleTypeId.Scp0492 || player.Role == RoleTypeId.ZombieFlamingo && Config.CanFlamingoInfect) && !(Config.InfectIfNo049 && Player.ReadyList.Any(p => p.Role == RoleTypeId.Scp049));
        }

        public static bool IsScp008(this Player player)
        {
            return player.ReferenceHub.IsScp008();
        }

        public static bool IsScp008(this ReferenceHub hub)
        {
            return hub.TryGetComponent(out Scp008Component component) && component.isActiveAndEnabled;
        }

        private static readonly Random randInt = new();
        private static readonly Dictionary<byte, List<string>> effectDamageType = new()
        {
            { DeathTranslations.Bleeding.Id, new() { nameof(Bleeding), nameof(Hemorrhage) }},
            { DeathTranslations.Poisoned.Id, new() { nameof(Poisoned) }},
            { DeathTranslations.Scp207.Id, new() { nameof(Scp207) }}
        };

        public static IEnumerable<Player> List => Player.List.Where(p => p.IsScp008());
        private static Config Config => MainClass.Instance.pluginConfig;
        private static Translation Translation => MainClass.Instance.pluginTranslation;
    }
}
