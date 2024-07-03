using UnityEngine;
using UnityEngine.EventSystems;

namespace GoPlay.Touch.Gestures
{
    public class DragGestureRecognizer : GestureRecognizerBase, IPointerDownHandler, IDragHandler
    {
        public float holdTime = 0.4f;
        public float dragMinDistance = 5f;

        private bool _isDragStarted;
        private Vector2 _startPos;
        private float _startTime;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _startPos = eventData.position;
            _startTime = Time.unscaledTime;
            _isDragStarted = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragStarted)
            {
                var deltaTime = Time.unscaledTime - _startTime;
                if (deltaTime >= holdTime) return;

                var deltaPos = eventData.position - _startPos;
                if (deltaPos.magnitude < dragMinDistance) return;
            }

            _isDragStarted = true;
            OnRecognized.Invoke(eventData.delta / 100f);
        }
    }
}