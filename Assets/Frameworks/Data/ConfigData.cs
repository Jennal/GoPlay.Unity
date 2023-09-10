using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asyncoroutine;
using GoPlay.AssetManagement;
using GoPlay.Globals;

namespace GoPlay.Managers
{
    public partial class ConfigData : MonoSingleton<ConfigData>
    {
        public static string AssetBundleName => $"config.{GlobalVariables.Instance.LocalData.Language}";
        public static bool IsReady => AssetManager.Instance.IsAssetBundleLoaded(AssetBundleName);

        public static async Task Init()
        {
            await AssetManager.Instance.LoadAssetBundles(AssetBundleName);
        }

        protected static List<IUIAssetConf> s_IUIAssets;
        
        public static IUIAssetConf GetUIAssetByClasName(string className)
        {
            Instance.InitUIAssets();
            if (s_IUIAssets == null) return null;
            return s_IUIAssets.FirstOrDefault(o => o.UIClassName == className);
        }

        partial void InitUIAssets();
    }
}