using GoPlay.Managers;
using GoPlay.Services;
using UnityEngine;
using GoPlay.Framework.Extensions;

namespace GoPlay.Framework.UI
{
    public class ScreenTouchEffect : MonoBehaviour
    {
        public GameObject EffectPrefab;

        private Camera _camera;
        public Camera Camera
        {
            get
            {
                if (!_camera) _camera = Camera.main;
                return _camera;
            }
        }

        public UnityEngine.Touch[] _cacheTouches;
        public void LateUpdate()
        {
            _cacheTouches = Input.touches;
            var touchCount = _cacheTouches.Length;
            for (var i = 0; i < touchCount; ++i)
            {
                var touch = _cacheTouches[i];
                if (touch.phase.Equals(TouchPhase.Ended))
                {
                    SpawnEffect(touch.position);
                }
            }

#if UNITY_EDITOR
            // Simulate in Editor
            if (Input.GetMouseButtonUp(0))
            {
                SpawnEffect(Input.mousePosition);
            }
#endif
        }

        private void SpawnEffect(Vector2 pos)
        {
            var go = PoolService.Instance.Spawn(EffectPrefab);
            go.GameObject.transform.SetParent(transform, false);
            go.GameObject.transform.position = Camera.ScreenToWorldPoint(pos).ToVector2();
        }
    }
}