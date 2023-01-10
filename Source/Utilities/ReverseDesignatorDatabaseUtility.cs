using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    public static class ReverseDesignatorDatabaseUtility
    {
        public static bool TryGet<T>(this ReverseDesignatorDatabase database, out T value) where T : Designator
        {
            value = database.Get<T>();
            return value != null;
        }
    }
}
