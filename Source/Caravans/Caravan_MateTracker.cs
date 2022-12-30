using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CardboardBread.WorkLater.Caravans
{
    public class Caravan_MateTracker : IExposable
    {
        public Caravan caravan;

        public Caravan_MateTracker(Caravan caravan)
        {
            this.caravan = caravan;
        }

        public void MateTrackerTick()
        {
            TryMateCaravanAnimals();
        }

        public IEnumerable<Pawn> GetPotentialMatingAnimals()
        {
            return from pawn in caravan.PawnsListForReading.AsParallel()
                   where pawn.def.race.Animal
                   where !pawn.Sterile()
                   where pawn.Faction == Faction.OfPlayer
                   where pawn.gender != Gender.None
                   select pawn;
        }

        public bool IsAnimalPotentialMate(Pawn pawn)
        {
            return pawn.def.race.Animal
                && !pawn.Sterile()
                && pawn.Faction == Faction.OfPlayer
                && pawn.gender != Gender.None;
        }

        public bool IsAnimalCapableMate(Pawn pawn)
        {
            return !pawn.InMentalState
                && !pawn.Downed
                && !pawn.InCaravanBed()
                && !pawn.CarriedByCaravan()
                && pawn.CanCasuallyInteractNow(twoWayInteraction: true);
        }

        public bool IsAnimalPossiblePairing(Pawn male, Pawn female)
        {
            return male.gender == Gender.Male
                && female.gender == Gender.Female
                && !male.IsForbidden(female)
                && PawnUtility.FertileMateTarget(male, female);
        }

        public void TryMateCaravanAnimals()
        {
            var potential = GetPotentialMatingAnimals().ToList(); // pawn.RaceProps.mateMtbHours
            foreach (var male in potential)
            {
                if (IsAnimalCapableMate(male) && male.gender == Gender.Male)
                {
                    foreach (var female in potential)
                    {
                        if (IsAnimalCapableMate(female) && female.gender == Gender.Female)
                        {
                            if (!male.IsForbidden(female) && PawnUtility.FertileMateTarget(male, female))
                            {
                                PawnUtility.Mated(male, female);
                            }
                        }
                    }
                }
            }
        }

        public Pawn FindMateForAnimal(List<Pawn> pawns,Pawn male)
        {
            if (male == null || !IsAnimalCapableMate(male) && male.gender == Gender.Male) return null;

            foreach (var female in pawns)
            {
                if (IsAnimalCapableMate(female) && female.gender == Gender.Female)
                {

                }
            }
        }

        public bool TryMateAnimalPair(Pawn male, Pawn female)
        {
            if (!male.IsForbidden(female) && PawnUtility.FertileMateTarget(male, female))
            {
                PawnUtility.Mated(male, female);
                return true;
            }
            return false;1
        }

        void IExposable.ExposeData()
        {
        }
    }
}
