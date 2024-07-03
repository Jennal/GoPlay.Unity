using System;
using UnityEngine.Events;

namespace GoPlay.Framework.Extensions
{
    public static class UnityEventExtensions
    {
        public static void AddListenerOnce(this UnityEvent unityEvent, Action action)
        {
            UnityAction ue = null;
            ue = () =>
            {
                action.Invoke();
                unityEvent.RemoveListener(ue);
            };
            unityEvent.AddListener(ue);
        }
    }
}