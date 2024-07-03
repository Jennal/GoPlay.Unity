using System;
using System.Threading.Tasks;
using UnityEngine;
using GoPlay.Managers;
using GoPlay.Services;

namespace GoPlay
{
    [Serializable]
    public class GameObjectRefer : AssetRefer
    {
#if UNITY_EDITOR
        private GameObject _gameObject;
#endif
        
        public GameObject Instantiate()
        {
            return Instantiate<GameObject>();
        }
        
        public GameObject Instantiate(Transform parent)
        {
            return Instantiate<GameObject>(parent);
        }
        
        public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace)
        {
            return Instantiate<GameObject>(parent, instantiateInWorldSpace);
        }

        public async Task<T> LoadPoolableAsync<T>(params object[] args)
            where T : MonoBehaviour, IPoolable
        {
            var prefab = await LoadPrefabAsync();
            var poolable = ServiceManager.Get<PoolService>().Spawn(prefab, args);
            return poolable.GameObject.GetComponent<T>();
        }
        
        public static implicit operator GameObject(GameObjectRefer data)
        {
#if UNITY_EDITOR
            if (data && data._gameObject) return data._gameObject;
#endif
            
            if (!data) return null;
            
            var gameObject = data.LoadPrefab();
#if UNITY_EDITOR
            data._gameObject = gameObject;
#endif
            return gameObject;
        }
    }
}