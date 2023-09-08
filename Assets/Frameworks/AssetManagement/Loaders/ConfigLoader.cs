using UnityEngine;
using GoPlay.Globals;
using GoPlay.Managers;

namespace GoPlay.AssetManagement.Loaders
{
    public static class ConfigLoader
    {
        public static T LoadConfig<T>(string name)
            where T : ScriptableObject
        {
#if DEBUG_LOADING_TIME
            var startTime = Time.realtimeSinceStartup;
#endif
            
            var so = AssetManager.Instance.LoadScriptableObject(ConfigData.AssetBundleName, name);

            var abName = name.Substring(0, name.Length - 5);
            abName = $"conf_{abName.ToLower()}.{GlobalVariables.Instance.LocalData.Language}";
            AssetManager.Instance.LoadAssetBundlesSync(abName);
            
#if DEBUG_LOADING_TIME
            var costTime = Time.realtimeSinceStartup - startTime;
            Debug.Log($"ConfigLoader.LoadConfig<{typeof(T).Name}>({name}) => cost {costTime:F3}s");
#endif
            
            return so as T;
        }
    }
}