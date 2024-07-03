using System;
using GoPlay.Framework.Extensions;
using UnityEngine;

namespace GoPlay.Cameras
{
    public class MainCamera
    {
        private static Camera _instance;

        public static Camera Instance
        {
            get
            {
                if (!_instance) _instance = Camera.main;
                return _instance;
            }   
        }
        
        public static Vector3 ScreenToWorldPoint(Vector2 screenPos)
        {
            var worldPos = Instance.ScreenToWorldPoint(screenPos);
            return worldPos.ToVector2();
        }
        
        public static Vector2 GetScreenSize()
        {
            var size = new Vector2(Instance.pixelWidth, Instance.pixelHeight) / 100f;
            if (Instance.orthographic)
            {
                var orthographicSize = Instance.orthographicSize;
                // Debug.Log($"size = {size}, orthographic = {orthographicSize}");
                size.x *= (orthographicSize * 2f / size.y);
                size.y = orthographicSize * 2f;
            }
            else
            {
                throw new Exception("Not Supported yet!");
            }

            return size;
        }
    }
}