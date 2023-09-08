using System;
using System.Collections.Generic;
using System.Linq;

namespace GoPlay.Framework.Extensions
{
    public static class IListExtensions {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list) {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }

        public static void RingMove<T>(this IList<T> list, int offset)
        {
            if (list.Count <= 0) return;
            
            offset %= list.Count;
            if (offset == 0) return;
            
            var listClone = new List<T>(list);
            for (int i = 0; i < list.Count; i++)
            {
                var nextIdx = (i + offset) % list.Count;
                list[nextIdx] = listClone[i];
            }
        }

        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (range.Contains(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }

        public static T Shift<T>(this IList<T> list)
        {
            if (list.Count <= 0) return default;

            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static void Remove<T>(this IList<T> list, Func<T, bool> match)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}