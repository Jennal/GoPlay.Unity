using System;
using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class StringExtensions
    {
        public static Vector2Int ToVector2Int(this string val)
        {
            var arr = val.Trim().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var x = int.Parse(arr[0].Trim());
            var y = int.Parse(arr[1].Trim()); 
            return new Vector2Int(x, y);
        }

        public static int ToInt32(this string val)
        {
            return int.Parse(val.Trim());
        }
    }
}