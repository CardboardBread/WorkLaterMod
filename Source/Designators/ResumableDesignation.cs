using HugsLib;
using RimWorld;
using System;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public class ResumableDesignation : Designation, IExposable
    {
        public long initTick;
        public long waitTicks;
        public long finalTick;
        public Action callback;

        public Pawn pawn => target.Pawn;

        public void RemoveSelf() => pawn.Map.designationManager.TryRemoveDesignationOn(pawn, def);

        // Instantiation from ExposeData()
        public ResumableDesignation()
        {
        }

        // Instantiation from a designator.
        public ResumableDesignation(LocalTargetInfo target, DesignationDef def, long waitTicks, ColorDef colorDef = null) : base(target, def, colorDef)
        {
            this.waitTicks = waitTicks;
            Initialize();
        }

        public void Initialize(bool schedule = true)
        {
            initTick = Find.TickManager.TicksGame;
            finalTick = initTick + waitTicks;
            callback = () => DesignationCallback();
            if (schedule)
            {
                HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(callback, (int)waitTicks);
                //Log.Message($"Scheduled callback in {waitTicks} ticks");
            }
        }

        public void Cleanup(bool schedule = true)
        {
            // Set waitTicks to a value usable after save/load.
            var currentTick = Find.TickManager.TicksGame;
            var elapsedTicks = currentTick - initTick;
            waitTicks = finalTick - elapsedTicks;
            if (schedule)
            {
                HugsLibController.Instance.TickDelayScheduler.TryUnscheduleCallback(callback);
                //Log.Message("Cancelled existing callback");
            }
        }

        public new void ExposeData()
        {
            base.ExposeData();

            // Before saving, update waitTicks to the remaining ticks until final.
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Cleanup(false);
            }

            Scribe_Values.Look(ref waitTicks, nameof(waitTicks));

            // After loading waitTicks, set proper init and final.
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Initialize();
            }
        }

        public void Notify_Removing()
        {
            Cleanup();
        }

        public new void Delete()
        {
            base.Delete();
            Cleanup();
        }

        public virtual void DesignationCallback()
        {
        }
    }
}
