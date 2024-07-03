using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asyncoroutine;
using GoPlay.Framework.Extensions;

namespace GoPlay.Framework.UI
{
    public partial class UIManager
    {
        internal static Dictionary<string, Queue<(string, object[])>> WindowQueue = new Dictionary<string, Queue<(string, object[])>>();

        public static void Queue<T>(params object[] data)
            where T : UIPageChild
        {
            Queue(typeof(T).Name, typeof(T), data);
        }
        
        public static void Queue(string key, Type type, params object[] data)
        {
            if (!WindowQueue.ContainsKey(key)) WindowQueue[key] = new Queue<(string, object[])>();
            WindowQueue[key].Enqueue((type.Name, data));

            if (WindowQueue[key].Count == 1)
            {
                Instance.StartCoroutine(OpenNextWindow(key).Invoke().AsCoroutine());
            }
        }

        private static Func<Task> OpenNextWindow(string key)
        {
            return async () =>
            {
                if (!WindowQueue.ContainsKey(key) || WindowQueue[key].Count <= 0) return;

                var (typeName, data) = WindowQueue[key].Peek();
                var win = await OpenAsync(typeName, data);
                win.OnCloseEvent.AddListenerOnce(() =>
                {
                    WindowQueue[key].Dequeue();
                    OpenNextWindow(key).Invoke();
                });
            };
        }
    }
}