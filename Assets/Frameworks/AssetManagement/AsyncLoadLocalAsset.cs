using System.Collections;
using GoPlay.Async;
using UnityEngine;

namespace GoPlay.AssetManagement
{
    public class AsyncLoadLocalAsset<T> : CustomAsyncOperation
        where T : Object
    {
        protected string AssetbundleName;
        protected string AssetName;
        
        protected AyncLoadLocalAssetbundles AyncLoadLocalAssetbundles;

        private T _result;
        public T Result => _result;
        public override float Progress => _result ? 1f : 0f;
        public override IEnumerator Start()
        {
            if ( ! AssetManager.Instance.IsAssetBundleLoaded(AssetbundleName))
            {
                yield return AyncLoadLocalAssetbundles.Start();
            }

            _result = AssetManager.Instance.LoadAsset<T>(AssetbundleName, AssetName);
        }
        
        public AsyncLoadLocalAsset(string assetBundleName, string assetName)
        {
            AssetbundleName = assetBundleName;
            AssetName = assetName;

            if ( ! AssetManager.Instance.IsAssetBundleLoaded(AssetbundleName))
            {
                var hash = AssetManager.Instance.GetHash128(assetBundleName);
                var assetInfo = new AssetInfo
                {
                    Name = assetBundleName,
                    Version = hash
                };
                AyncLoadLocalAssetbundles = AssetManager.Instance.LoadAssetBundles(assetInfo);
            }
        }
    }
}