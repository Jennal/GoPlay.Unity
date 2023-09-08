using System.Linq;
using System.Threading.Tasks;
using GoPlay.AssetManagement;
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
            return AssetManager.Instance.LoadPrefab(conf.Assetbundle, conf.Asset);
        }
        
        public static Task<GameObject> LoadAsync<T>()
            where T : UIPanel
        {
            var conf = ConfigData.GetUIAssetByClasName(typeof(T).Name);
            return AssetManager.Instance.LoadAssetAsync<GameObject>(conf.Assetbundle, conf.Asset);
        }
        
        public static GameObject Load(string typeName)
        {
            var conf = ConfigData.GetUIAssetByClasName(typeName);
            return AssetManager.Instance.LoadPrefab(conf.Assetbundle, conf.Asset);
        }
        
        public static async Task<GameObject> LoadAsync(string typeName)
        {
            var conf = ConfigData.GetUIAssetByClasName(typeName);
            return await AssetManager.Instance.LoadAssetAsync<GameObject>(conf.Assetbundle, conf.Asset);
        }
    }
}