using System.Linq;
using System.Threading.Tasks;
using GoPlay.AssetManagement;
using GoPlay.Data.Config;
using GoPlay.Managers;
using UnityEngine;

namespace GoPlay.Framework.UI
{    
    public static class UILoader
    {
        public static GameObject Load<T>()
            where T : UIPanel
        {
            var conf = ConfigData.GetUIAssetByClasName(typeof(T).Name);
            return AssetManager.Instance.LoadPrefab(conf.UIAssetbundle, conf.UIAsset);
        }
        
        public static async Task<GameObject> LoadAsync<T>()
            where T : UIPanel
        {
            // var conf = ConfigData.GetUIAssetByClasName(typeof(T).Name);
            var uiAssetConfs = await ConfigData.GetAsync<UIAssetsConfs>();
            //Debug.Log($"UIAssetConfs: {string.Join(",", uiAssetConfs.Values.Select(o=>o.UIClassName))}");
            var conf = uiAssetConfs.Values.FirstOrDefault(o=>o.UIClassName == typeof(T).Name);
            //Debug.Log($"UI Conf: type: {typeof(T).Name} value: {JsonUtility.ToJson(conf)}");
            return await AssetManager.Instance.LoadAssetAsync<GameObject>(conf.UIAssetbundle, conf.UIAsset);
        }
        
        public static GameObject Load(string typeName)
        {
            var conf = ConfigData.GetUIAssetByClasName(typeName);
            return AssetManager.Instance.LoadPrefab(conf.UIAssetbundle, conf.UIAsset);
        }
        
        public static async Task<GameObject> LoadAsync(string typeName)
        {
            var conf = ConfigData.GetUIAssetByClasName(typeName);
            return await AssetManager.Instance.LoadAssetAsync<GameObject>(conf.UIAssetbundle, conf.UIAsset);
        }
    }
}