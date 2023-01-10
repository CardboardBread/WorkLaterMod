using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public static class DesignatorUtility
    {
        public static bool TryDesignateThing(this Designator designator, Thing thing)
        {
            var can = designator.CanDesignateThing(thing);
            if (can)
            {
                designator.DesignateThing(thing);
            }
            return can;
        }

        // TODO: TryDesignateThing but return new designation in out param.
    }
}
