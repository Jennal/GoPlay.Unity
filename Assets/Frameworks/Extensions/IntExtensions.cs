using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class IntExtensions
    {
        public static int Sign(this int val)
        {
            if (val == 0) return 0;
            if (val > 0) return 1;
            return -1;
        }

        public static bool InRange(this int value, int v1, int v2, bool boundaryValue = true)
        {
            var min = Mathf.Min(v1, v2);
            var max = Mathf.Max(v1, v2);
            if (boundaryValue)
            {
                return value <= max && value >= min;
            }
            return value < max && value > min;
        }
    }
}