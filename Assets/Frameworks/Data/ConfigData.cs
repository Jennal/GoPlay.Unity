using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asyncoroutine;
using GoPlay.AssetManagement;
using GoPlay.AssetManagement.Loaders;
using GoPlay.Globals;
using UnityEngine;

namespace GoPlay.Managers
{
    public partial class ConfigData : MonoSingleton<ConfigData>
    {
        private static Dictionary<string, ScriptableObject> _cache = new Dictionary<string, ScriptableObject>();
        
        public static string AssetBundleName => $"config.{GlobalVariables.Instance.LocalData.Language}";
        public static bool IsReady => AssetManager.Instance.IsAssetBundleLoaded(AssetBundleName);

        public static IEnumerator Init()
        {
            yield return AssetManager.Instance.LoadAssetBundles(AssetBundleName);
        }

        protected static List<IUIAssetConf> s_IUIAssets;
        
        public static IUIAssetConf GetUIAssetByClasName(string className)
        {
            Instance.InitUIAssets();
            if (s_IUIAssets == null) return null;
            return s_IUIAssets.FirstOrDefault(o => o.UIClassName == className);
        }

        partial void InitUIAssets();

        public static async Task<T> GetAsync<T>()
            where T : ScriptableObject
        {
            var key = typeof(T).Name;
            if (_cache.ContainsKey(key) && _cache[key] is T val) return val;

            _cache[key] = await ConfigLoader.LoadConfigAsync<T>();
            return (T)_cache[key];
        }

        public static T Get<T>()
            where T : ScriptableObject
        {
            var key = typeof(T).Name;
            if (_cache.ContainsKey(key) && _cache[key] is T val) return val;

            throw new Exception($"{typeof(T).Name} not loaded yet");
        }
    }
}