using UnityEngine;
using UnityEngine.EventSystems;

namespace GoPlay.Touch.Gestures
{
    public class HoldGestureRecognizer : GestureRecognizerBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public float holdTime = 0.4f;
        public float dragMinDistance = 10f;
        public float holdDistance = 3000f;
        public float triggerInterval = 0.1f;
        
        private bool _isPointerDown;
        private Vector2 _startPos;
        private Vector2 _lastPos;
        private float _startTime;
        private float _lastTriggerTime;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
            _startPos = eventData.position;
            _lastPos = eventData.position;
            _startTime = Time.unscaledTime;
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var deltaTime = Time.unscaledTime - _startTime;
            if (deltaTime < holdTime)
            {
                var deltaPos = eventData.position - _startPos;
                if (deltaPos.magnitude >= dragMinDistance)
                {
                    _isPointerDown = false;
                    return;
                }
            }
            
            _lastPos = eventData.position;
        }

        private void Update()
        {
            if (!_isPointerDown) return;
            
            var deltaStartTime = Time.unscaledTime - _startTime;
            if (deltaStartTime < holdTime) return;
            
            var deltaTriggerTime = Time.unscaledTime - _lastTriggerTime;
            if (deltaTriggerTime < triggerInterval) return;
            
            var deltaPos = _lastPos - _startPos;
            if (deltaPos.magnitude > holdDistance) return;

            _lastTriggerTime = Time.unscaledTime;
            Recognize(_lastPos);
        }
    }
}