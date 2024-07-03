using System;
using GoPlay.Cameras;
using GoPlay.Framework.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace GoPlay.Touch.Gestures
{
    [Serializable]
    public class GestureEvent : UnityEvent<Vector3> {}
    
    public class GestureRecognizerBase : MonoBehaviour
    {
        public GestureEvent OnRecognized = new GestureEvent();

        protected virtual void Start()
        {
        }

        protected virtual void Recognize(Vector2 screenPos)
        {
            var worldPos = MainCamera.Instance.ScreenToWorldPoint(screenPos);
            OnRecognized.Invoke(worldPos.ToVector2());
        }
    }
}