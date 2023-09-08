using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace GoPlay.AssetManagement
{
    public class StreamingAssetReader
    {
        /// <summary>
        /// 把文件从StreamingAsset拷贝出来
        /// </summary>
        /// <param name="src">原始路径，必须位于StreamingAsetsPath</param>
        /// <param name="dst">目标路径，必须可写</param>
        /// <returns></returns>
        public static IEnumerator Copy(string src, string dst)
        {
            if (!src.StartsWith(Application.streamingAssetsPath)) throw new Exception("wrong usage!");

            byte[] data;
            if (src.Contains("://"))
            {
                var rq = UnityWebRequest.Get(src);
                yield return rq.SendWebRequest();
                data = rq.downloadHandler.data;
            }
            else
            {
                data = File.ReadAllBytes(src);
            }

            var dir = Path.GetDirectoryName(dst);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllBytes(dst, data);
        }

        /// <summary>
        /// 把文件从StreamingAsset读取出来
        /// </summary>
        /// <param name="path">路径，必须位于StreamingAsetsPath</param>
        /// <param name="hash">版本hash</param>
        /// <returns></returns>
        public static AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string path)
        {
            if (!path.StartsWith(Application.streamingAssetsPath)) throw new Exception("wrong usage!");
            var rq = AssetBundle.LoadFromFileAsync(path);
            return rq;
        }

        /// <summary>
        /// 把文件从StreamingAsset读取出来
        /// </summary>
        /// <param name="path">路径，必须位于StreamingAsetsPath</param>
        /// <param name="hash">版本hash</param>
        /// <returns></returns>
        public static UnityWebRequestAsyncOperation LoadAssetBundleAsyncOperation(string path, Hash128 hash)
        {
            if (!path.StartsWith(Application.streamingAssetsPath)) throw new Exception("wrong usage!");

            //Logger.Debug("Load from local: {0}", path);
            if (!path.StartsWith("jar:file//") && !path.Contains("://")) path = "file://" + path;
            var rq = UnityWebRequestAssetBundle.GetAssetBundle(path, hash, 0U);
            return rq.SendWebRequest();
        }

        /// <summary>
        /// 把文件从StreamingAsset读取出来
        /// </summary>
        /// <param name="path">路径，必须位于StreamingAsetsPath</param>
        /// <param name="hash">版本hash</param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundleAsyncOperationSync(string path, Hash128 hash)
        {
            if (!path.StartsWith(Application.streamingAssetsPath)) throw new Exception("wrong usage!");

            //Logger.Debug("Load from local: {0}", path);
            if (!path.StartsWith("jar:file//") && !path.Contains("://")) path = "file://" + path;
            var rq = UnityWebRequestAssetBundle.GetAssetBundle(path, hash, 0U);
            var op = rq.SendWebRequest();
            while (!op.isDone) Thread.Sleep(1);
            return DownloadHandlerAssetBundle.GetContent(op.webRequest);
        }
    }
}
