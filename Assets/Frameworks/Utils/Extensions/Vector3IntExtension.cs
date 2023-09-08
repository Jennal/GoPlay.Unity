using System.Collections.Generic;
using UnityEngine;

namespace GoPlay.Utils
{
    public static class Vector3IntExtension
    {
        public static Vector3 ToVector3(this Vector3Int val)
        {
            return new Vector3(val.x, val.y, val.z);
        }

        /// <summary>
        /// 获取曼哈顿距离内的点
        /// </summary>
        /// <param name="val"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static IEnumerable<Vector3Int> ListInManhattan(this Vector3Int val, int distance)
        {
            distance = Mathf.Abs(distance);
            for (var x = -distance; x <= distance; x++)
            {
                for (var y = -distance; y <= distance; y++)
                {
                    if (Mathf.Abs(x) + Mathf.Abs(y) > distance) continue;

                    yield return val + new Vector3Int(x, y, 0);
                }
            }
        }

        public static Vector3Int Left(this Vector3Int val, int count = 1)
        {
            return val + Vector3Int.left * count;
        }

        public static Vector3Int Right(this Vector3Int val, int count = 1)
        {
            return val + Vector3Int.right * count;
        }

        public static Vector3Int Up(this Vector3Int val, int count = 1)
        {
            return val + Vector3Int.up * count;
        }

        public static Vector3Int Down(this Vector3Int val, int count = 1)
        {
            return val + Vector3Int.down * count;
        }

        public static Vector2Int ToVector2Int(this Vector3Int val)
        {
            return new Vector2Int(val.x, val.y);
        }

        public static IEnumerable<Vector3Int> GetNeighbours8(this Vector3Int pos)
        {
            yield return pos + Vector3Int.left;
            yield return pos + Vector3Int.left + Vector3Int.up;
            yield return pos + Vector3Int.up;
            yield return pos + Vector3Int.up + Vector3Int.right;
            yield return pos + Vector3Int.right;
            yield return pos + Vector3Int.right + Vector3Int.down;
            yield return pos + Vector3Int.down;
            yield return pos + Vector3Int.down + Vector3Int.left;
        }

        public static IEnumerable<Vector3Int> GetNeighbours4(this Vector3Int pos)
        {
            yield return pos + Vector3Int.left;
            yield return pos + Vector3Int.up;
            yield return pos + Vector3Int.right;
            yield return pos + Vector3Int.down;
        }
    }
}