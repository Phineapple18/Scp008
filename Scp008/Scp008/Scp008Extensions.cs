using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using static PlayerStatsSystem.Scp049DamageHandler;
using PluginAPI.Core;

#if CHRISTMAS
using PlayerRoles.PlayableScps.Scp1507;
#endif

namespace Scp008
{
    public static class Scp008Extensions
    {
        public static bool TryInfectWith008(this Player player, int chance)
        {
            if (!CanBeInfected(player))
            {
                Log.Debug($"Player {player.Nickname} can't be infected with Scp008.", Config.Debug, PluginName);
                return false;
            }
            if (randInt.Next(99) < chance)
            {
                try
                {
                    player.GetComponent<Scp008Component>().enabled = true;
                }
                catch (Exception)
                {
                    player.GameObject.AddComponent<Scp008Component>();
                }
                Log.Debug($"Player {player.Nickname} has been infected with Scp008.", Config.Debug, PluginName);
                return true;
            }
            Log.Debug($"Infection chance failed for player {player.Nickname}.", Config.Debug, PluginName);
            return false;
        }

        public static bool TryCureOf008(this Player player, int chance, bool showCureMessage = true)
        {
            if (!player.IsScp008())
            {
                Log.Debug($"Player {player.Nickname} is not infected with Scp008, therefore can't be cured.", Config.Debug, PluginName);
                return false;
            }
            if (randInt.Next(99) < chance)
            {
                player.GetComponent<Scp008Component>().enabled = false;
                if (player.IsHuman && showCureMessage)
                {
                    player.ReceiveHint(Translation.CuredMessage, 5);
                }
                Log.Debug($"Player {player.Nickname} has been cured of Scp008.", Config.Debug, PluginName);
                return true;
            }
            Log.Debug($"Cure chance failed for player {player.Nickname}.", Config.Debug, PluginName);
            return false;
        }

        internal static bool TrySpawnAs0492(this Player player, DamageHandlerBase damageHandler)
        {
            bool canSpawn = false;
            bool isRevival = false;
            player.TryCureOf008(100, false);
            switch (damageHandler)
            {
                case CustomReasonDamageHandler customHandler:
                    canSpawn = customHandler.ServerLogsText.Contains(Translation.InfectionDeathReason) && Config.DeathReasons.Contains("Infection");
                    goto default;
                case Scp049DamageHandler scp049Handler:
                    if (scp049Handler.Attacker.Hub != null)
                    {
                        isRevival = scp049Handler.DamageSubType != AttackType.Scp0492 && Config.DeathReasons.Contains("Scp049");
                        canSpawn = scp049Handler.DamageSubType == AttackType.Scp0492 && Config.DeathReasons.Contains("Scp0492") || isRevival;
                    }
                    goto default;
#if CHRISTMAS
                case Scp1507DamageHandler scp1507Handler:
                    canSpawn = scp1507Handler.Attacker.Role == RoleTypeId.ZombieFlamingo && Config.DeathReasons.Contains("Infection");
                    goto default;
#endif
                default:
                    canSpawn = canSpawn || Config.DeathReasons.Contains("Any");
                    break;
            }
            if (canSpawn)
            {
                string newRole = "SCP-049-2";
                RoleChangeReason changeReason = isRevival ? RoleChangeReason.Revived : RoleChangeReason.RemoteAdmin;
                if (player.IsHuman)
                {
                    Timing.CallDelayed(Timing.WaitForOneFrame, () => player.SetRole(RoleTypeId.Scp0492, changeReason));
                }
#if CHRISTMAS
                else
                {
                    newRole = "Zombie Flamingo";
                    Timing.CallDelayed(Timing.WaitForOneFrame, () => player.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.ZombieFlamingo, changeReason, RoleSpawnFlags.None));
                }
#endif
                RagdollManager.ServerSpawnRagdoll(player.ReferenceHub, damageHandler);
                Log.Debug($"Player {player.Nickname} has been turned into {newRole}.", Config.Debug, PluginName);
                return true;
            }
            return false;
        }

        internal static bool IsInfectionEffectDamage(Player player, DamageHandlerBase damageHandler)
        {
            List<string> effects = new();
            switch (damageHandler)
            {
                case Scp049DamageHandler scp049Handler:
                    effects.Add(scp049Handler.DamageSubType.ToString());
                    break;
                case Scp3114DamageHandler scp3114Handler:
                    effects.Add(scp3114Handler.Subtype.ToString());
                    break;
                case ScpDamageHandler scpHandler:
                    if (scpHandler.Damage == 2.1f)
                    {
                        effects.Add("Corroding");
                    }
                    break;
                case UniversalDamageHandler universalHandler:
                    effects = effectDeathTranslation.Keys.Where(e => effectDeathTranslation[e] == universalHandler.TranslationId).ToList();
                    break;
                default: break;
            }
            if (!effects.IsEmpty() && effects.Any(effectName => Config.Scp008Effects.ContainsKey(effectName) && player.EffectsManager.TryGetEffect(effectName, out StatusEffectBase effect) && effect.Intensity == intensity))
            {
                return true;
            }
            return false;
        }

        private static bool CanBeInfected(this Player player)
        {
            if (player.IsScp008() || player.TemporaryData.Contains("IsGhostSpectator"))
            {
                return false;
            }
            if (!player.IsHuman)
            {
#if CHRISTMAS
                if (player.Role == RoleTypeId.Flamingo && Config.CanFlamingoBeInfected)
                {
                    return true;
                }
#endif
                return false;
            }
            return true;
        }

        internal static bool CanInfect(this Player player)
        {
            if (player == null)
            {
                return false;
            }
            if (player.Role == RoleTypeId.Scp0492)
            {
                return true;
            }
#if CHRISTMAS
            if (player.Role == RoleTypeId.ZombieFlamingo && Config.CanFlamingoInfect)
            {
                return true;
            }
#endif
            return false;
        }

        public static bool IsScp008(this ReferenceHub hub)
        {
            return Player.Get(hub).IsScp008();
        }

        public static bool IsScp008(this Player player)
        {
            return player != null && player.TemporaryData.Contains(dataName);
        }

        public const string dataName = "IsScp008";

        public const byte intensity = 11;

        private static readonly Dictionary<string, byte> effectDeathTranslation = new()
        {
           { nameof(Asphyxiated), DeathTranslations.Asphyxiated.Id },
           { nameof(Bleeding), DeathTranslations.Bleeding.Id },
           { nameof(Decontaminating), DeathTranslations.Decontamination.Id },
           { nameof(Hemorrhage), DeathTranslations.Bleeding.Id },
           { nameof(PocketCorroding), DeathTranslations.PocketDecay.Id },
           { nameof(Poisoned), DeathTranslations.Poisoned.Id},
           { nameof(Scp207), DeathTranslations.Scp207.Id },
           { nameof(SeveredHands), DeathTranslations.SeveredHands.Id }
        };

        private static readonly Random randInt = new();

        public static IEnumerable<Player> List => Player.GetPlayers().Where(p => p.IsScp008());
        private static Config Config => Plugin.Singleton.pluginConfig;
        private static Translation Translation => Plugin.Singleton.pluginTranslation;
        private static string PluginName => Plugin.Singleton.pluginHandler.PluginName;
    }
}
