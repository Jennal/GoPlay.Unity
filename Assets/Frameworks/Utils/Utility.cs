using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GoPlay.Utils
{

    public static class StaticUtils
    {
        public static string GetStreamingAssetPath () {
            string filePath =
#if UNITY_ANDROID && !UNITY_EDITOR
               "jar:file://" + Application.dataPath + "!/assets/";
               
#elif UNITY_IPHONE && !UNITY_EDITOR
               "file://" + Application.dataPath +"/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
               "file://" + Application.dataPath + "/StreamingAssets/";
#else
               string.Empty;  
#endif
            return filePath;
        }

        /// <summary>
        /// 获取数组中的下一个index
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>为负数表示结果非法</returns>
        public static int GetNextIndex<T> (T[] arr, int index) {
            if (arr == null) return -1;
            int len = arr.Length;
            if (len <= 0) return -1;
            if (index < 0 || index >= len) return -1;
            int ret = index + 1;
            if (ret >= len) {
                return 0;
            } else {
                return ret;
            }
        }

        /// <summary>
        /// 获取数组中的上一个index
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>为负数表示结果非法</returns>
        public static int GetPreIndex<T> (T[] arr, int index) {
            if (arr == null) return -1;
            int len = arr.Length;
            if (len <= 0) return -1;
            if (index < 0 || index >= len) return -1;
            int ret = index - 1;
            if (ret < 0) {
                return len - 1;
            } else {
                return ret;
            }
        }


        #region Debug
//#if UNITY_EDITOR
        /// <summary>
		/// 在屏幕上显示文字
		/// GUI方法 : 只能在MonoBehaviour派生类的OnGUI方法中使用
		/// </summary>
		/// <param name="screenPos">Screen position.</param>
		/// <param name="content">Content.</param>
		/// <param name="fontSize">Font size.</param>
		public static void GUIText(Vector2 screenPos, string content, int fontSize = 50)
        {
            GUI.skin.label.fontSize = fontSize;
            // 注意GUI中的y轴坐标是与屏幕坐标相反的
            var guiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
            GUI.Label(new Rect(guiPos.x, guiPos.y, Screen.width, Screen.height), content);
        }

//#endif


        


        /// <summary>
        /// 利用序列化方式复制一个对象
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CloneObject<T>(T obj) {
            return JsonUtility.FromJson<T>(JsonUtility.ToJson (obj));
        }
        #endregion // Debug


        #region 字符串
        /// <summary>
        /// 剪去行尾的注释
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimEndComments(string s) {
            var commentIndex = s.LastIndexOf("//", StringComparison.Ordinal);
            if (commentIndex >= 0) {
                s = s.Substring(0, commentIndex);
            }

            return s.TrimEnd(' ', '\t');
        }
        /// <summary>
        /// string 转bype数组
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="s">S.</param>
        public static byte[] String2Bytes(string s) {
            var byteArray = System.Text.Encoding.Default.GetBytes(s);
            return byteArray;
        }

        /// <summary>
        /// byte 数组转string
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="bytes">Bytes.</param>
        public static string Bytes2String (byte[] bytes) {
            string str = System.Text.Encoding.Default.GetString(bytes);
            return str;
        }
        
        public static bool IsNumber(char c)
        {
            return (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' ||
                    c == '5' || c == '6' || c == '7' || c == '8' || c == '9');
        }
        /// <summary>
        /// 检查string
        /// </summary>
        /// <returns><c>true</c>, if string valid was checked, <c>false</c> otherwise.</returns>
        /// <param name="str">String.</param>
        public static bool CheckStringValid (string str) {
            return ! string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 从文件中读取json
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadJsonFromFile(string path)
        {
            // not available for android
            if (!File.Exists(path)) {
                Debug.LogError("LoadJsonFromFile file not exist  " + path);
                return "";
            }

            var sr = new StreamReader(path);

            string json = sr.ReadToEnd();
            //Debug.Log( "json content : " + json);

            sr.Dispose();
            return json;
        }

        /// <summary>
        /// 从字符串解析Color
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color ParseColor (string str) {
            var fields = ParseCommaString2Int(str);
            int r = fields.Count > 0 ? fields[0] : 255;
            int g = fields.Count > 1 ? fields[1] : 255;
            int b = fields.Count > 2 ? fields[2] : 255;
            int a = fields.Count > 3 ? fields[3] : 255;
            return NewColor(r, g, b, a);
        }
        /// <summary>
        /// 从整数色值创建Color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Color NewColor (int r, int g, int b, int a = 255) {
            return new Color (r / 255f, g / 255f, b / 255f, a / 255f);
        }
        /// <summary>
        /// 给string加上颜色信息
        /// </summary>
        /// <returns>string with color</returns>
        /// <param name="src">Source string</param>
        /// <param name="colorName">Color name, you can use color name in rich text , also can see vee.ColorName </param>
        public static string DecorateWithColor(this string src, string colorName)
        {
            return "<color=" + colorName + ">" + src + "</color>";
        }
        public static string DecorateWithColor(this string src, Color col)
        {
            var colorHex = "#" + ColorUtility.ToHtmlStringRGBA(col);
            return DecorateWithColor(src, colorHex);
        }
        
        /// <summary>
        /// 给string加上字体大小信息
        /// </summary>
        /// <returns>string with size.</returns>
        /// <param name="src">Source string</param>
        /// <param name="size">Size number</param>
        public static string DecorateWithSize(this string src, int size)
        {
            return "<size=" + size + ">" + src + "</size>";
        }

        /// <summary>
        /// 解析字符串为整型列表(逗号分隔)
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<int> ParseCommaString2Int(string raw/*, params char[] separator*/)
        {
            var retList = new List<int>();
            if (!CheckStringValid(raw)) return retList;
            var dataArr = raw.Split(',');
            int dataCnt = dataArr.Length;
            bool formatError = false;
            for (var i=0; i<dataCnt; ++i)
            {
                string data = dataArr[i];
                if (int.TryParse(data, out var v)) {
                    retList.Add(v);
                }
                else {
                    formatError = true;
                    retList.Add(int.MaxValue);
                }
            }

#if UNITY_EDITOR
            if (formatError) {
                Debug.LogWarning($"ParseCommaString2Int : Format Error for [{raw}]");
            }
#endif       
            
            return retList;
        }
        /// <summary>
        /// 解析字符串为浮点列表(逗号分隔)
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<float> ParseCommaString2Float(string raw/*, params char[] separator*/)
        {
            var retList = new List<float>();
            if (!CheckStringValid(raw)) return retList;
            var dataArr = raw.Split(',');
            int dataCnt = dataArr.Length;

            bool formatError = false;
            for (var i = 0; i < dataCnt; ++i)
            {
                string data = dataArr[i];
                if (float.TryParse(data, out float v)) {
                    retList.Add(v);
                }
                else {
                    formatError = true;
                    retList.Add(float.MaxValue);
                }
            }

#if UNITY_EDITOR
            if (formatError) {
                Debug.LogWarning($"ParseCommaString2Float : Format Error for [{raw}]");
            }
#endif            
            
            return retList;
        }
        
        /// <summary>
        /// 从串中随机选择一个整数
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static int PickRamdomIntFromString (string dataSource) {
            if (string.IsNullOrEmpty(dataSource)) {
                throw new ArgumentNullException();
            }

            var randomList = ParseCommaString2Int (dataSource);
            return (randomList.Count > 0) ? RandomChoice<int> (randomList) : 0;
        }

        /// <summary>
        /// 查找string中所有匹配的子串index
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sub"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<int> FindAll(this string target, string sub, int startIndex = 0, int length = Int32.MaxValue) {
            var ret = new List<int>();
            var stringLength = target.Length;
            var subLength = sub.Length;
            if (stringLength <= 0 || subLength <= 0) return ret;
            if (startIndex >= stringLength) {
                startIndex = stringLength - 1;
            }

            var checkLength = length >= (stringLength - startIndex) ? stringLength : length;
            while (true) {
                var index = target.IndexOf(sub, startIndex, checkLength, StringComparison.Ordinal);
                if (index < 0) {
                    break;
                }
                else {
                    ret.Add(index);
                    checkLength -= (index - startIndex) + subLength;
                    startIndex = index + subLength;
                }
            }

            return ret;
        }
        #endregion // 字符串
        

        #region Random
        /// <summary>
        /// 获取区间内的随机整数
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public static int RandomInt(int start, int end)
        {
            int len = end - start;
            return (int)(Mathf.Floor(UnityEngine.Random.value * (len + 1)) + start);
        }

        /// <summary>
        /// 获取区间内随机浮点数， make sure end bigger than start
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public static float RandomFloat(float start, float end)
        {
            return start + UnityEngine.Random.value * (end - start);
        }

        /// <summary>
        /// 随机选择一个list中的元素
        /// </summary>
        /// <returns>The choice.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T RandomChoice<T>(List<T> list)
        {
            if (list == null || list.Count <= 0) return default(T);
            var idx = RandomInt(0, list.Count - 1);
            return list[idx];
        }

        /// <summary>
        /// 随机打乱list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void ShuffleList<T>(List<T> list)
        {
            list.Sort((left, right) => RandomInt(-1, 1));
        }


        /// <summary>
        /// 添加一个元素到list，并且确保不重复添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="ele"></param>
        public static void AddWithoutRepeat<T>(List<T> list, T ele)
        {
            if (list == null) return;
            if (!list.Contains(ele))
            {
                list.Add(ele);
            }
        }

        /// <summary>
        /// 用默认值填充list到指定的数量
        /// </summary>
        /// <param name="list"></param>
        /// <param name="toCount"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void FillList <T> (List<T> list, int toCount) {
            if (list == null) return;
            if (list.Count < toCount) {
                for (var i=list.Count; i<toCount; ++i) {
                    list.Add(default(T));
                }
            }
        }

        public static void ForIncrease(int count, Action<int> action, int startIndex = 0) {
            int endIndex = startIndex + count;
            for (var i = startIndex; i < endIndex; ++i) {
                action(i);
            }
        }
        /// <summary>
        /// 遍历array，对每个元素调用action
        /// </summary>
        /// <param name="array"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEachInArray<T> (T[] array, Action<T, int> action) {
            if (array == null) return;
            var list = new List<T>();
            list.AddRange(array);
            ForEachInList(list, action);
        }

        /// <summary>
        /// 遍历list，对每个元素调用action
        /// </summary>
        /// <param name="list"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEachInList<T> (List<T> list, Action<T, int> action) {
            if (list == null) return;
            int eleCount = list.Count;
            for (var i=0; i<eleCount; ++i) {
                action(list[i], i);
            }
        }

        /// <summary>
        /// 遍历list，并删除符合条件的元素们
        /// </summary>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <typeparam name="T">返回true则删除，否则跳过</typeparam>
        /// <returns></returns>
        public static int RemoveInList<T>(List<T> list, Predicate<T> match) {
            int removeCount = 0;
            if (list == null) return removeCount;
            int eleCount = list.Count;
            for (var i=eleCount-1; i>=0; --i) {
                if (match(list[i])) {
                    list.RemoveAt(i);
                    removeCount += 1;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// 对每个transform的child调用action
        /// </summary>
        /// <param name="root"></param>
        /// <param name="action"></param>
        public static void ForEachChild (this Transform root, Action<Transform, int> action) {
            if (root == null) return;
            int childCnt = root.childCount;
            for (var i=0; i<childCnt; ++i) {
                var child = root.GetChild(i);
                action(child, i);
            }
        }

        public static int RemoveChild(this Transform root, Predicate<Transform> match) {
            int removeCount = 0;
            if (root == null) return removeCount;
            int childCnt = root.childCount;
            for (var i=childCnt-1; i>=0; --i) {
                var child = root.GetChild(i);
                if (match(child)) {
                    var go = child.gameObject;
                    DestroyGameObject(ref go, true);
                    removeCount += 1;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// 遍历dictionary，对每个元素调用action
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="action"></param>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static void ForEachInDictionary<K, V> (IDictionary<K, V> dic, Action<K, V> action) {
            if (dic == null) return;
            foreach(var kp in dic) {
                action(kp.Key, kp.Value);
            }
        }

        /// <summary>
        /// 获取反序list
        /// </summary>
        /// <param name="l"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Invert<T>(this List<T> l) {
            var ret = new List<T>();
            var count = l.Count;
            for (var i = count - 1; i >= 0; --i) {
                ret.Add(l[i]);
            }
            return ret;
        }
        
        public static List<T> Invert<T>(this T[] a) {
            var ret = new List<T>();
            var count = a.Length;
            for (var i = count - 1; i >= 0; --i) {
                ret.Add(a[i]);
            }
            return ret;
        }
        
        /// <summary>
        /// 弹出列表第一个元素
        /// </summary>
        /// <param name="l"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PopFirst<T>(this List<T> l)
        {
            return l.Pop(0);
        }
        
        /// <summary>
        /// 弹出列表最后一个元素
        /// </summary>
        /// <param name="l"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PopLast<T>(this List<T> l)
        {
            return l.Pop();
        }
        
        /// <summary>
        /// 取出list指定位置的一个元素，并返回
        /// </summary>
        /// <param name="l">目标list</param>
        /// <param name="index">指定的下标</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>返回取出的元素</returns>
        public static T Pop<T>(this List<T> l, int index = -1)
        {
            if (l == null || l.Count <= 0)
            {
                return default(T);
            }
            if (index < 0) index = l.Count - 1;
            
            if (index >= l.Count)
            {
                return default(T);
            }

            var t = l[index];
            l.RemoveAt(index);
            return t;
        }

        public static List<T> PopRange<T>(this List<T> list, int count)
        {
            if (!list.Any()) return null;

            count = Mathf.Min(list.Count, count);
            var range = list.Take(count).ToList();
            list.RemoveRange(range);

            return range;
        }
        #endregion //Ramdom


        #region 游戏场景物体
        // Set Position ...
        public static void SetWorldX(this Transform tran, float x) {
            var position = tran.position;
            position = new Vector3(x, position.y, position.z);
            tran.position = position;
        }
        public static void SetWorldY(this Transform tran, float y) {
            var position = tran.position;
            position = new Vector3(position.x, y, position.z);
            tran.position = position;
        }
        public static void SetWorldZ(this Transform tran, float z) {
            var position = tran.position;
            position = new Vector3(position.x, position.y, z);
            tran.position = position;
        }
        public static void SetWorldXY(this Transform tran, float x, float y)
        {
            tran.position = new Vector3(x, y, tran.position.z);
        }
        public static void SetWorldXY(this Transform tran, Vector2 vec2)
        {
            tran.position = new Vector3(vec2.x, vec2.y, tran.position.z);
        }
        // Set Local Position...
        public static void SetLocalX(this Transform tran, float x) {
            var localPosition = tran.localPosition;
            localPosition = new Vector3(x, localPosition.y, localPosition.z);
            tran.localPosition = localPosition;
        }
        public static void SetLocalY(this Transform tran, float y) {
            var localPosition = tran.localPosition;
            localPosition = new Vector3(localPosition.x, y, localPosition.z);
            tran.localPosition = localPosition;
        }
        public static void SetLocalZ(this Transform tran, float z) {
            var localPosition = tran.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y, z);
            tran.localPosition = localPosition;
        }
        public static void SetLocalXY(this Transform tran, float x, float y)
        {
            tran.localPosition = new Vector3(x, y, tran.localPosition.z);
        }
        public static void SetLocalXY(this Transform tran, Vector2 vec2)
        {
            tran.localPosition = new Vector3(vec2.x, vec2.y, tran.localPosition.z);
        }

        // Set Rotation...
        public static void SetRotationEulerX(this Transform tran, float x)
        {
            var rotEuler = tran.rotation.eulerAngles;
            tran.rotation = Quaternion.Euler(x, rotEuler.y, rotEuler.z);
        }
        public static void SetRotationEulerY(this Transform tran, float y)
        {
            var rotEuler = tran.rotation.eulerAngles;
            tran.rotation = Quaternion.Euler(rotEuler.x, y, rotEuler.z);
        }
        public static void SetRotationEulerZ(this Transform tran, float z)
        {
            var rotEuler = tran.rotation.eulerAngles;
            tran.rotation = Quaternion.Euler(rotEuler.x, rotEuler.y, z);
        }
        // Set Local Rotation...
        public static void SetLocalRotationEulerX(this Transform tran, float x)
        {
            var rotEuler = tran.localRotation.eulerAngles;
            tran.localRotation = Quaternion.Euler(x, rotEuler.y, rotEuler.z);
        }
        public static void SetLocalRotationEulerY(this Transform tran, float y)
        {
            var rotEuler = tran.localRotation.eulerAngles;
            tran.localRotation = Quaternion.Euler(rotEuler.x, y, rotEuler.z);
        }
        public static void SetLocalRotationEulerZ(this Transform tran, float z)
        {
            var rotEuler = tran.localRotation.eulerAngles;
            tran.localRotation = Quaternion.Euler(rotEuler.x, rotEuler.y, z);
        }
        // Set local scale...
        public static void SetLocalScaleX(this Transform tran, float x) {
            var localScale = tran.localScale;
            localScale = new Vector3(x, localScale.y, localScale.z);
            tran.localScale = localScale;
        }
        public static void SetLocalScaleY(this Transform tran, float y) {
            var localScale = tran.localScale;
            localScale = new Vector3(localScale.x, y, localScale.z);
            tran.localScale = localScale;
        }
        public static void SetLocalScaleXY(this Transform tran, float x, float y)
        {
            tran.localScale = new Vector3(x, y, tran.localScale.z);
        }
        public static void SetLocalScaleXY(this Transform tran, Vector2 vec2)
        {
            tran.localScale = new Vector3(vec2.x, vec2.y, tran.localScale.z);
        }

        public static void FlipX (this Transform tran) {
            SetLocalScaleX(tran, - tran.localScale.x);
        }

        public static void FlipY (this Transform tran) {
            SetLocalScaleY(tran, - tran.localScale.y);
        }

        public static void EnsureScaleXPositive (this Transform tran) {
            float nowScale = tran.lossyScale.x;
            if (nowScale < 0) {
                FlipX(tran);
            }
        }

        public static void EnsureScaleYPositive (this Transform tran) {
            float nowScale = tran.lossyScale.y;
            if (nowScale < 0) {
                FlipY(tran);
            }
        }


        /// <summary>
        /// 重新激活GameObject，可用于动画播放
        /// </summary>
        /// <param name="go"></param>
        public static void ReActive(this GameObject go)
        {
            go.SetActive(false);
            go.SetActive(true);
        }

        /// <summary>
        /// 复制对象（包含位置）
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObject CloneGameObject(GameObject go)
        {
            var newGo = Object.Instantiate(go, Vector3.zero, Quaternion.identity);
            if (go.transform.parent != null)
            {
                newGo.transform.SetParent(go.transform.parent);
            }

            newGo.transform.localPosition = go.transform.localPosition;
            newGo.transform.localRotation = go.transform.localRotation;
            newGo.transform.localScale = go.transform.localScale;
            return newGo;
        }

        /// <summary>
        /// 在指定位置创建空对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="lPos"></param>
        /// <param name="lRot"></param>
        /// <param name="lScale"></param>
        /// <returns></returns>
        public static GameObject CreateGameObject(string name, Transform parent,
            Vector3 lPos, Vector3 lRot, Vector3 lScale)
        {
            var newObj = new GameObject(name);
            if (parent != null)
            {
                newObj.transform.SetParent(parent);
            }

            newObj.transform.localPosition = lPos;
            newObj.transform.localRotation = Quaternion.Euler(lRot.x, lRot.y, lRot.z);
            newObj.transform.localScale = lScale;

            return newObj;
        }

        public static GameObject CreateGameObject(string name, Transform parent)
        {
            return CreateGameObject(name, parent, Vector3.zero, Vector3.zero, Vector3.one);
        }

        /// <summary>
        /// 复制一个prefab并放在一个物体下面，初始化transform
        /// </summary>
        /// <param name="template"></param>
        /// <param name="parent"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static GameObject CloneGameObjectAsChild(GameObject template, Transform parent, string newName = "")
        {
            if (template == null) {
                Debug.LogWarning("CloneGameObjectAsChild Template is null");
                return null;
            }

            var newGo = Object.Instantiate<GameObject>(template);
            if (newGo != null) {
                if ( ! string.IsNullOrEmpty(newName)) {
                    newGo.name = newName;
                }

                SetParent(newGo.transform, parent, true);
            }

            return newGo;
        }

        /// <summary>
        /// 为一个物体设置父物体
        /// </summary>
        /// <param name="child">子物体</param>
        /// <param name="parent">父物体</param>
        /// <param name="reset">是否重置transform</param>
        public static void SetParent (Transform child, Transform parent, bool reset = false ) {
            if (child == null || parent == null) return;
            child.SetParent(parent);
            if (reset) {
                child.localScale = Vector3.one;
                child.localRotation = Quaternion.identity;
                child.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 将一个层级中的物体替换为另一个
        /// </summary>
        /// <param name="oldObj">原来的物体</param>
        /// <param name="newObj">新物体</param>
        /// <param name="immediate">是否立即删除旧物体</param>
        /// <param name="useOldTransform">是否使用旧物体的transform信息</param>
        public static void ReplaceGameObject(GameObject oldObj, GameObject newObj, 
            bool immediate = false, bool useOldTransform = true)
        {
            var index = oldObj.transform.GetSiblingIndex();
            newObj.transform.SetParent(oldObj.transform.parent);
            newObj.transform.SetSiblingIndex(index);
            if (useOldTransform)
            {
                newObj.transform.localPosition = oldObj.transform.localPosition;
                newObj.transform.localRotation = oldObj.transform.localRotation;
                newObj.transform.localScale = oldObj.transform.localScale;
            }

            if (immediate)
            {
                Object.DestroyImmediate(oldObj);
            }
            else
            {
                Object.Destroy(oldObj);
            }
        }

        /// <summary>
        /// 销毁物体,并置引用为null
        /// </summary>
        /// <param name="go"></param>
        /// <param name="immediate">是否立即删除</param>
        public static void DestroyGameObject(ref GameObject go, bool immediate = false)
        {
//            if (go == null) return;
            go.SetActive(false);
            if (immediate)
            {
                Object.DestroyImmediate(go);
            }
            else
            {
                SetParent(go.transform, null);
                Object.Destroy(go);
            }
            go = null;
        }

        public static void DestroyGameObject (this MonoBehaviour mono, bool immediate = false) {
            if (mono != null) {
                var go = mono.gameObject;
                DestroyGameObject(ref go, immediate);
            }
        }

        public static void DestroyComponent(UnityEngine.Object com) {
            if (com != null) {
                UnityEngine.Object.DestroyImmediate(com);
            }
        }
        

        public static string GetGameObjectName (this MonoBehaviour mono) {
            if (mono != null) {
                var go = mono.gameObject;
                if (go != null) return go.name;
            }

            return "null";
        }
        /// <summary>
        /// 去掉 gameobject名字中的(Clone)字样
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteCloneInName (this GameObject obj)
        {
            obj.name = TrimCloneInString(obj.name);
        }

        /// <summary>
        /// 剪去string中的(Clone)字样
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimCloneInString (string s) {
            return s.Replace("(Clone)", "");
        }

        /// <summary>
        /// 查找Hierarchy顶层物体
        /// </summary>
        /// <returns>The top object.</returns>
        /// <param name="name">Name.</param>
        public static GameObject GetTopObject(string name)
        {
            var scene = SceneManager.GetActiveScene();
            var rootObjs = scene.GetRootGameObjects();
            return rootObjs.FirstOrDefault(obj => obj.name == name);
        }

#if UNITY_EDITOR
        public static GameObject FindGameObjectInScene(int instanceId) {
            var scene = SceneManager.GetActiveScene();
            var rootObjs = scene.GetRootGameObjects();
            foreach (var rootObj in rootObjs) {
                var target = rootObj.FindInChildren(instanceId);
                if (target != null) {
                    return target;
                }
            }

            return null;
        }

        public static GameObject FindInChildren(this GameObject go, int instanceId) {
            if (go.GetInstanceID() == instanceId) {
                return go;
            }
            else {
                var childCount = go.transform.childCount;
                for (var i = 0; i < childCount; ++i) {
                    var child = go.transform.GetChild(i);
                    var target = child.gameObject.FindInChildren(instanceId);
                    if (target != null) {
                        return target;
                    }
                }

                return null;
            }
        }

        public static string GetHierarchy(this GameObject go) {
            string ret = go.name;
            var parent = go.transform.parent;
            while (parent != null) {
                ret = parent.gameObject.name + "--" + ret;
                parent = parent.parent;
            }

            return ret;
        }
#endif

        /// <summary>
        /// 删除一个物体下面的所有子物体(不包含自身)
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="immediate"></param>
        public static void RemoveAllChildren(this Transform node, bool immediate = false)
        {
//            if (node == null) return;
            
            var childCount = node.childCount;
            for (var i = childCount - 1; i >= 0; --i)
            {
                var child = node.GetChild(i).gameObject;
                DestroyGameObject(ref child, immediate);

//#if UNITY_EDITOR
//                if (Application.isPlaying)
//                {
//                    Object.Destroy(child.gameObject);
//                }
//                else
//                {
//                    Object.DestroyImmediate(child.gameObject);   
//                }
//#else
//                    GameObject.Destroy(child.gameObject);
//#endif
            }
        }
        /// <summary>
        /// 隐藏一个物体下的所有子物体(不包含自身)
        /// </summary>
        /// <param name="obj"></param>
        public static void HideAllChildren(this Transform obj)
        {
            var childCount = obj.childCount;
            for (var i = 0; i < childCount; ++i)
            {
                var child = obj.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 获取所有子物体的transform
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<Transform> GetAllChildren(this Transform obj) {
            var ret = new List<Transform>();
            var childCount = obj.childCount;
            for (var i = 0; i < childCount; ++i)
            {
                var child = obj.GetChild(i);
                ret.Add(child);
            }

            return ret;
        }

        /// <summary>
        /// 获取第一个子物体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Transform GetFirstChild(this Transform obj) {
            return obj.childCount > 0 ? obj.GetChild(0) : null;
        }

        /// <summary>
        /// 获取最后一个子物体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Transform GetLastChild(this Transform obj) {
            var childCount = obj.childCount;
            return childCount > 0 ? obj.GetChild(childCount - 1) : null;
        }

        /// <summary>
        /// 为一个物体设置layer
        /// </summary>
        /// <param name="go">物体</param>
        /// <param name="ly">层</param>
        /// <param name="setChildren">是否设定所有子物体的层级</param>
        public static void SetLayer(this GameObject go, int ly, bool setChildren = true)
        {
            go.layer = ly;
            if (setChildren)
            {
                var nodes = go.GetComponentsInChildren<Transform>();
                foreach (var node in nodes)
                {
                    node.gameObject.layer = ly;
                }
            }
        }

        /// <summary>
        /// 查找符合条件的子物体
        /// </summary>
        /// <param name="root">root transform</param>
        /// <param name="match">条件</param>
        /// <param name="recursive">是否递归向下层查找</param>
        /// <returns></returns>
        public static List<Transform> FindChildren(this Transform root, Predicate<Transform> match, bool recursive = false) {
            var result = new List<Transform>();
            var childCount = root.childCount;
            for (var i=0; i<childCount; ++i) {
                var child = root.GetChild(i);
                if (match(child)) {
                    result.Add(child);
                }

                if (recursive) {
                    var childMatches = FindChildren(child, match, true);
                    result.AddRange(childMatches.ToArray());
                }
            }

            return result;
        }

        #endregion // 游戏场景物体

        #region  Component
        /// <summary>
        /// 获取组件，如果没有就添加
        /// </summary>
        /// <param name="go"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var com = go.GetComponent<T>();
            if (com == null) com = go.AddComponent<T>();
            return com;
        }
        
        public static T GetOrAddComponent<T>(this Transform node) where T : Component
        {
            var com = node.GetComponent<T>();
            if (com == null) com = node.gameObject.AddComponent<T>();
            return com;
        }

        /// <summary>
        /// 在子物体或者自己上查找指定类型的第一个组件
        /// </summary>
        /// <param name="go"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindComponentInChildren<T>(this GameObject go) where T : Component {
            var coms = go.GetComponentsInChildren<T>(true);
            return coms.Length > 0 ? coms[0] : null;
        }

        /// <summary>
        /// 在父物体中寻找组件，不包含自身
        /// </summary>
        /// <param name="node"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindComponentInParents<T>(this Transform node) where T : Component {
            var nowNode = node;
            T targetCom = null;
            while (nowNode != null && nowNode.parent != null) {
                nowNode = nowNode.parent;
                var com = nowNode.GetComponent<T>();
                if (com != null) {
                    targetCom = com;
                    break;
                }
            }

            return targetCom;
        }

        /// <summary>
        /// 获取子物体上所有指定类型的组件(不包含自己)
        /// </summary>
        /// <param name="go"></param>
        /// <param name="includeInactive"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetComponentsInChildrenWithoutSelf<T>(this GameObject go, bool includeInactive) where T : Component {
            var all = go.GetComponentsInChildren<T>(includeInactive);
            return all.Where(c => c.gameObject != go).ToArray();
        }
        
        public static T[] GetComponentsInChildrenWithoutSelf<T>(this Transform tran, bool includeInactive) where T : Component {
            var all = tran.GetComponentsInChildren<T>(includeInactive);
            return all.Where(c => c.transform != tran).ToArray();
        }
        #endregion // Component


        #region Misc
        public static Camera MainCamera {
            get {
                var cam = Camera.main;
#if UNITY_EDITOR
                if (cam == null) {
                    Debug.LogWarning($"No Main Camera in scene");
                }
#endif
                return cam; 
            }
        }
        
        /// <summary>
        /// this is used for clear Warning  CS0414 : assigned but value is never used
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void Use(this object obj)
        {
            // Do Nothing	
        }
        
        /// <summary>
        /// 通过SortGroup设置层级
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layerName"></param>
        /// <param name="order"></param>
        public static void SetGroupSortOrder(this GameObject obj, string layerName, int order) {
            var group = obj.GetOrAddComponent<SortingGroup>();
            group.sortingLayerName = layerName;
            group.sortingOrder = order;
        }

        public static void SetTimeScale(float val)
        {
            Time.timeScale = val;
        }

        public static void ResetTimeScale()
        {
            SetTimeScale(1f);
        }
        
        public static bool InTimePause() {
            return Time.timeScale < 0.8f;
        }
        
        public static Vector3 GetZeroZ(this Vector3 vec3)
        {
            return new Vector3(vec3.x, vec3.y, 0f);
        }
        
        #endregion //Misc
    }

}