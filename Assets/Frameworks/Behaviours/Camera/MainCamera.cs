using UnityEngine;

namespace GoPlay.Cameras
{
    public class MainCamera : MonoBehaviour
    {
        private static Camera _instance;
        public static Camera Instance => _instance;

        private void Awake()
        {
            _instance = GetComponent<Camera>();
        }
    }
}