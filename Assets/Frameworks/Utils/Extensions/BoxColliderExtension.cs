using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GoPlay.Utils
{
    public static class BoxColliderExtension
    {
        public static Rect GetRect(this BoxCollider2D box) {
            var bound = box.bounds;
            var bottomLeft  = bound.center + new Vector3(-bound.size.x / 2f, -bound.size.y / 2f);
            return new Rect(bottomLeft, bound.size);
        }
        
        public static Rect GetScreenRect(this BoxCollider2D box, Camera cam) {
            var bound = box.bounds;

            var sizeInWorld = bound.size;
            var bottomLeft  = bound.center + new Vector3(-sizeInWorld.x / 2f, -sizeInWorld.y / 2f);
//            var TopLeft     = bound.center + new Vector3(-sizeInWorld.x / 2f, sizeInWorld.y / 2f);
            var topRight    = bound.center + new Vector3(sizeInWorld.x / 2f, sizeInWorld.y / 2f);
//            var bottomRight = bound.center + new Vector3(sizeInWorld.x / 2f, -sizeInWorld.y / 2f);

            var screenBottomLeft = cam.WorldToScreenPoint(bottomLeft);
            var screenTopRight = cam.WorldToScreenPoint(topRight);
            
            return new Rect(screenBottomLeft, 
                new Vector2(Mathf.Abs(screenTopRight.x - screenBottomLeft.x), Mathf.Abs(screenTopRight.y - screenBottomLeft.y)));
        }

        public static void SetRect(this BoxCollider2D box, Rect worldRect) {
            box.size = worldRect.size;
            var goPos = box.transform.position;
            box.offset = worldRect.center - new Vector2(goPos.x, goPos.y);
        }
    }
}
