using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using Scp008.Features;

namespace Scp008
{
    [HarmonyPatch("FriendlyFireHandler", "IsFriendlyFire")]
    internal class FriendlyFirePatch
    {
        internal static void Postfix(ReferenceHub damagedPlayer, ref bool __result)
        {
            if (__result && damagedPlayer.IsScp008())
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(AttackerDamageHandler), "ProcessDamage")]
    internal class ProcessDamagePatch
    {
        internal static bool Prefix(AttackerDamageHandler __instance, ReferenceHub ply)
        {
            return !(!Server.FriendlyFire && ply.IsScp008() && ply.GetFaction() == __instance.Attacker.Hub.GetFaction() && ply.playerStats.GetModule<HealthStat>().CurValue < MainClass.Instance.pluginConfig.FfHealthCutoff);
        }
    }
}
