using UnityEngine;

namespace GoPlay.Utils
{
    public static class CanvasExtension
    {
        public static Camera GetCamera(this Canvas canvas)
        {
            var camera = Camera.current;
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) camera = canvas.worldCamera;
            if (!camera) camera = Camera.current ? Camera.current : Camera.main;

            return camera;
        }
    }
}