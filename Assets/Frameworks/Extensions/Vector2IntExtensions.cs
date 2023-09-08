using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector3Int ToVector3Int(this Vector2Int val)
        {
            return new Vector3Int(val.x, val.y, 0);
        }
        
        public static Vector2Int AllDimensionOne(this Vector2Int val)
        {
            var result = new Vector2Int(
                val.x.Sign(),
                val.y.Sign()
            );
            return result;
        }
    }
}