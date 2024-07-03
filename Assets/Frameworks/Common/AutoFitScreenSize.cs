using GoPlay.Cameras;
using UnityEngine;

namespace GoPlay.UI.Common
{
    public class AutoFitScreenSize : MonoBehaviour
    {
        public SpriteRenderer SpriteRenderer;
        
        public void Start()
        {
            var size = MainCamera.GetScreenSize();
            var spriteSize = SpriteRenderer.sprite.rect.size / SpriteRenderer.sprite.pixelsPerUnit;
            var scale = Mathf.Max(size.x / spriteSize.x, size.y / spriteSize.y);
            if (scale < 1f) return;
            
            transform.localScale = Vector3.one * scale;
        }
    }
}