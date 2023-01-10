using CardboardBread.WorkLater.Utilities;
using HugsLib;
using RimWorld;
using System;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public class ResumableDesignation : Designation, IExposable
    {
        public ResumableTickDelay TickDelay;
        public Action Callback; // public field so any modifications can be made.

        // Instantiation from ExposeData()
        public ResumableDesignation() : base()
        {
        }

        // Instantiation from a designator.
        public ResumableDesignation(LocalTargetInfo target, DesignationDef def, ColorDef colorDef = null, long tickDelay = 0) : base(target, def, colorDef)
        {
            this.TickDelay = tickDelay;
            this.Callback = DesignationCallback;
        }

        public new void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TickDelay, nameof(TickDelay));

            // Remake tick delay after loading.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                StartSchedule();
            }
        }

        public new void Notify_Added()
        {
            base.Notify_Added();
            StartSchedule();
        }

        public void Notify_Removing()
        {
            StopSchedule();
        }

        public new void Delete()
        {
            base.Delete();
            StopSchedule();
        }

        public void StartSchedule() => HugsLibController.Instance.TickDelayScheduler.ScheduleCallback(Callback, (int)TickDelay);
        public void StopSchedule() => HugsLibController.Instance.TickDelayScheduler.TryUnscheduleCallback(Callback);

        public virtual void DesignationCallback()
        {
        }
    }
}
