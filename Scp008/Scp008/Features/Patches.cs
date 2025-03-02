using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PlayerRoles;
using PlayerStatsSystem;

namespace Scp008.Features
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
            return !(ply.IsScp008() && ply.GetFaction() == __instance.Attacker.Hub.GetFaction() && ply.playerStats.GetModule<HealthStat>().CurValue < MainClass.Instance.pluginConfig.FfHealthCutoff);
        }
    }
}
