using System.Collections.Generic;

namespace GoPlay.Utils
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var temp = list[i];
                var randomIndex = UnityEngine.Random.Range(0, count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> range)
        {
            foreach (var t in range)
            {
                list.Remove(t);
            }
        }
    }
}