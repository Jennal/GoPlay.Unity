// #undef DEBUG_ASSET_BUNDLE
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asyncoroutine;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
#if UNITY_WEBGL || UNITY_EDITOR
using UnityEngine.Networking;
#endif
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 414
#endif

namespace GoPlay.AssetManagement
{
    public class AssetInfo
    {
        public string Name;
        public Hash128 Version;
    }

    /// <summary>
    /// AssetBundle管理器，支持在非打包情况下，在Editor中测试
    /// </summary>
    public class AssetManager : MonoSingleton<AssetManager>
    {
#if ODIN_INSPECTOR_3
        [ReadOnly, ShowInInspector]
#endif
        private List<AssetBundleRefCount> loadedAssetBundles = new List<AssetBundleRefCount>();
        private AssetBundleManifest assetBundleManifest = null;

        private bool _isInitingVersion = false;

        public AyncLoadLocalAssetbundles CurrentAsyncOperation;

        public int LoadedCount
        {
            get
            {
#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
                return loadedAssetBundles.Count;
#else
                return AssetBundle.GetAllLoadedAssetBundles().Count();
#endif
            }
        }

        private AssetManager()
        {
        }

        public void DumpLoaded()
        {
            var str = "AssetManager.DumpLoaded:";
            foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                str += "\n\t => " + bundle.name;
            }

            Debug(str);
        }

        private RuntimePlatform platform
        {
            get
            {
#if UNITY_ANDROID
                return RuntimePlatform.Android;
#elif UNITY_IOS
                return RuntimePlatform.IPhonePlayer;
#elif UNITY_WEBGL
                return RuntimePlatform.WebGLPlayer;
#elif UNITY_STANDALONE_OSX
                return RuntimePlatform.OSXPlayer;
#elif UNITY_STANDALONE_WIN
                return RuntimePlatform.WindowsPlayer;
#endif
            }
        }

        public IEnumerator Init()
        {
            yield return InitVersion();
        }
        
        private IEnumerator InitVersion()
        {
#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
            yield break;
#else
            while (_isInitingVersion) yield return null;
            if (assetBundleManifest != null) yield break;
#if !UNITY_WEBGL
            while (!Caching.ready) yield return null;
#endif

            Debug("AssetManager.InitVersion");
            _isInitingVersion = true;
            
            //版本信息不走Caching，强制读取最新，防止之后读取的Assetbundle都是缓存过的无法更新
            var name = platform.ToPath();
            var path = name.StreamingAssetsPath();
            Debug($"load Manifest: {path}");
#if UNITY_WEBGL && !UNITY_EDITOR
            var op = UnityWebRequestAssetBundle.GetAssetBundle(path);
            yield return op.SendWebRequest();
            var bundle = DownloadHandlerAssetBundle.GetContent(op);
            assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
#else
            var op = StreamingAssetReader.LoadAssetBundleFromFileAsync(path);
            yield return op;
            assetBundleManifest = op.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
#endif
            Debug($"assetBundleManifest: {assetBundleManifest}");
            // var asyncLocalLoader = new AyncLoadLocalAssetbundles(new List<AssetInfo>
            // {
            //     new AssetInfo
            //     {
            //         Name = name,
            //         Version = new Hash128(0U, 0U, 0U, 0U),
            //     },
            // });
            // yield return asyncLocalLoader.Start();
            //
            // foreach (var bundle in asyncLocalLoader.Assetbundles)
            // {
            //     assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //     //版本信息中不包含自己
            //     //removeOldVersion(name);
            // }
            _isInitingVersion = false;

            //foreach (var bundle in assetBundleManifest.GetAllAssetBundles())
            //{
            //    Logger.Debug("Bundle: {0}", bundle);
            //}

            //var list = new List<string>();
            //Caching.GetAllCachePaths(list);
            //foreach (var item in list)
            //{
            //    Logger.Debug("Cache path: {0}", item);
            //}
#endif
        }

