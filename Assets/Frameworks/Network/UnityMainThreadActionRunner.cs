using System;
using System.Collections.Generic;
using GoPlay.Services;
using UnityEngine;

namespace GoPlay.Network
{
    public class UnityMainThreadActionRunner : MonoBehaviour, IMainThreadActionRunner
    {
        private static UnityMainThreadActionRunner _instance;
        public static UnityMainThreadActionRunner Instance
        {
            get
            {
                if (_instance == null) Install();
                return _instance;
            }   
        }
        
        private Queue<Action> _actions = new Queue<Action>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Install()
        {
            if (_instance != null) return;
            _instance = new GameObject(nameof(UnityMainThreadActionRunner)).AddComponent<UnityMainThreadActionRunner>();
            DontDestroyOnLoad(_instance);
        }
        
        public void Invoke(Action action)
        {
            if (action == null) return;
            _actions.Enqueue(action);
        }

        private void Update()
        {
            if (_actions.Count <= 0) return;

            while (_actions.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }
    }
}