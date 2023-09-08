#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GoPlay.Cameras
{
    public class BindToWorldObject : MonoBehaviour
    {
#if ODIN_INSPECTOR_3
        [BoxGroup("Bindings")]
#endif
        public Transform Target;

        private Camera _mainCamera;
        private Camera _uiCamera;
        
        private void Start()
        {
            _mainCamera = Camera.main;
            _uiCamera = UICamera.Instance;
        }

        private void Update()
        {
            if (!_mainCamera && !_uiCamera) return;
            
            var screenPos = _mainCamera.WorldToScreenPoint(Target.position);
            var worldPos = _uiCamera.ScreenToWorldPoint(screenPos);
            transform.position = worldPos;
        }
    }
}