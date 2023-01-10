using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    [HarmonyPatch(typeof(Designation))]
    public static class Patch_Designator
    {
        // Link Designation.Notify_Removing() to a pretend override, because the base method is declared internal.
        [HarmonyPostfix]
        [HarmonyPatch("Notify_Removing")]
        public static void Notify_Removing_Postfix(Designation __instance)
        {
            if (__instance is ResumableDesignation resumable)
            {
                resumable.Notify_Removing();
            }
        }
    }
}
