using System.Linq;
using UnityEngine;
using GoPlay.Framework.Extensions;

namespace GoPlay.Utils
{
    public static class RectTransformExtension
    {
        public static Bounds GetWorldBounds(this RectTransform rt)
        {
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            var center = new Vector3(corners.Sum(o => o.x) / 4f, corners.Sum(o => o.y) / 4f);
            var size = new Vector3(corners.Max(o => o.x) - corners.Min(o => o.x), corners.Max(o => o.y) - corners.Min(o => o.y));
            var bounds = new Bounds(center, size);

            return bounds;
        }

        public static Vector3 GetWorldCenter(this RectTransform rt)
        {
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            var center = new Vector3(corners.Sum(o => o.x) / 4f, corners.Sum(o => o.y) / 4f);
            return center;
        }
        
        public static bool IsOverlap(this RectTransform rt, RectTransform other)
        {
            if (!rt || !other) return false;
            
            return rt.GetWorldBounds().Intersects(other.GetWorldBounds());
        }
        
        /// <summary>
        /// L => Left
        /// B => Bottom
        /// T => Top
        /// R => Right
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="parent"></param>
        /// <returns>获得当前节点左下角相对于目标父级左下角的偏移量</returns>
        public static Vector2 GetLBFromParentLB(this RectTransform rt, RectTransform parent = null)
        {
            if (!parent) parent = rt.parent as RectTransform;

            var delta = Vector2.zero;
            var halfParentRect = Vector2.zero;
            var node = rt;
            while (node && parent != node.parent)
            {
                var nodeParent = node = node.parent as RectTransform;
                if (!nodeParent) break;
                
                halfParentRect = nodeParent.rect.size * 0.5f;
                delta += halfParentRect + node.localPosition.ToVector2() + node.rect.position;
                node = nodeParent;
            }

            halfParentRect = parent.rect.size * 0.5f;
            return halfParentRect + rt.localPosition.ToVector2() + rt.rect.position + delta;
        }

        public static void SetLBFromParentLB(this RectTransform rt, Vector2 pos, RectTransform parent = null)
        {
            if (!parent) parent = rt.parent as RectTransform;

            var node = rt;
            var halfParentRect = Vector2.zero;

            while (node && parent != node)
            {
                halfParentRect += (node.parent as RectTransform).rect.size * 0.5f + node.rect.position;
                node = node.parent as RectTransform;
            }

            rt.localPosition = pos - halfParentRect;
        }
    }
}