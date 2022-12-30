using CardboardBread.WorkLater.Utilities;
using HugsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public class ResumableDesignation_SlaughterWhenGrown : ResumableDesignation
    {
        public override void DesignationCallback()
        {
            base.DesignationCallback();
            if (pawn.Discarded || pawn.Destroyed)
            {
                // Thing is unavailable, do nothing.
                //Log.Message($"Scheduled ({pawn}) is unavailable at the time of callback, cancelling designation.");
                RemoveSelf();
                return;
            }

            if (!pawn.Destroyed && !pawn.Discarded && !pawn.Spawned) // (mapIndexOrState == -1) Thing is unspawned.
            {
                // Wait until spawned.
                //Log.Message($"Scheduled ({pawn}) is unspawned at time of callback, cancelling designation.");
                RemoveSelf();
                return;
            }

            if (pawn.Spawned)
            {
                if (pawn.ageTracker.Adult)
                {
                    //Log.Message($"({pawn}) is adult at time of callback, upgrading SlaughterWhenGrown designation to Slaughter designation.");
                    // Upgrade the SlaughterWhenGrown designation to Slaughter
                    // Use SlaughterFullyGrown for upgrade checks and easy upgrade.
                    var sidegrade = Find.ReverseDesignatorDatabase.Get<Designator_SlaughterFullyGrown>();
                    Assert.IsNotNull(sidegrade);

                    RemoveSelf();
                    sidegrade.TryDesignateThing(pawn);
                    //Log.Message($"Upgraded SlaughterWhenGrown designation to Slaughter designation on ({pawn}).");
                }
                else
                {
                    // Wait until grown again, plus a ~1 second delay.
                    waitTicks = pawn.GetTicksToAdulthood() + 60;
                    HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(callback, (int)waitTicks);
                    //Log.Message($"({pawn}) is not adult at time of callback, waiting additional {waitTicks} ticks.");
                }
            }
        }
    }
}
