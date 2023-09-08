using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GoPlay {
    public class FileUtils {
        public static string DataPathPrefix {
            get {
                return Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ('/'));
            }
        }
        
        #region File
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir">需要创建的路径</param>
        public static void CreateDirectory (string dir) {
            if (!Directory.Exists (dir)) {
                Directory.CreateDirectory (dir);
            }
        }

        /// <summary>
        /// 确保文件夹存在
        /// </summary>
        /// <param name="dirPath">文件夹的路径</param>
        public static void EnsureDirectory (string dirPath) {
            if (!string.IsNullOrEmpty (dirPath)) {
                CreateDirectory (dirPath);
            }
        }
        
        /// <summary>
        /// 确保文件路径上的文件夹存在
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public static void EnsureDirectoryOnPath (string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var dirName = Path.GetDirectoryName (filePath);
            EnsureDirectory(dirName);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="bytes">文件内容</param>
        public static void CreatFile (string filePath, byte[] bytes) {
            EnsureDirectoryOnPath (filePath);
            FileInfo file = new FileInfo (filePath);
            Stream stream = file.Create ();

            stream.Write (bytes, 0, bytes.Length);
            stream.Flush ();
            stream.Close ();
            stream.Dispose ();
        }
        public static void CreatFile (string filePath, string txt) {
            EnsureDirectoryOnPath (filePath);
            File.WriteAllText(filePath, txt);
        }

        /// <summary>
        /// 删除目录以及下面所有的文件
        /// </summary>
        public static void DeleteDirectory (string dir, bool inUnity = false) {
            if (Directory.Exists (dir)) {
                if (inUnity) {
#if UNITY_EDITOR
                    string metaDir = dir + ".meta";
                    if (File.Exists(metaDir)) {
                        UnityEditor.FileUtil.DeleteFileOrDirectory(metaDir);
                    }

                    UnityEditor.FileUtil.DeleteFileOrDirectory(dir);
#else
                    Directory.Delete (dir, true);
#endif
                } else {
                    Directory.Delete (dir, true);
                }
            }
        }

        /// <summary>
        /// 把一个文件从项目中删除(同时删除其对应的meta文件)
        /// </summary>
        /// <param name="fileNameWithPath"></param>
        /// <param name="logMsg"></param>
        public static void DeleteFileFromProject (string fileNameWithPath, bool logMsg = false) {
            if (File.Exists (fileNameWithPath + ".meta")) {
                File.Delete (fileNameWithPath + ".meta");
                if (logMsg) {
                    Debug.Log ("The File : " + fileNameWithPath + ".meta" + " is DELETED ");
                }
            }
            if (File.Exists (fileNameWithPath)) {
                File.Delete (fileNameWithPath);
                if (logMsg) {
                    Debug.Log ("The File : " + fileNameWithPath + " is DELETED ");
                }
            }
        }

        /// <summary>
        /// 计算文件的md5值，返回大写格式
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GenerateFileMD5Upper (string url) {
            if (File.Exists (url) == false)
                return string.Empty;

            byte[] fileByte = File.ReadAllBytes (url);

            if (fileByte == null)
                return string.Empty;

            byte[] hashByte = new MD5CryptoServiceProvider ().ComputeHash (fileByte);

            return byteArrayToString (hashByte);
        }

        /// <summary>
        /// 输出数据的十六进制字符串
        /// </summary>
        /// <param name="arrInput"></param>
        /// <returns></returns>
        private static string byteArrayToString (byte[] arrInput) {
            StringBuilder sOutput = new StringBuilder (arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++) {
                sOutput.Append (arrInput[i].ToString ("X2"));
            }
            return sOutput.ToString ();
        }


        public static string GetAssetFullPath (UnityEngine.Object fileObj) {
#if UNITY_EDITOR
            return DataPathPrefix + "/" + AssetDatabase.GetAssetPath (fileObj);
#else
            return "";
#endif
        }

        public static void TrimFileNameExtention (ref string fileName) {
            var indexDot = fileName.LastIndexOf ('.');
            if (indexDot > 0) {
                fileName = fileName.Substring (0, indexDot);
            }
        }
        
        public static bool HasExtension(string file, IEnumerable<string> extensionGroup) {
            return extensionGroup.Any(e => file.EndsWith(e, System.StringComparison.OrdinalIgnoreCase));
        }
        
        public static string CombinePath (string path, string name) {
            return path + "/" + name;
        }

        public static string GetProjectRelativePath (string path) {
            var assetIndex = path.IndexOf ("Assets");
            if (assetIndex >= 0) {
                return path.Substring(assetIndex);
            }

            return CombinePath("Assets", path);
        }
        /// <summary>
        /// 保存texture到文件
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="path"></param>
        public static void SaveTextureToFile( Texture2D tex, string path) {
            var bytes=tex.EncodeToPNG();
            var fileStream = File.Create(path);
            var binary= new BinaryWriter(fileStream);
            binary.Write(bytes);
            fileStream.Close();
        }

#if UNITY_EDITOR
        public static T LoadAsset<T> (string path) where T : UnityEngine.Object {
            path = FileUtils.GetProjectRelativePath(path);
            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null) {
                Debug.LogWarning("Can't find Asset at " + path);
            }
            return obj;
        }
#endif
        #endregion // File

        #region Encrypt
        private static byte[] bytes = ASCIIEncoding.ASCII.GetBytes ("iw3q" + "5BE8"); //  SystemInfo.deviceUniqueIdentifier.Substring(0, 4)

        public static string Encrypt (string originalString) {
            if (String.IsNullOrEmpty (originalString)) {
                return "";
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider ();
            MemoryStream memoryStream = new MemoryStream ();
            CryptoStream cryptoStream = new CryptoStream (memoryStream, cryptoProvider.CreateEncryptor (bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter (cryptoStream);
            writer.Write (originalString);
            writer.Flush ();
            cryptoStream.FlushFinalBlock ();
            writer.Flush ();
            var base64 = Convert.ToBase64String (memoryStream.GetBuffer (), 0, (int) memoryStream.Length);

            //base64编码中有不能作为文件名的'/'符号，这里把它替换一下，增强适用范围
            return base64.Replace ('/', '@');
        }

        public static string Decrypt (string cryptedString) {
            if (String.IsNullOrEmpty (cryptedString)) {
                return "";
            }

            cryptedString = cryptedString.Replace ('@', '/');

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider ();
            MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (cryptedString));
            CryptoStream cryptoStream = new CryptoStream (memoryStream, cryptoProvider.CreateDecryptor (bytes, bytes), CryptoStreamMode.Read);
            StreamReader reader = new StreamReader (cryptoStream);
            return reader.ReadToEnd ();
        }

        #endregion //Encrypt
    }
}