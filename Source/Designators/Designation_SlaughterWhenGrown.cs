using CardboardBread.WorkLater.Utilities;
using HugsLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public class Designation_SlaughterWhenGrown : ResumableDesignation
    {
        // Instantiation from ExposeData()
        public Designation_SlaughterWhenGrown() : base()
        {
        }

        // Instantiation from a designator.
        public Designation_SlaughterWhenGrown(LocalTargetInfo target, DesignationDef def, ColorDef colorDef = null, long tickDelay = 0) : base(target, def, colorDef, tickDelay)
        {
        }

        public override void DesignationCallback()
        {
            base.DesignationCallback();
            if (!target.HasThing || !(target.Thing is Pawn pawn))
            {
#if DEBUG
                Log.Error($"{this.GetType().Name} targeting non-pawn."); 
#endif
                Delete();
                return;
            }

            if (pawn.Discarded || pawn.Destroyed)
            {
                // Thing is unavailable, do nothing.
#if DEBUG
                Log.Message($"Scheduled ({pawn}) is unavailable at the time of callback, removing designation.");
#endif
                Delete();
                return;
            }

            if (!pawn.Destroyed && !pawn.Discarded && !pawn.Spawned) // (mapIndexOrState == -1)
            {
                // Thing is unspawned. TODO: Wait until spawned
#if DEBUG
                Log.Message($"Scheduled ({pawn}) is unspawned at time of callback, removing designation.");
#endif
                Delete();
                return;
            }

            if (pawn.Spawned)
            {
                if (pawn.ageTracker.Adult)
                {
                    // Replace the SlaughterWhenGrown designation with Slaughter.
#if DEBUG
                    Log.Message($"Scheduled ({pawn}) is adult at time of callback, replacing SlaughterWhenGrown designation with Slaughter designation."); 
#endif
                    // Use SlaughterFullyGrown for easy replacement.
                    var replacer = Find.ReverseDesignatorDatabase.Get<Designator_SlaughterFullyGrown>();
                    if (replacer == null)
                    {
                        Log.Error("ReverseDesignatorDatabase does not possess Designator_SlaughterFullyGrown");
                        return;
                    }

                    replacer.TryDesignateThing(pawn);
                    Delete();
                }
                else
                {
                    // Wait until grown again, plus a ~1 second delay.
#if DEBUG
                    Log.Message($"Scheduled ({pawn}) is not adult at time of callback, waiting additional {TickDelay} ticks."); 
#endif
                    TickDelay = pawn.GetTicksToAdulthood() + 60;
                    StartSchedule();
                }
            }
            else
            {
                Log.Error($"Scheduled ({pawn}) is in unknown state.");
                Delete();
                return;
            }
        }
    }
}