        public bool Exists(string name)
        {
            if (assetBundleManifest == null) return true;
            return assetBundleManifest.GetAllAssetBundles().Any(o => o == name);
        }
        
        public Hash128 GetHash128(string name)
        {
            //TODO:
            //return new Hash128();

            if (assetBundleManifest == null) return new Hash128();

            return assetBundleManifest.GetAssetBundleHash(name);
        }

        public bool IsAssetBundleLoaded(string name)
        {
#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
            return loadedAssetBundles.Any(o => o.Value == name);
#else
            return AssetBundle.GetAllLoadedAssetBundles().Any(o => o.name == name);
#endif
        }

        /// <summary>
        /// 一次性加载多个AssetBundle，用于切换场景
        /// </summary>
        /// <param name="assetbundleInfos"></param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundles(params string[] assetbundleNames)
        {
            // Debug($"AssetManager.LoadAssetBundles: {string.Join(", ", assetbundleNames)}");
            yield return InitVersion();

            if (assetbundleNames == null || assetbundleNames.Length == 0) yield break;

            assetbundleNames = assetbundleNames.Distinct().ToArray();
            var infos = new List<AssetInfo>();
            foreach (var name in assetbundleNames)
            {
                if (!Exists(name)) continue;
                if (IsAssetBundleLoaded(name) || infos.Any(o => o.Name == name)) continue;

                var depends = GetDependency(name);
                foreach (var depend in depends)
                {
                    if (IsAssetBundleLoaded(depend.Name) || infos.Any(o => o.Name == depend.Name)) continue;

                    infos.Add(depend);
                }

                infos.Add(new AssetInfo
                {
                    Name = name,
                    Version = GetHash128(name),
                });
            }

#if DEBUG_LOADING_TIME
            var startTime = UnityEngine.Time.realtimeSinceStartup;
#endif
            
            yield return LoadAssetBundles(infos.ToArray())?.Start();
            
#if DEBUG_LOADING_TIME
            var costTime = UnityEngine.Time.realtimeSinceStartup - startTime;
            UnityEngine.Debug.Log($"AssetManager.LoadAssetBundles => total cost: {costTime:F3}s");
#endif
        }

        public void LoadAssetBundlesSync(params string[] assetbundleNames)
        {
#if !(UNITY_EDITOR && DEBUG_ASSET_BUNDLE)
            Assert.IsTrue(assetBundleManifest);
#endif
            if (assetbundleNames == null || assetbundleNames.Length == 0) return;

            assetbundleNames = assetbundleNames.Distinct().ToArray();
            var infos = new List<AssetInfo>();
            foreach (var name in assetbundleNames)
            {
                if (!Exists(name)) continue;
                if (IsAssetBundleLoaded(name) || infos.Any(o => o.Name == name)) continue;

                var depends = GetDependency(name);
                foreach (var depend in depends)
                {
                    if (IsAssetBundleLoaded(depend.Name) || infos.Any(o => o.Name == depend.Name)) continue;

                    infos.Add(depend);
                }

                infos.Add(new AssetInfo
                {
                    Name = name,
                    Version = GetHash128(name),
                });
            }
            
            foreach (var assetInfo in infos)
            {
                var item = loadedAssetBundles.FirstOrDefault(o => o.Value == assetInfo.Name);
                if (item != null)
                {
                    item.Retain();
                    continue;
                }

                //加载
                loadedAssetBundles.Add(new AssetBundleRefCount
                {
                    Value = assetInfo.Name,
                    Count = 1,
                });
                
#if !(UNITY_EDITOR && DEBUG_ASSET_BUNDLE)
                var path = assetInfo.Name.StreamingAssetsPath();
                var bundle = StreamingAssetReader.LoadAssetBundleAsyncOperationSync(path, assetInfo.Version);
                //TODO: bundle
#endif
            }
        }

