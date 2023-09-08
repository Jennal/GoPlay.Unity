using UnityEngine;

namespace GoPlay.Utils
{
    public static class TransformExtensions
    {
        public static void SetPositionVector2(this Transform self, Vector2 val)
        {
            self.position = new Vector3(val.x, val.y, self.position.z);
        }
        
        public static void SetPositionVector2(this Transform self, Vector3 val)
        {
            self.position = new Vector3(val.x, val.y, self.position.z);
        }
        
        public static void RectTransformResizeFullScreen(this Transform transform)
        {
            var rt = transform as RectTransform;
            if (!rt) return;
            
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            
            rt.anchoredPosition3D = Vector3.zero;
            rt.localScale = Vector3.one;
        }
    }
}