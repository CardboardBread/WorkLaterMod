using System;
using CardboardBread.WorkLater.Utilities;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public class Designator_SlaughterWhenGrown : Designator
    {
        // Track every designated pawn within a Designator.DesignateMultiCell() call.
        public List<Pawn> JustDesignatedPawns = new List<Pawn>();

        public override int DraggableDimensions => 2;

        protected override DesignationDef Designation => WorkLaterDefOf.SlaughterWhenGrown;

        public DesignationManager DesManager => base.Map.designationManager;

        public Designator_SlaughterWhenGrown()
        {
            defaultLabel = "DesignatorSlaughterWhenGrown".Translate();
            defaultDesc = "DesignatorSlaughterWhenGrownDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/Slaughter");
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.Designate_Slaughter;
            //hotKey = KeyBindingDefOf.Misc7;
        }

        public override AcceptanceReport CanDesignateThing(Thing thing)
        {
            if (thing is Pawn pawn &&
                pawn.def.race.Animal &&
                pawn.Faction == Faction.OfPlayer &&
                !DesManager.HasDesignationOn(pawn, Designation) &&
                !pawn.InAggroMentalState)
            {
                //return !pawn.ageTracker.Adult // Allow only designating non-adult animals.
                return true; // Allow designating animals of any growth percent.
            }
            return false;
        }

        public override void DesignateThing(Thing thing)
        {
            if (!(thing is Pawn pawn)) throw new ArgumentException($"Pawn-specific designation given non-pawn Thing ({thing})");

            var forwardTicks = pawn.GetTicksToAdulthood();
#if DEBUG
            Log.Message($"Designating ({thing}) to be slaughtered in {forwardTicks} ticks.");
#endif

            // Add designation and track designated thing.
            DesManager.AddDesignation(new Designation_SlaughterWhenGrown(thing, Designation, tickDelay: forwardTicks));
            JustDesignatedPawns.Add(pawn);

            // Remove ReleaseAnimalToWild or vanilla Slaughter designations if present.
            DesManager.TryRemoveDesignationOn(thing, DesignationDefOf.ReleaseAnimalToWild);
            DesManager.TryRemoveDesignationOn(thing, DesignationDefOf.Slaughter);
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 cell)
        {
            // Make sure target cell is inside map.
            if (!cell.InBounds(base.Map))
            {
                return false;
            }

            // Make sure the target cell has any slaughterable things.
            if (!SlaughterablesInCell(cell).Any())
            {
                return "MessageMustDesignateSlaughterable".Translate();
            }

            return true;
        }

        public override void DesignateSingleCell(IntVec3 cell)
        {
            foreach (var pawn in SlaughterablesInCell(cell))
            {
                DesignateThing(pawn);
            }
        }

        public IEnumerable<Pawn> SlaughterablesInCell(IntVec3 cell)
        {
            // Don't designate anything hidden. Do this here so clicking on fogged tiles doesn't notify the player.
            if (cell.Fogged(base.Map))
            {
                yield break;
            }

            foreach (var thing in cell.GetThingList(base.Map))
            {
                if (thing is Pawn pawn && CanDesignateThing(thing).Accepted)
                {
                    yield return pawn;
                }
            }
        }

        protected override void FinalizeDesignationSucceeded()
        {
            // Do checks on any correctly-designated targets.
            base.FinalizeDesignationSucceeded();
            foreach (var pawn in JustDesignatedPawns)
            {
                ShowDesignationWarnings(pawn);
            }
            JustDesignatedPawns.Clear();
        }

        public void ShowDesignationWarnings(Pawn pawn)
        {
            SlaughterDesignatorUtility.CheckWarnAboutBondedAnimal(pawn);
            SlaughterDesignatorUtility.CheckWarnAboutVeneratedAnimal(pawn);
        }

    }
}