        private IEnumerable<AssetInfo> GetDependency(string name)
        {
#if !(UNITY_EDITOR && DEBUG_ASSET_BUNDLE)
            //自动加载依赖
            var depends = assetBundleManifest.GetAllDependencies(name);
            foreach (var depend in depends)
            {
                yield return new AssetInfo
                {
                    Name = depend,
                    Version = GetHash128(depend),
                };
            }
#else
                var depends = AssetDatabase.GetAssetBundleDependencies(name, true);
                foreach (var depend in depends)
                {
                    yield return new AssetInfo
                    {
                        Name = depend,
                        Version = GetHash128(depend),
                    };
                }
#endif
        }
        
        internal AyncLoadLocalAssetbundles LoadAssetBundles(params AssetInfo[] assetbundleInfos)
        {
            foreach (var assetInfo in assetbundleInfos)
            {
                var item = loadedAssetBundles.FirstOrDefault(o => o.Value == assetInfo.Name);
                if (item != null)
                {
                    item.Retain();
                    continue;
                }

                //模拟加载
                loadedAssetBundles.Add(new AssetBundleRefCount
                {
                    Value = assetInfo.Name,
                    Count = 1,
                });
                //Logger.Debug("Loading AssetBundle Done: {0}", assetInfo.Name);
            }
#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
            return null;
#else
            var list = new List<AssetInfo>();
            foreach (var assetInfo in assetbundleInfos)
            {
                if (IsAssetBundleLoaded(assetInfo.Name)) continue;

                //无论如何，加入下载列表
                list.Add(assetInfo);
            }

            if (list.Count == 0) return null;

            CurrentAsyncOperation = new AyncLoadLocalAssetbundles(list);
            return CurrentAsyncOperation;

            //foreach (var bundle in CurrentAsyncOperation.Assetbundles)
            //{
            //    removeOldVersion(bundle.name);
            //}
            //CurrentAsyncOperation = null;
#endif
        }

        //Cache会自动根据hash来判断是否更新Cache，不需要手工删除
        //private void removeOldVersion(string bundleName)
        //{
        //    if (assetBundleManifest == null) return;

        //    var curVersion = GetHash128(bundleName);
        //    var list = new List<Hash128>();
        //    Caching.GetCachedVersions(bundleName, list);
        //    foreach (var item in list)
        //    {
        //        if (item == curVersion) continue;

        //        Caching.ClearCachedVersion(bundleName, item);
        //    }
        //}

