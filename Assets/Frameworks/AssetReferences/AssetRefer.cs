using System;
using System.IO;
using System.Threading.Tasks;
using GoPlay.AssetManagement;
using UnityEngine;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;

namespace GoPlay
{
    [Serializable]
    public class AssetRefer : ICloneable
    {
        public string AssetBundle;
        public string AssetName;

        [YamlIgnore]
        public string AssetShortName => Path.GetFileNameWithoutExtension(AssetName);

        public static implicit operator bool(AssetRefer data)
        {
            return data != null && !string.IsNullOrEmpty(data.AssetBundle) && !string.IsNullOrEmpty(data.AssetName);
        }
        
        public Sprite LoadSprite()
        {
            return AssetManager.Instance.LoadSprite(AssetBundle, AssetName);
        }

        public GameObject LoadPrefab()
        {
            return AssetManager.Instance.LoadAsset<GameObject>(AssetBundle, AssetName);
        }

        public T Load<T>()
            where T : Object
        {
            return AssetManager.Instance.LoadAsset<T>(AssetBundle, AssetName);
        }
        
        public T Instantiate<T>()
            where T : Object
        {
            var prefab = AssetManager.Instance.LoadAsset<T>(AssetBundle, AssetName);
            return Object.Instantiate(prefab);
        }
        
        public T Instantiate<T>(Transform parent)
            where T : Object
        {
            var prefab = AssetManager.Instance.LoadAsset<T>(AssetBundle, AssetName);
            return Object.Instantiate(prefab, parent);
        }
        
        public T Instantiate<T>(Transform parent, bool instantiateInWorldSpace)
            where T : Object
        {
            var prefab = AssetManager.Instance.LoadAsset<T>(AssetBundle, AssetName);
            return Object.Instantiate(prefab, parent, instantiateInWorldSpace);
        }
        
        public Task<Sprite> LoadSpriteAsync()
        {
            return AssetManager.Instance.LoadSpriteAsync(AssetBundle, AssetName);
        }

        public Task<GameObject> LoadPrefabAsync()
        {
            return AssetManager.Instance.LoadAssetAsync<GameObject>(AssetBundle, AssetName);
        }

        public Task<T> LoadAsync<T>()
            where T : Object
        {
            return AssetManager.Instance.LoadAssetAsync<T>(AssetBundle, AssetName);
        }
        
        public async Task<T> InstantiateAsync<T>()
            where T : Object
        {
            var prefab = await AssetManager.Instance.LoadAssetAsync<T>(AssetBundle, AssetName);
            return Object.Instantiate(prefab);
        }
        
        public async Task<T> InstantiateAsync<T>(Transform parent)
            where T : Object
        {
            var prefab = await AssetManager.Instance.LoadAssetAsync<T>(AssetBundle, AssetName);
            return Object.Instantiate(prefab, parent);
        }
        
        public async Task<T> InstantiateAsync<T>(Transform parent, bool instantiateInWorldSpace)
            where T : Object
        {
            var prefab = await AssetManager.Instance.LoadAssetAsync<T>(AssetBundle, AssetName);
            return Object.Instantiate(prefab, parent, instantiateInWorldSpace);
        }

        public object Clone()
        {
            return new AssetRefer
            {
                AssetBundle = AssetBundle,
                AssetName = AssetName
            };
        }
    }
}