using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GoPlay.Editor {
    public class EditorUtils {
        #region Editor Listeners
        [UnityEditor.Callbacks.DidReloadScripts]
        static void onEditorReloadScripts () {
            // Debug.Log ("onEditorReloadScripts");
        }
        #endregion //Editor Listeners

        public static bool IsScript(string assetPath)
        {
            return assetPath.EndsWith(".cs");
        }

        public static bool IsScene(string assetPath)
        {
            return assetPath.EndsWith(".unity");
        }
        
        public static bool IsPrefab(string assetPath)
        {
            return assetPath.EndsWith(".prefab");
        }
        
        /// <summary>
        /// 在project中精确查找匹配给定名字的资源, 过滤掉部分匹配的情况
        /// </summary>
        /// <param name="fileName">文件全名，不包含扩展名</param>
        /// <param name="filter">可以指定类型如 t:texture2d</param>
        /// <param name="searchInFolders">搜索的文件夹</param>
        /// <returns>返回资源的guid</returns>
        public static List<string> FindAssetsExactly(string fileName, string filter, string[] searchInFolders) {
            var searchFilter = string.Format($"{fileName} {filter}");
            var guids = AssetDatabase.FindAssets(searchFilter, searchInFolders);

            var result = new List<string>();
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var assetFileName = Path.GetFileNameWithoutExtension(path);
                if (assetFileName == fileName) {
                    result.Add(guid);
                }
            }

            return result;
        }
        
        /// <summary>
        /// 在编辑器的project中高亮选中Asset文件
        /// </summary>
        /// <param name="path"></param>
        public static void SelectAssetAtPath(string path) {
            var assetObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (null != assetObj)
            {
                EditorGUIUtility.PingObject(assetObj);
            }
        }
        
        /// <summary>
        /// 获取editor选中对象列表
        /// </summary>
        /// <returns></returns>
        public static List<UnityEngine.Object> GetSelection () {
            UnityEngine.Object[] arr = Selection.GetFiltered (typeof (UnityEngine.Object), SelectionMode.TopLevel);
            int selectCount = arr.Length;

            if (selectCount <= 0) {
                Debug.LogWarning ("Nothing Selected !!");
                return new List<UnityEngine.Object> ();
            }

            List<UnityEngine.Object> retList = new List<UnityEngine.Object> ();
            retList.AddRange (arr);
            return retList;
        }

        /// <summary>
        /// 获取editor选中的GameObject对象列表
        /// </summary>
        /// <returns></returns>
        public static List<GameObject> GetSelectGameObjects () {
            var selection = GetSelection ();
            List<GameObject> retList = new List<GameObject> ();
            var selectCnt = selection.Count;
            for (var i = 0; i < selectCnt; ++i) {
                GameObject go = selection[i] as GameObject;
                if (go != null) {
                    retList.Add (go);
                } else {
                    Debug.LogWarning ("Select Object is NOT a GameObject : " + FileUtils.GetAssetFullPath (selection[i]));
                }
            }

            return retList;
        }

        public static string GetSelectFolder()
        {
            string tempSelectedFolder = "";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                tempSelectedFolder = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(tempSelectedFolder) && File.Exists(tempSelectedFolder))
                {
                    tempSelectedFolder = Path.GetDirectoryName(tempSelectedFolder);
                    break;
                }
            }

            return tempSelectedFolder;
        }
        /// <summary>
        /// 保存prefab
        /// </summary>
        /// <param name="go"></param>
        /// <param name="savePath"></param>
        public static void SavePrefab (GameObject go, string savePath) {
            savePath = FileUtils.GetProjectRelativePath(savePath);
            // check exist
            //var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject> (savePath);
            FileUtils.EnsureDirectoryOnPath(savePath);
            bool saveSuc = false;
            PrefabUtility.SaveAsPrefabAsset(go, savePath, out saveSuc);
            if (saveSuc)
            {
                Debug.Log("Save " + savePath + " Succeeded !!");
            }
            else
            {
                Debug.LogWarning("Save " + savePath + " Failed !!");
            }
        }

        /// <summary>
        /// 加载prefab
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab (string path) {
            path = FileUtils.GetProjectRelativePath(path);
            return (GameObject) AssetDatabase.LoadAssetAtPath (path, typeof (GameObject));
        }

        public static GameObject LoadPrefabByGuid(string guid) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return LoadPrefab(path);
        }


        public static T CreateAsset<T> (string path, bool save = true) where T : ScriptableObject {
            path = FileUtils.GetProjectRelativePath(path);
            T scriptableObj = ScriptableObject.CreateInstance<T> ();
            UnityEditor.AssetDatabase.CreateAsset (scriptableObj, path);
            if (save) {
                UnityEditor.AssetDatabase.SaveAssets ();
                UnityEditor.AssetDatabase.Refresh();
            }

            if (scriptableObj == null) {
                Debug.LogWarning("Create Asset Failed " + path);
            }
            return scriptableObj;
        }

        public static void DeleteAsset (string path, bool save = true) {
            path = FileUtils.GetProjectRelativePath(path);
            UnityEditor.AssetDatabase.DeleteAsset (path);
            if (save) {
                UnityEditor.AssetDatabase.SaveAssets ();
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        public static T LoadAsset<T> (string path) where T : UnityEngine.Object {
            path = FileUtils.GetProjectRelativePath(path);
            T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null) {
                Debug.LogWarning("Can't find Asset at " + path);
            }
            return obj;
        }

        public static void RevertProperty(MonoBehaviour obj, string propertyName) {
            var so = new SerializedObject(obj);
            var sp = so.FindProperty(propertyName);
            if (sp == null) {
                Debug.LogWarning($"[Editor] RevertProperty Faild because Serialized Property not found [object :{obj.name}, property : {propertyName}]");
                return;
            }
            
            PrefabUtility.RevertPropertyOverride(sp, InteractionMode.AutomatedAction);
            so.ApplyModifiedProperties();
        }

        public static List<string> FindCodeFiles(string fileNameWithoutExt, List<string> folders = null) {
            if (folders == null || folders.Count <= 0) folders = new List<string>() {"Assets/Game/Scripts"};
            var guids = FindAssetsExactly(fileNameWithoutExt, "t:Object", folders.ToArray());
            var ret = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToList();
            return ret;
        }
        
        /// <summary>
        /// 检测键盘事件：按下(Editor专用)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool GetKeyDown(KeyCode code)
        {
            var e = Event.current;
            if (e != null && e.isKey)
            {
                return (e.keyCode == code && e.type == EventType.KeyDown);
            }

            return false;
        }
        
        /// <summary>
        /// 检测键盘事件：抬起(Editor专用)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool GetKeyUp(KeyCode code)
        {
            var e = Event.current;
            if (e != null && e.isKey)
            {
                return (e.keyCode == code && e.type == EventType.KeyUp);
            }

            return false;
        }
    }
}

#endif