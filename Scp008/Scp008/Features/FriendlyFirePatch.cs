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
    [HarmonyPatch(typeof(AttackerDamageHandler), "ProcessDamage")]
    internal class FriendlyFirePatch
    {
        internal static void Prefix(AttackerDamageHandler __instance, ReferenceHub ply)
        {
            if (ply.IsScp008() && ply.GetFaction() == __instance.Attacker.Hub.GetFaction() && ply.playerStats.GetModule<HealthStat>().CurValue < MainClass.Instance.pluginConfig.FriendlyfireHealth)
            {
                __instance.ForceFullFriendlyFire = true;
            }
        }
    }
}
