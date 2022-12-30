using HarmonyLib;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using Verse;

namespace CardboardBread.WorkLater.Caravans
{
    [HarmonyPatch(typeof(Caravan))]
    public static class Patch_Caravan
    {
        public static Dictionary<Caravan, Caravan_MateTracker> MateTrackers = new Dictionary<Caravan, Caravan_MateTracker>();
        
        // Create mate trackers alongside the other caravan tracker classes.
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor)]
        public static void ctor_Postfix(Caravan __instance)
        {
            MateTrackers[__instance] = new Caravan_MateTracker(__instance);
        }

        // Add saving the mate tracker in a Caravan's data.
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Caravan.ExposeData))]
        public static void ExposeData_Postfix(Caravan __instance)
        {
            Caravan_MateTracker container = null;

            // Grab mate tracker instance from static dict before saving.
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                container = MateTrackers[__instance];
            }

            // Save/Load mate tracker from/into container variable.
            Scribe_Deep.Look(ref container, "mateTracker", __instance);

            // Relay loaded mate tracker instance to static dict after loading.
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                MateTrackers[__instance] = container;
            }
        }

        // Add ticking the mate tracker to a Caravan's tick method.
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Caravan.Tick))]
        public static void Tick_Postfix(Caravan __instance)
        {
            var mateTracker = MateTrackers[__instance];
            Assert.IsNotNull(mateTracker);
            if (Find.TickManager.TicksGame % __instance.HashOffset() == 0) // Only try mating occasionally.
            {
                mateTracker.MateTrackerTick();
            }
        }
    }
}
