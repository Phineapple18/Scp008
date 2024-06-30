using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

namespace Scp008.Patches
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
}
