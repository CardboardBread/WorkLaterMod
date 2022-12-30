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
        public static List<Type> AllDesignatorTypes => typeof(Designator).AllSubclassesNonAbstract();

        //public static List<Designator> AllDesignators = new List<Designator>();

        static DesignatorUtility()
        {
        }

        public static bool TryDesignateThing(this Designator designator, Thing thing)
        {
            var can = designator.CanDesignateThing(thing);
            if (can)
            {
                designator.DesignateThing(thing);
            }
            return can;
        }

        public static bool TryDesignateThing(this Designator designator, Thing thing, DesignationDef def, out Designation designation)
        {
            var can = TryDesignateThing(designator, thing);
            designation = thing.Map.designationManager.DesignationOn(thing, def); // Leaks existing designation (which is still a failure to designate).
            return can;
        }
    }
}
