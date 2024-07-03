using UnityEngine;

namespace GoPlay.Cameras
{
    public class UICamera : MonoBehaviour
    {
        private static Camera _instance;

        public static Camera Instance
        {
            get
            {
                if (!_instance)
                {
                    var uiLayer = 1 << LayerMask.NameToLayer("UI");
                    foreach (var camera in Camera.allCameras)
                    {
                        if ((camera.cullingMask & uiLayer) == uiLayer)
                        {
                            _instance = camera;
                            break;
                        }
                    }
                }

                return _instance;
            }
        }
    }
}