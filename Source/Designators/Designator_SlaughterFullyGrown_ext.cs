using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    // Copy of Designator_SlaughterFullyGrown but extending Designator_Slaughter to simplify implementation.
    public class Designator_SlaughterFullyGrown_ext : Designator_Slaughter
    {
        public Designator_SlaughterFullyGrown_ext()
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
            if (base.CanDesignateThing(thing) &&
                thing is Pawn pawn &&
                pawn.ageTracker.Adult) // If targeted animal is fully grown.
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
