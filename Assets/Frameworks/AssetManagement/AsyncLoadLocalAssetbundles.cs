﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
 using GoPlay.Async;
 using UnityEngine;
using UnityEngine.Networking;

 namespace GoPlay.AssetManagement
{
    public class AyncLoadLocalAssetbundles : CustomAsyncOperation
    {
        public override float Progress
        {
            get
            {
                if (assetInfosForLoad == null || assetInfosForLoad.Count <= 0) return 0;
                
                return (float)(Assetbundles.Count + assetInfosFailed.Count) / assetInfosForLoad.Count;
            }
        }

        public List<AssetBundle> Assetbundles = new List<AssetBundle>();

        private List<AssetInfo> assetInfosForLoad;
        private List<AssetInfo> assetInfosFailed = new List<AssetInfo>();

        public AyncLoadLocalAssetbundles(List<AssetInfo> assetInfosForLoad)
        {
            this.assetInfosForLoad = assetInfosForLoad;
        }

        public override IEnumerator Start()
        {
            return StartWithCache();
        }

        private IEnumerator StartWithCache()
        {
#if DEBUG_LOADING_TIME
                var startTime = Time.realtimeSinceStartup;
#endif
            var opList = new Dictionary<AssetInfo, AsyncOperation>();
            foreach (var assetInfo in assetInfosForLoad)
            {
                
                //读取更新后的文件
                var path = assetInfo.Name.PersistentAssetsPath();
                if (File.Exists(path))
                {
                    var op = AssetBundle.LoadFromFileAsync(path);
                    opList.Add(assetInfo, op);
                }
                else
                {
                    //读取原包中的文件
                    path = assetInfo.Name.StreamingAssetsPath();
                    var op = StreamingAssetReader.LoadAssetBundleAsyncOperation(path, assetInfo.Version);
                    opList.Add(assetInfo, op);
                }
            }

            foreach (var item in opList)
            {
                var assetInfo = item.Key;
                var op = item.Value;
                
                yield return op;

                switch (op)
                {
                    case AssetBundleCreateRequest request:
                        Assetbundles.Add(request.assetBundle);
                        break;
                    case UnityWebRequestAsyncOperation request:
                        if (request.webRequest.isDone)
                        {
                            var assetBundle = DownloadHandlerAssetBundle.GetContent(request.webRequest);
                            Assetbundles.Add(assetBundle);
                        }

                        //失败
                        assetInfosFailed.Add(assetInfo);
                        OnErrorEvent(new Exception(string.Format("Loading Assetbundle failed: {0}[{1}]", assetInfo.Name,
                            assetInfo.Version)));
                        break;
                }
                
#if DEBUG_LOADING_TIME
                var cost = Time.realtimeSinceStartup - startTime;
                Debug.Log($"AsyncLoadLocalAssetBundles.StartWithCache: loading {assetInfo.Name}, cost {cost:F3}s");
#endif
            }
        }
    }
}
