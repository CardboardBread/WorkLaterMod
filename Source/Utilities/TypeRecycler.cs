using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardboardBread.WorkLater.Utilities
{
    // Utility for reusing frequently instantiated types, rather than frequently discarding and making new.
    // Make sure to reset any types to a default or zero state before recycling them.
    public static class TypeRecycler<T>
    {
        public static List<T> RecycleBin = new List<T>();

        public static T GetOrDefault()
        {
            if (RecycleBin.Count != 0)
            {
                var get = RecycleBin.Last();
                RecycleBin.RemoveAt(RecycleBin.Count - 1);
                return get;
            }
            else
            {
                return default;
            }
        }

        public static T GetOrCallback(Func<T> callback)
        {
            if (RecycleBin.Any())
            {
                var get = RecycleBin.Last();
                RecycleBin.RemoveAt(RecycleBin.Count - 1);
                return get;
            }
            else
            {
                return callback();
            }
        }

        public static void Yield(T value) => RecycleBin.Add(value);
    }
}
