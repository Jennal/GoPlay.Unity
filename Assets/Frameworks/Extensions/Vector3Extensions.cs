using UnityEngine;

namespace GoPlay.Framework.Extensions
{
    public static class Vector3Extensions
    {
        public static bool Approximately(this Vector3 self, Vector3 other)
        {
            return Mathf.Approximately(self.x, other.x)
                   && Mathf.Approximately(self.y, other.y)
                   && Mathf.Approximately(self.z, other.z);
        }

        public static Color ToColor(this Vector3 self) {
            return new Color(self.x, self.y,self.z);
        }

        public static Vector2 ToVector2(this Vector3 self)
        {
            return new Vector2(self.x, self.y);
        }
        
        public static Vector3 Multiply1By1(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static bool IsSameDirection(this Vector3 a, Vector3 b)
        {
            if (!Mathf.Approximately(Mathf.Sign(a.x), Mathf.Sign(b.x))) return false;
            if (!Mathf.Approximately(Mathf.Sign(a.y), Mathf.Sign(b.y))) return false;
            if (!Mathf.Approximately(Mathf.Sign(a.z), Mathf.Sign(b.z))) return false;

            return true;
        }
    }
}