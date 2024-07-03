using System;
using System.Collections.Generic;
using System.Linq;
using GoPlay.Managers;
using GoPlay.Services.Base;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace GoPlay.Services
{
    [Serializable]
    public class PoolData
    {
#if ODIN_INSPECTOR_3
#if UNITY_EDITOR
        [Title("$Name", "$Count")]
#endif
        [OnValueChanged("FixComponentType")]
        [AssetsOnly] /*[ValidateInput("ValidatePrefab", "Prefab must have a IPoolable")]*/
#endif
        public GameObject Prefab;
        [HideInInspector] public Queue<IPoolable> ReusableObjects = new Queue<IPoolable>();
        public int ReuseLimit = 5;

        private Type _componentType;
        public Type ComponentType
        {
            get
            {
                if (!Prefab) return null;
                if (_componentType == null) _componentType = Prefab.GetComponent<IPoolable>()?.GetType();
                return _componentType;
            }
            set { _componentType = value; }
        }

        #region Validate

        private bool ValidatePrefab(GameObject go)
        {
            if (!go) return false;
            var comp = go.GetComponent<IPoolable>();
            return comp != null;
        }

        #endregion
        
        #region OnValueChanged

        private void FixComponentType()
        {
            if (!Prefab) return;
            
            var comp = Prefab.GetComponent<IPoolable>();
            if(comp != null) ComponentType = comp.GetType();
        }

        #endregion
        
        #region Statistics

        #if UNITY_EDITOR

        public string Name => ComponentType?.Name;
        public string Count => $"pool: {ReusableObjects.Count:D2}, live: {live}, destoried: {destoried}, avg: {average:F1}, peek: {peek}, hit rate: {hitRate:F2}";

        private int live;
        private int peek;
        private float average;
        private int destoried;
        private int totalTimes;
        private int hitTimes;

        private float hitRate
        {
            get
            {
                if (totalTimes == 0) return 0f;
                return (float)hitTimes / totalTimes;
            }
        }

        public void StatisSpawn()
        {
            live++;
            totalTimes++;
            hitTimes++;
            peek = Mathf.Max(peek, live);
            CalcAverage();
        }

        private void CalcAverage()
        {
            if (totalTimes > 1) average = (float) (totalTimes - 1) / totalTimes * average + (float) live / totalTimes;
            else average = live;
        }

        public void StatisCreate()
        {
            totalTimes++;
            live++;
            peek = Mathf.Max(peek, live);
            CalcAverage();
        }

        public void StatisDespawn()
        {
            live--;
            CalcAverage();
        }
        
        public void StatisDestory()
        {
            live--;
            destoried++;
            CalcAverage();
        }
#endif

        #endregion
    }
    
    public class PoolService : ServiceBase
    {
        public static PoolService Instance => ServiceManager.Get<PoolService>();
        
        private static int Count = 0; 
        public List<PoolData> Pools = new List<PoolData>();
        
        private List<Tuple<Action, float>> _delayDespawnObjects = new List<Tuple<Action, float>>();

        private Canvas _uiRoot;
        public Canvas UIRoot
        {
            get
            {
                if (!_uiRoot)
                {
                    var go = new GameObject("UIRoot");
                    go.layer = LayerMask.NameToLayer("UI");
                    _uiRoot = go.AddComponent<Canvas>();
                    _uiRoot.sortingLayerName = "UI";
                    _uiRoot.renderMode = RenderMode.ScreenSpaceCamera;
                    _uiRoot.worldCamera = FindObjectsOfType<Camera>().FirstOrDefault(o => o.name == "UICamera");
                    go.transform.SetParent(transform);
                }

                return _uiRoot;
            }
        }

        public void Register(GameObject prefab, int limit=5)
        {
            Register<IPoolable>(prefab, limit);
        }
        
        public void Register<T>(GameObject prefab, int limit=5)
            where T : IPoolable
        {
            if (Pools.Any(o => o.Prefab == prefab)) return;
            
            Pools.Add(new PoolData
            {
                Prefab = prefab,
                ComponentType = typeof(T),
                ReuseLimit = limit,
            });
        }

        public void Unregister(GameObject prefab)
        {
            var item = Pools.FirstOrDefault(o => o.Prefab == prefab);
            if (item == null) return; //throw new Exception($"Can't find Pool: {prefab.name}");

            while (item.ReusableObjects.Count > 0)
            {
                var obj = item.ReusableObjects.Dequeue();
                if(obj.GameObject) SafeDestroy(obj.GameObject);
            }
            Pools.Remove(item);
        }
        
//        public void Unregister<T>()
//            where T : PoolableMonoBehaviour
//        {
//            var item = Pools.FirstOrDefault(o => o.ComponentType == typeof(T));
//            if (item == null) throw new Exception($"Can't find Pool of type: {typeof(T).Name}");
//
//            while (item.ReusableObjects.Count > 0)
//            {
//                var obj = item.ReusableObjects.Dequeue();
//                SafeDestroy(obj.gameObject);
//            }
//            Pools.Remove(item);
//        }

        public void UnregisterAll()
        {
            for (var i=Pools.Count - 1; i >= 0; i--)
            {
                var pool = Pools[i];
                Unregister(pool.Prefab);
            }
            
            Pools.Clear();
            Count = 0;
        }

        public bool IsRegisted<T>()
            where T : MonoBehaviour, IPoolable
        {
            return Pools.Any(o => o.ComponentType == typeof(T));
        }
        
        public bool IsRegisted(GameObject prefab)
        {
            var comp = prefab.GetComponent<IPoolable>();
            return Pools.Any(o => o.ComponentType == comp.GetType() || o.Prefab == prefab);
        }
        
        public IPoolable Spawn(GameObject prefab, params object[] args)
        {
            return Spawn<PoolableMonoBehaviour>(prefab, args);
        }
        
        public T Spawn<T>(GameObject prefab, params object[] args)
            where T : MonoBehaviour, IPoolable
        {
            Register<T>(prefab);
            
            var item = Pools.FirstOrDefault(o => o.Prefab == prefab);
            if (item == null) throw new Exception($"Can't find Pool of type: {typeof(T).Name}");

            T result = default;
            if (item.ReusableObjects.Count > 0)
            {
                result = (T)item.ReusableObjects.Dequeue();
#if UNITY_EDITOR
                if(result) item.StatisSpawn();
#endif
            }
            
            if (!result)
            {
                Count++;
                var root = getParent(item.Prefab);
                var go = Instantiate(item.Prefab, root);
                go.name = $"{item.Prefab.name}_{Count}";
                result = go.GetComponent<T>();
                if (!result) result = go.AddComponent<T>();
#if UNITY_EDITOR
                item.StatisCreate();
#endif
            }

            result.Pool = item;
            result.OnSpawn(args);
            return result;
        }
        
        public T Spawn<T>(params object[] args)
            where T : MonoBehaviour, IPoolable
        {
            var item = Pools.FirstOrDefault(o => o.ComponentType == typeof(T));
            if (item == null) throw new Exception($"Can't find Pool of type: {typeof(T).Name}");

            return Spawn<T>(item.Prefab, args);
        }

        public void Despawn<T>(T obj)
            where T : MonoBehaviour, IPoolable
        {
            if (!Instance) return;
            if (!obj || !obj.gameObject) return;
            
            obj.OnDespawn();
            var item = obj.Pool;
            if (item == null)
            {
                // throw new Exception($"Can't find Pool of type: {typeof(T).Name}");
                SafeDestroy(obj.gameObject);
                return;
            }
            if (item.ReusableObjects.Contains(obj)) return;
            
            if (!Pools.Contains(item) || item.ReusableObjects.Count >= item.ReuseLimit)
            {
                SafeDestroy(obj.gameObject);

#if UNITY_EDITOR
                item.StatisDestory();
#endif
                return;
            }
            
            var root = getParent(obj.gameObject);
            obj.transform.SetParent(root, false);
            item.ReusableObjects.Enqueue(obj);
            
#if UNITY_EDITOR
            item.StatisDespawn();
#endif
        }

        public void Despawn<T>(T obj, float delay)
            where T : MonoBehaviour, IPoolable
        {
            if (delay <= 0f)
            {
                Despawn(obj);
            }
            else
            {
                _delayDespawnObjects.Add(new Tuple<Action, float>(() => { Despawn(obj); }, delay));
            }
        }

        private static void SafeDestroy(GameObject go)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(go);    
            }
            else
            {
                DestroyImmediate(go);
            }
#else
            Destroy(go);
#endif
        }

        private Transform getParent(GameObject go)
        {
            if (go.GetComponent<Graphic>() || go.GetComponent<CanvasRenderer>()) return UIRoot.transform;
            return transform;
        }

        private void Update()
        {
            for (var i = _delayDespawnObjects.Count - 1; i >= 0; i--)
            {
                var item = _delayDespawnObjects[i];
                item.Item2 -= Time.deltaTime;

                if (item.Item2 > 0) continue;
                
                item.Item1.Invoke();
                _delayDespawnObjects.Remove(item);
            }
        }
    }
}
