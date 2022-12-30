using System;
using System.Collections.Generic;
using System.Linq;

namespace CardboardBread.WorkLater.Utilities
{
    // Extension of TypeRecycler to work with nullable types (class types).
    public static class ClassRecycler<T> where T : class
    {
        public static List<T> RecycleBin => TypeRecycler<T>.RecycleBin;

        public static T GetOrDefault() => TypeRecycler<T>.GetOrDefault();

        public static T GetOrCallback(Func<T> callback) => TypeRecycler<T>.GetOrCallback(callback);

        public static T GetOrNull()
        {
            if (RecycleBin.Any())
            {
                var get = RecycleBin.Last();
                RecycleBin.RemoveAt(RecycleBin.Count - 1);
                return get;
            }
            else
            {
                return null;
            }
        }

        public static void Yield(T value) => TypeRecycler<T>.Yield(value);
    }
}
