using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CardboardBread.WorkLater.Utilities
{
    public static class ThingUtility
    {
        public static long GetTicksToAdulthood(this Pawn pawn)
        {
            // The remaining percent of growth until adulthood.
            var remainingGrowth = 1f - pawn.ageTracker.Growth;

            // Native call for determining if a pawn is at adulthood, derived from it's minimum adult age (in ticks) and its current age (in ticks)
            var isAdult = pawn.ageTracker.Adult;

            if (remainingGrowth == 0f || isAdult)
            {
                return 0;
            }

            // The lowest number of ticks a pawn's age (in ticks) must be for it to be an adult.
            var adultTickLimit = pawn.ageTracker.AdultMinAgeTicks;

            // The number of ticks this pawn's age equals, not how many ticks this pawn has been in-game for.
            //var tickBioAge = pawn.ageTracker.AgeBiologicalTicks;

            // Idk yet.
            //var birthTick = pawn.ageTracker.BirthAbsTicks;

            // The ratio of game ticks to age ticks, for pawns that age slower/faster.
            //var BioTickRate = pawn.ageTracker.BiologicalTicksPerTick;
            
            // remaining growth as a percent of minimum age ticks.
            var remainingGrowthInTicks = adultTickLimit * remainingGrowth;

            return Mathf.FloorToInt(remainingGrowthInTicks);
        }
    }
}