        /// <summary>
        /// 清空所有已经加载的AssetBundle
        /// </summary>
        /// <param name="unloadLoadedObject"></param>
        public void UnloadAllAssetBundles(bool unloadLoadedObject = false)
        {
            loadedAssetBundles.Clear();
#if !(UNITY_EDITOR && DEBUG_ASSET_BUNDLE)
            AssetBundle.UnloadAllAssetBundles(unloadLoadedObject);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <param name="unloadLoadedObject"></param>
        public void UnloadAssetBundle(string assetbundleName, bool unloadLoadedObject = false)
        {
            var item = loadedAssetBundles.FirstOrDefault(o => o.Value == assetbundleName);
            if (item == null) return;

            var list = new List<string>();
            list.Add(item.Value);
            
            var depends = GetDependency(assetbundleName);
            list.AddRange(depends.Select(o => o.Name));
            list = list.Distinct().ToList();

            foreach (var abName in list)
            {
                item = loadedAssetBundles.FirstOrDefault(o => o.Value == abName);
                item?.Release(() =>
                {
#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
                    var ab = GetAssetBundle(abName);
                    if (ab == null) return;

                    ab.Unload(unloadLoadedObject);
#endif
                    loadedAssetBundles.Remove(item);
                });
            }
        }

        /// <summary>
        /// 加载assetBundle中的所有资源，该接口比每个资源单独加载要快
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public Object[] LoadAllAssets(string assetBundleName)
        {
            if (!IsAssetBundleLoaded(assetBundleName))
            {
                Error("AssetBundle: {0} 未加载，无法直接加载Asset", assetBundleName);
                return null;
            }

#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            var result = new List<Object>();

            foreach (var path in assetPaths)
            {
                var targets = AssetDatabase.LoadAllAssetsAtPath(path);
                result.AddRange(targets);
            }

            return result.ToArray();
#else
            var ab = GetAssetBundle(assetBundleName);
            if (ab == null) return null;

            return ab.LoadAllAssets();
#endif
        }

#if UNITY_EDITOR
        public string GetAssetPath(string assetBundleName, string assetName)
        {
            assetName = Path.GetFileNameWithoutExtension(assetName);
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (assetPaths.Length == 0)
            {
                Error("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                return null;
            }

            return assetPaths[0];
        }
#endif

        public T LoadAsset<T>(string assetBundleName, string assetName)
            where T : Object
        {
            if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(assetName))
            {
                Warning("AssetBundle: '{0}'\tAsset: '{1}' 为空", assetBundleName, assetName);
                return null;
            }
            
            if (!IsAssetBundleLoaded(assetBundleName))
            {
                LoadAssetBundlesSync(assetBundleName);
                // Error("AssetBundle: {0} 未加载，无法直接加载Asset {1}", assetBundleName, assetName);
                // return null;
            }

#if UNITY_EDITOR && DEBUG_ASSET_BUNDLE
            var path = assetName;
            if (!path.StartsWith("Assets/"))
            {
                string[] assetPaths =
                    AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Error("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                    return null;
                }

                foreach (var assetPath in assetPaths)
                {
#if DEBUG_LOADING_ASSET
                    UnityEngine.Debug.Log($"Loading {assetPath}");
#endif
                    var target = AssetDatabase.LoadMainAssetAtPath(assetPath);
                    if (target is T) return target as T;
                }
            }
            else
            {
                var target = AssetDatabase.LoadMainAssetAtPath(path);
                if (target is T) return target as T;
            }

            return null;
#else
            var ab = GetAssetBundle(assetBundleName);
            if (ab == null) return null;

            return ab.LoadAsset<T>(assetName);
#endif
        }

        public async Task<T> LoadAssetAsync<T>(string assetBundleName, string assetName)
            where T : Object
        {
            if (!IsAssetBundleLoaded(assetBundleName))
            {
                await LoadAssetBundles(assetBundleName);
            }
            
            var ala = new AsyncLoadLocalAsset<T>(assetBundleName, assetName);
            await ala.Start();
            return ala.Result;
        }

        public AssetBundle GetAssetBundle(string assetBundleName)
        {
            return AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(o => o.name == assetBundleName);
        }

        /// <summary>
        /// 加载二进制文件，用于sqlite数据文件
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public byte[] LoadBytes(string assetBundleName, string assetName)
        {
            var asset = LoadAsset<TextAsset>(assetBundleName, assetName);
            if (asset == null) return null;

            return asset.bytes;
        }
        
        /// <summary>
        /// 加载二进制文件，用于sqlite数据文件
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async Task<byte[]> LoadBytesAsync(string assetBundleName, string assetName)
        {
            var asset = await LoadAssetAsync<TextAsset>(assetBundleName, assetName);
            if (asset == null) return null;

            return asset.bytes;
        }

        /// <summary>
        /// 加载Prefab
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public GameObject LoadPrefab(string assetBundleName, string assetName)
        {
            var asset = LoadAsset<GameObject>(assetBundleName, assetName);
            if (asset == null) return null;

            return asset;
        }
        
        /// <summary>
        /// 加载Prefab
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async Task<GameObject> LoadPrefabAsync(string assetBundleName, string assetName)
        {
            var asset = await LoadAssetAsync<GameObject>(assetBundleName, assetName);
            if (asset == null) return null;

            return asset;
        }

        public ScriptableObject LoadScriptableObject(string assetBundlename, string assetName)
        {
            var asset = LoadAsset<ScriptableObject>(assetBundlename, assetName);
            if (asset == null) return null;

            return asset;
        }
        
        public async Task<ScriptableObject> LoadScriptableObjectAsync(string assetBundlename, string assetName)
        {
            var asset = await LoadAssetAsync<ScriptableObject>(assetBundlename, assetName);
            if (asset == null) return null;

            return asset;
        }

        public Sprite LoadSprite(string assetBundleName, string assetName)
        {
            var asset = LoadAsset<Sprite>(assetBundleName, assetName);
            if (asset != null) return asset;

            var texture = LoadAsset<Texture2D>(assetBundleName, assetName);
            if (texture != null)
            {
                var sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                sp.name = assetName;
                return sp;
            }

            return null;
        }
        
        public async Task<Sprite> LoadSpriteAsync(string assetBundleName, string assetName)
        {
            var asset = await LoadAssetAsync<Sprite>(assetBundleName, assetName);
            if (asset != null) return asset;

            var texture = await LoadAssetAsync<Texture2D>(assetBundleName, assetName);
            if (texture != null)
            {
                var sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                sp.name = assetName;
                return sp;
            }

            return null;
        }

        public AnimationClip LoadAnimationClip(string assetBundleName, string assetName)
        {
            var asset = LoadAsset<AnimationClip>(assetBundleName, assetName);
            if (asset == null) return null;

            return asset;
        }

        public void ShowLoadedAssetBundles()
        {
            var bundles = AssetBundle.GetAllLoadedAssetBundles();
            foreach (var bundle in bundles)
            {
                Debug($"Loaded AssetBundle: {bundle}");
            }
        }
        
        #region Log

        public static void Debug(string fmt, params object[] args)
        {
            var log = LogString(fmt, args);
            UnityEngine.Debug.Log(log);
        }


        public static void Error(string fmt, params object[] args)
        {
            var log = LogString(fmt, args);
            UnityEngine.Debug.LogError(log);
        }
        
        public static void Warning(string fmt, params object[] args)
        {
            var log = LogString(fmt, args);
            UnityEngine.Debug.LogWarning(log);
        }

        private static string LogString(string fmt, params object[] args)
        {
            var sb = new StringBuilder();
            sb.Append(Time())
                .Append(Prefix())
                .Append(string.Format(fmt, args));
            return sb.ToString();
        }

        private static string Time()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return $"[{UnityEngine.Time.time:F3}]";
#else
            return $"[<color=green>{UnityEngine.Time.time:F3}</color>]";
#endif
        }

        private static string Prefix()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return $"[AssetManager] ";
#else
            return $"<color={Color.blue.ToLogColor()}>[AssetManager]</color> ";
#endif
        }

        #endregion
    }

    internal static class VersionPathExtension
    {
        internal static string ToPath(this RuntimePlatform self)
        {
            switch (self)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "StandaloneOSXUniversal";
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    return "StandaloneLinux";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.PS4:
                    return "PS4";
                case RuntimePlatform.XboxOne:
                    return "XboxOne";
                case RuntimePlatform.tvOS:
                    return "TvOS";
                case RuntimePlatform.Switch:
                    return "Switch";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "StandaloneWindows";
                default:
                    return "StandaloneWindows";
            }
        }

        internal static string StreamingAssetsPath(this string name)
        {
            return Path.Combine(Application.streamingAssetsPath, name);
        }

        internal static string PersistentAssetsPath(this string name)
        {
            return Path.Combine(Application.persistentDataPath, name);
        }
    }

    static class LogColorExtension
    {
        public static string ToLogColor(this Color color)
        {
            return $"#{(byte) (color.r * 255f):X2}{(byte) (color.g * 255f):X2}{(byte) (color.b * 255f):X2}";
        }
    }
}