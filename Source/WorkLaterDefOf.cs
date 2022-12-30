using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater
{
    [DefOf]
    public static class WorkLaterDefOf
    {
        static WorkLaterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(WorkLaterDefOf));
        }

        public static DesignationDef SlaughterWhenGrown;

        public static DesignationDef SlaughterFullyGrown;
    }
}
