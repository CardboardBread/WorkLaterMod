using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    [HarmonyPatch(typeof(Designator_Slaughter))]
    public static class Patch_Designator_Slaughter
    {
        // Ensure our custom slaughter designations are removed by the vanilla designator.
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Designator_Slaughter.DesignateThing))]
        public static void DesignateThing_Postfix(Designator_Slaughter __instance, Thing t)
        {
            //__instance.Map.designationManager.TryRemoveDesignationOn(t, WorkLaterDefOf.SlaughterFullyGrown);
            __instance.Map.designationManager.TryRemoveDesignationOn(t, WorkLaterDefOf.SlaughterWhenGrown);
        }
    }
}
