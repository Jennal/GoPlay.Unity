using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector3 ToVector3(this Vector2 val)
        {
            return new Vector3(val.x, val.y);
        }
        
        public static Vector2 Multiply1By1(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        
        /// <summary>
        /// called when vector2 is used as a MinMaxSlider in Odin，
        /// to calc span avoid divide zero error
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float CalcSliderSpan(this Vector2 val) {
            var delta = val.y - val.x;
            return Mathf.Approximately(delta, 0f) ? 0.01f : delta;
        }
        
        public static bool Approximately(this Vector2 self, Vector2 other) {
            return Mathf.Approximately(self.x, other.x)
                   && Mathf.Approximately(self.y, other.y);
        }
    }
}