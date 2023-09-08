using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class ContactPoint2DExtensions
    {
        public static Vector2 GetCenter(this IEnumerable<ContactPoint2D> points)
        {
            if (points == null || !points.Any()) return Vector2.zero;
            
            var count = points.Count();
            var sum = Vector2.zero;
            foreach (var point in points)
            {
                sum += point.point;
            }

            return sum / count;
        }
    }
}