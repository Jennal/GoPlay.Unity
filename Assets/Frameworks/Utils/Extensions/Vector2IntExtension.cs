using System.Collections.Generic;
using UnityEngine;

namespace GoPlay.Utils
{
    public static class Vector2IntExtension
    {
        public static Vector3Int ToVector3Int(this Vector2Int val)
        {
            return new Vector3Int(val.x, val.y, 0);
        }

       
    }
}