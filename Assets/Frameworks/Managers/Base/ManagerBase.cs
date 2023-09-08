using System;
using System.Collections.Generic;
using System.Linq;
using GoPlay.Globals;
using GoPlay.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GoPlay.Managers
{
    public abstract class ManagerBase<T, TManager> : MonoBehaviour
        where TManager : ManagerBase<T, TManager>
        where T : MonoBehaviour, IInitialize
    {
        protected static TManager _instance;
        protected static List<T> _managerList = new List<T>();

        protected static void CheckInstance()
        {
            if (GlobalVariables.IsQuitting) return;
            if (_instance) return;
            
            var go = new GameObject(typeof(TManager).Name);
            _instance = go.AddComponent<TManager>();
            if (Application.isPlaying) {
                DontDestroyOnLoad(_instance);
            }
        }
        
        public static TManagerItem Create<TManagerItem>(Transform root=null)
            where TManagerItem : MonoBehaviour, T
        {
            CheckInstance();
            
            var mgr = FindObjectsOfType<TManagerItem>().FirstOrDefault(o => o.enabled && o.gameObject.activeInHierarchy);
            if (!mgr)
            {
                var go = new GameObject(typeof(TManagerItem).Name);
                mgr = go.AddComponent<TManagerItem>();
            }
            
            if(root) mgr.transform.SetParent(root);
            else mgr.transform.SetParent(_instance.transform);

            if (!_managerList.OfType<TManagerItem>().Any(o => o))
            {
                Add(mgr);
                mgr.Initialize();
            }

            return mgr;
        }
        
        public static void Destroy<TManagerItem>() 
            where TManagerItem : MonoBehaviour, T
        {
            CheckInstance();
            var mgrs = _managerList.OfType<TManagerItem>();
            foreach (var mgr in mgrs) {
                _managerList.Remove(mgr);
                if (mgr is IDestroyed exec) exec.Destroyed();
                Destroy(mgr.gameObject);
            }
        }
        
        public static TManagerItem Check<TManagerItem>()
            where TManagerItem : MonoBehaviour, T
        {
            CheckInstance();

            return _managerList.OfType<TManagerItem>().FirstOrDefault(o => o);
        }

        public static IEnumerable<TManagerItem> GetAll<TManagerItem>()
        {
            CheckInstance();
            
            return _managerList.OfType<TManagerItem>();
        }
        
        public static TManagerItem Get<TManagerItem>()
            where TManagerItem : MonoBehaviour, T
        {
            if (GlobalVariables.IsQuitting) throw new Exception($"Don't call Get<{nameof(TManagerItem)}> while destroy");
            
            CheckInstance();
            
            var mgr = _managerList.OfType<TManagerItem>().FirstOrDefault(o => o);
            if (!mgr) mgr = Create<TManagerItem>();

            return mgr;
        }
        
        public static void Add<TManagerItem>(TManagerItem mgr)
            where TManagerItem : MonoBehaviour, T
        {
            CheckInstance();
            
            // Debug.Log($"{_instance.name}.Add({_managerList.OfType<TManagerItem>().Count()} / {_managerList.Count}) => {typeof(TManagerItem).Name}");
            _managerList.Add(mgr);
        }

        public static void Clear()
        {
            CheckInstance();

            foreach (var mgr in _managerList)
            {
                if (mgr is IDestroyed exec) exec.Destroyed();
                Object.Destroy(mgr);
            }
            _managerList.Clear();
        }

        protected virtual void FixedUpdate()
        {
            foreach (var mgr in _managerList)
            {
                if(!mgr) continue;
                if(!mgr.gameObject.activeInHierarchy) continue;
                if(!mgr.enabled) continue;
                
                if (mgr is IFixedExecute exec) exec.FixedExecute();
            }
            
            foreach (var mgr in _managerList)
            {
                if(!mgr) continue;
                if(!mgr.gameObject.activeInHierarchy) continue;
                if(!mgr.enabled) continue;
                
                if (mgr is IFixedCleanUp exec) exec.FixedCleanUp();
            }
        }
        
        protected virtual void Update()
        {
            foreach (var mgr in _managerList)
            {
                if(!mgr) continue;
                if(!mgr.gameObject.activeInHierarchy) continue;
                if(!mgr.enabled) continue;
                
                if (mgr is IExecute exec) exec.Execute();
            }
        }

        protected virtual void LateUpdate()
        {
            foreach (var mgr in _managerList)
            {
                if(!mgr) continue;
                if(!mgr.gameObject.activeInHierarchy) continue;
                if(!mgr.enabled) continue;
                
                if (mgr is ICleanUp exec) exec.CleanUp();
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var mgr in _managerList)
            {
                if(!mgr) continue;
                if(!mgr.gameObject.activeInHierarchy) continue;
                if(!mgr.enabled) continue;
                
                if (mgr is IDestroyed exec) exec.Destroyed();
            }
        }
    }
}
