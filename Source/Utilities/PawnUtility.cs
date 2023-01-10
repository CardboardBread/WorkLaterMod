using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CardboardBread.WorkLater.Utilities
{
    public static class PawnUtility
    {
        public static long GetTicksToAdulthood(this Pawn pawn)
        {
            // The remaining percent of growth until adulthood.
            var remainingGrowth = 1f - pawn.ageTracker.Growth;

            // Native call for determining if a pawn is at adulthood, derived from it's minimum adult age (in ticks) and its current biological age (in ticks).
            var isAdult = pawn.ageTracker.Adult;

            // Short circuit for fully grown pawns.
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

            // Chop off fractions of a tick.
            return Mathf.CeilToInt(remainingGrowthInTicks);
        }

        #region Mating

        // Filters the given collection of pawns into a collection animals
        // that could mate without any changes required (taming,
        // un-sterilization, etc).
        public static IEnumerable<Pawn> GetPotentialMatingAnimals(this IEnumerable<Pawn> pawns)
        {
            return from pawn in pawns
                   where pawn.def.race.Animal
                   where pawn.gender != Gender.None
                   where !pawn.Sterile()
                   where pawn.Faction == Faction.OfPlayer
                   select pawn;
        }

        // Individual scope for GetPotentialMatingAnimals()
        public static bool IsPotentialMatingAnimal(this Pawn pawn)
        {
            return pawn.def.race.Animal
                && pawn.gender != Gender.None
                && !pawn.Sterile()
                && pawn.Faction == Faction.OfPlayer;
        }

        // Filters the given collection of pawns into a collection of animals
        // that are immediately capable of mating, such that JobGiver_Mate
        // would apply. (not whether the job giver would find a suitable mate)
        public static IEnumerable<Pawn> GetCapableMatingAnimals(this IEnumerable<Pawn> pawns, bool potentialMatingAnimals = false)
        {
            // Shortcut for calling GetPotentialMatingAnimals()
            var filtered = potentialMatingAnimals ? pawns : pawns.GetPotentialMatingAnimals();

            return from pawn in filtered
                   where !pawn.InMentalState
                   where !pawn.InCaravanBed()
                   where !pawn.CarriedByCaravan()
                   where !pawn.Downed
                   where pawn.Awake()
                   where pawn.CanCasuallyInteractNow(twoWayInteraction: true)
                   select pawn;
        }

        // Individual scope for GetCapableMatingAnimals()
        public static bool IsCapableMatingAnimal(this Pawn pawn)
        {
            return pawn.IsPotentialMatingAnimal()
                && !pawn.InMentalState
                && !pawn.InCaravanBed()
                && !pawn.CarriedByCaravan()
                && !pawn.Downed
                && pawn.Awake()
                && pawn.CanCasuallyInteractNow(twoWayInteraction: true);
        }

        // Potential mating but for a pair of animals
        public static bool IsPotentialMatingAnimalPair(Pawn first, Pawn second) // TODO: generalize to any 2 or more pawns?
        {
            return IsPotentialMatingAnimal(first)
                && IsPotentialMatingAnimal(second)
                && first.def == second.def
                && first.gender != second.gender;
        }

        // Capable mating but for a pair of animals.
        public static bool IsCapableMatingAnimalPair(Pawn first, Pawn second, bool potentialMatingAnimals = false)
        {
            return (potentialMatingAnimals ? true : IsPotentialMatingAnimalPair(first, second))
                   && IsCapableMatingAnimal(first)
                   && IsCapableMatingAnimal(second);
        }

        // If the given pair can mate right now (you can call PawnUtility.Mated() on them).
        public static bool IsImmediateMatingAnimalPair(Pawn first, Pawn second, bool potentialMatingAnimals = false, bool capableMatingAnimals = false)
        {
            return (potentialMatingAnimals ? true : IsPotentialMatingAnimalPair(first, second))
                   && (capableMatingAnimals ? true : IsCapableMatingAnimalPair(first, second))
                   && !first.IsForbidden(second)
                   && !second.IsForbidden(first) // TODO: verify if Thing.IsForbidden() reversal is necessary
                   && AnyFertileMateTarget(first, second);
        }

        public static IEnumerable<Pawn> GetImmediateMatingAnimals(this Pawn pawn, IEnumerable<Pawn> matingPool, bool potentialMatingAnimals = false, bool capableMatingAnimals = false)
        {
            foreach (var other in matingPool)
            {
                if (pawn != other && IsImmediateMatingAnimalPair(pawn, other, potentialMatingAnimals, capableMatingAnimals))
                {
                    yield return other;
                }
            }
        }

        // unordered version of PawnUtility.FertileMateTarget()
        public static bool AnyFertileMateTarget(Pawn first, Pawn second)
        {
            if (first.gender == second.gender)
            {
                return false;
            }

            if (first.gender == Gender.Male)
            {
                return RimWorld.PawnUtility.FertileMateTarget(first, second);
            }

            if (second.gender == Gender.Male)
            {
                return RimWorld.PawnUtility.FertileMateTarget(second, first);
            }

            return false;
        }

        // unordered version of PawnUtility.Mated()
        public static void AnyMated(Pawn first, Pawn second)
        {
            if (first.gender == second.gender)
            {
                return;
            }

            if (first.gender == Gender.Male)
            {
                RimWorld.PawnUtility.Mated(first, second);
            }

            if (second.gender == Gender.Male)
            {
                RimWorld.PawnUtility.Mated(second, first);
            }
        }

        #endregion

        #region IEnumerable Filtering

        public static IEnumerable<Pawn> AllColonists(this IEnumerable<Pawn> pawns)
        {
            return pawns.Where(pawn => pawn.IsColonist);
        }

        public static IEnumerable<Pawn> AllAnimals(this IEnumerable<Pawn> pawns)
        {
            return pawns.Where(pawn => pawn.RaceProps.Animal);
        }

        public static IEnumerable<Pawn> AllPrisoners(this IEnumerable<Pawn> pawns)
        {
            return pawns.Where(pawn => pawn.IsPrisoner);
        }

        public static IEnumerable<Pawn> AllMechanoids(this IEnumerable<Pawn> pawns)
        {
            return pawns.Where(pawn => pawn.RaceProps.IsMechanoid);
        }

        public static IEnumerable<Pawn> AllDowned(this IEnumerable<Pawn> pawns)
        {
            return pawns.Where(pawn => pawn.Downed && !pawn.ageTracker.CurLifeStage.alwaysDowned);
        }

        public static IEnumerable<Pawn> AllMentalState(this IEnumerable<Pawn> pawns)
        {
            return pawns.Where(pawn => pawn.InMentalState);
        }

        #endregion
    }
}
