#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GoPlay.Cameras
{
    public class CameraSizeFitter : MonoBehaviour
    {
        public Vector2 DesignResolution = new Vector2(1080, 1920);
        [Tooltip("控制生效的最大横宽比")]public float AdjustThreshold = 0.65f;
#if ODIN_INSPECTOR_3
        [Button(ButtonSizes.Large)]
#endif
        private void Start()
        {
            var camera = GetComponent<Camera>();
            if (!camera) return;
            var aspect = DesignResolution.x / DesignResolution.y;
            float UserAspect = (float)Screen.width / Screen.height;
            if( UserAspect > AdjustThreshold) return; 
            var scale = aspect / ((float)Screen.width / Screen.height);
            camera.orthographicSize = DesignResolution.y / 200f * scale;
        }
    }
}