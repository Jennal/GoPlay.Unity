using UnityEngine;
using UnityEngine.EventSystems;

namespace GoPlay.Touch.Gestures
{
    public class TapGestureRecognizer : GestureRecognizerBase, IPointerDownHandler, IPointerClickHandler
    {
        public float tapTime = 1f/15f;
        public float tapDistance = 100f;
        public bool ignoreDistance = false;
        
        private Vector2 _startPos;
        private float _startTime;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _startPos = eventData.position;
            _startTime = Time.unscaledTime;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var deltaTime = Time.unscaledTime - _startTime;
            if (deltaTime >= tapTime)
            {
                // Debug.Log("TapGestureRecognizer: Time is up");
                return;
            }
            
            var deltaPos = eventData.position - _startPos;
            if (!ignoreDistance && deltaPos.magnitude >= tapDistance)
            {
                // Debug.Log($"TapGestureRecognizer: Distance is too far: {deltaPos.magnitude}");
                return;
            }

            Recognize(eventData.position);
        }
    }
}