using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public static class DesignationManagerUtility
    {
        public static bool HasDesignationOn(this DesignationManager manager, Thing thing, DesignationDef def)
        {
            return manager.DesignationOn(thing, def) != null;
        }

        public static bool HasDesignationAt(this DesignationManager manager, IntVec3 cell, DesignationDef def)
        {
            return manager.DesignationAt(cell, def) != null;
        }

        public static bool TryGetDesignationOn(this DesignationManager manager, Thing thing, DesignationDef def, out Designation designation)
        {
            designation = manager.DesignationOn(thing);
            return designation != null;
        }

        public static bool TryGetDesignationAt(this DesignationManager manager, IntVec3 cell, DesignationDef def, out Designation designation)
        {
            designation = manager.DesignationAt(cell, def);
            return designation != null;
        }
    }
}
