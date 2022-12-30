using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardboardBread.WorkLater.Utilities
{
    public static class DictionaryUtility
    {
        public static V ComputeValueIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<V> absentFunc)
        {
            dict.TryComputeValueIfAbsent(key, absentFunc, out V value);
            return value;
        }

        public static bool TryComputeValueIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<V> absentFunc, out V value)
        {
            if (dict.ContainsKey(key))
            {
                value = dict[key];
                return true;
            }
            else
            {
                value = absentFunc();
                dict[key] = value;
                return false;
            }
        }

        // All of the below methods need `where V : class` since they all check if a map has a key and if the value for a given key is null.
        // This double check can be prevented by removing keys in place of mapping keys explicitly to null.
        public static V ComputeClassIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<V> absentFunc) where V : class
        {
            V value;
            if (dict.ContainsKey(key) && dict[key] != null)
            {
                value = dict[key];
            }
            else
            {
                value = absentFunc();
                dict[key] = value;
            }
            return value;
        }

        public static V ComputeClassIfAbsent<K, V>(this Dictionary<K, V> dict, K key, params Func<V>[] absentFuncs) where V : class
        {
            V value = null;
            if (dict.ContainsKey(key) && dict[key] != null)
            {
                value = dict[key];
            }
            else
            {
                foreach (var func in absentFuncs)
                {
                    value = func();
                    if (value != null)
                    {
                        break;
                    }
                }

                dict[key] = value;
            }
            return value;
        }

        public static bool TryComputeClassIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<V> absentFunc, out V value) where V : class
        {
            var test = dict.ContainsKey(key) && dict[key] != null;
            if (test)
            {
                value = dict[key];
            }
            else
            {
                value = absentFunc();
                dict[key] = value;
            }
            return test;
        }
    }
}
