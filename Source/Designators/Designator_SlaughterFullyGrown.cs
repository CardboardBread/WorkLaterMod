using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    // Similar to AllowTool's Harvest Fully Grown, places Slaughter markers on animals that are at maximum growth
    // (and therefore will give maximum butcher resources).
    // A SlaughterFullyGrown designation def is made for compatibility's sake, but is not used.
    public class Designator_SlaughterFullyGrown : Designator
    {
        // Track every designated pawn within a Designator.DesignateMultiCell() call.
        public List<Pawn> JustDesignatedPawns = new List<Pawn>();

        public override int DraggableDimensions => 2;

        protected override DesignationDef Designation => WorkLaterDefOf.SlaughterFullyGrown;

        public DesignationManager DesManager => base.Map.designationManager;

        public Designator_SlaughterFullyGrown()
        {
            defaultLabel = "DesignatorSlaughterFullyGrown".Translate();
            defaultDesc = "DesignatorSlaughterFullyGrownDesc".Translate();
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
                //!DesManager.HasDesignationOn(pawn, Designation) &&
                !DesManager.HasDesignationOn(pawn, DesignationDefOf.Slaughter) &&
                !pawn.InAggroMentalState &&
                pawn.ageTracker.Adult) // If targeted animal is fully grown.
            {
                return true;
            }
            return false;
        }

        public override void DesignateThing(Thing thing)
        {
            if (!(thing is Pawn pawn)) throw new ArgumentException($"Pawn-specific designation given non-pawn Thing ({thing})");

            // Add slaughter designation and track designated thing.
            DesManager.AddDesignation(new Designation(thing, DesignationDefOf.Slaughter));
            JustDesignatedPawns.Add(pawn);

            // Remove ReleaseAnimalToWild or SlaughterWhenGrown designations if present.
            DesManager.TryRemoveDesignationOn(thing, DesignationDefOf.ReleaseAnimalToWild);
            DesManager.TryRemoveDesignationOn(thing, WorkLaterDefOf.SlaughterWhenGrown);

            // Own designation should not be present.
            DesManager.TryRemoveDesignationOn(thing, Designation);
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 cell)
        {
            // Make sure target cell is inside map.
            if (!cell.InBounds(Map))
            {
                return "CellOutOfBounds".Translate();
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
            // Skip any cells hidden from the player. Do this here so clicking on fogged tiles doesn't notify the player.
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
