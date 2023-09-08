// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// namespace GoPlay.Utils {
//     public static class UIUtils {
//         /// <summary>
//         /// bytes 转 Texture2D
//         /// </summary>
//         /// <param name="bytes"></param>
//         /// <param name="w"></param>
//         /// <param name="h"></param>
//         /// <returns></returns>
//         public static Texture2D ByteToTex2d(byte[] bytes, int w = 100, int h = 100)
//         {
//             Texture2D tex = new Texture2D(w, h);
//             tex.LoadImage(bytes);
//             return tex;
//         }
//
//         /// <summary>
//         /// 获取所在canvas
//         /// </summary>
//         /// <param name="node"></param>
//         /// <returns></returns>
//         public static Canvas GetBelongCanvas(this Transform node) {
//             var canvas = node.GetComponent<Canvas>();
//             if (canvas == null) {
//                 canvas = node.FindComponentInParents<Canvas>();
//             }
//
//             return canvas;
//         }
//
//         /// <summary>
//         /// 设置canvas的camera
//         /// </summary>
//         /// <param name="go"></param>
//         /// <param name="cam"></param>
//         /// <param name="setChildren">同时设置子canvas的camera</param>
//         public static void SetCameraForCanvas(GameObject go, Camera cam, bool setChildren = true) {
//             var can = go.transform.GetBelongCanvas();
//             if (can == null) {
//                 return;
//             }
//
//             can.worldCamera = cam;
//             
//             if (setChildren) {
//                 var cans = can.gameObject.GetComponentsInChildrenWithoutSelf<Canvas>(true);
//                 foreach (var childCan in cans) {
//                     childCan.worldCamera = cam;
//                 }
//             }
//         }
//
//
//         public static void SetAnchorPosX (this RectTransform tran, float x) {
//             tran.anchoredPosition = new Vector2 (x, tran.anchoredPosition.y);
//         }
//         public static void SetAnchorPosY (this RectTransform tran, float y) {
//             tran.anchoredPosition = new Vector2 (tran.anchoredPosition.x, y);
//         }
//         public static void SetAnchorPos (this RectTransform tran, float x, float y) {
//             tran.anchoredPosition = new Vector3 (x, y);
//         }
//         public static void SetAnchorPos (this RectTransform tran, Vector2 vec2) {
//             tran.anchoredPosition = vec2;
//         }
//         
//         public static float GetRightOffset (this RectTransform tran) {
//             return tran.offsetMax.x;
//         }
//         public static void SetRightOffset (this RectTransform tran, float off) {
//             tran.offsetMax = new Vector2(off, GetTopOffset(tran));
//         }
//         public static float GetLeftOffset (this RectTransform tran) {
//             return tran.offsetMin.x;
//         }
//         public static void SetLeftOffset (this RectTransform tran, float off) {
//             tran.offsetMin = new Vector2(off, GetBottomOffset(tran));
//         }
//         public static float GetTopOffset (this RectTransform tran) {
//             return tran.offsetMax.y;
//         }
//         public static void SetTopOffset (this RectTransform tran, float off) {
//             tran.offsetMax = new Vector2(GetRightOffset(tran), off);
//         }
//         public static float GetBottomOffset (this RectTransform tran) {
//             return tran.offsetMin.y;
//         }
//         public static void SetBottomOffset (this RectTransform tran, float off) {
//             tran.offsetMin = new Vector2(GetLeftOffset(tran), off);
//         }
//
//         /// <summary>
//         /// 在ui层级中创建新的GameObject，自带RectTransform
//         /// </summary>
//         /// <param name="name"></param>
//         /// <param name="parent"></param>
//         /// <returns></returns>
//         public static RectTransform CreateGameObject(string name, RectTransform parent)
//         {
//             var newGo = StaticUtils.CreateGameObject(name, parent);
//             return newGo.GetOrAddComponent<RectTransform> ();
//         }
//
//
//
//         /// <summary>
//         /// 屏幕中心坐标
//         /// </summary>
//         /// <returns></returns>
//         public static Vector2 GetScreenCenter () {
//             return new Vector2 ((float) Screen.width / 2f, (float) Screen.height / 2f);
//         }
//         
//         /// <summary>
//         /// 随机屏幕位置
//         /// </summary>
//         /// <returns></returns>
//         public static Vector2 RandomScreenPosition () {
//             return Rand.Range(Vector2.zero, new Vector2(Screen.width, Screen.height));
//         }
//         
//         /// <summary>
//         /// 屏幕上边缘中心
//         /// </summary>
//         /// <returns></returns>
//         public static Vector2 GetScreenTopCenter () {
//             return new Vector2 ((float) Screen.width / 2f, (float) Screen.height);
//         }
//
//         /// <summary>
//         /// 创建一个Sprite
//         /// </summary>
//         /// <param name="tex"></param>
//         /// <returns></returns>
//         public static GameObject CreateSprite (Texture2D tex) {
//             GameObject newObj = new GameObject ("new sprite");
//             SpriteRenderer sr = newObj.AddComponent<SpriteRenderer> ();
//             if (tex != null) {
//                 Sprite sp = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
//                 sr.sprite = sp;
//             }
//             return newObj;
//         }
//         
//         /// <summary>
//         /// 设置ui 的透明度
//         /// </summary>
//         /// <param name="uiObj"></param>
//         /// <param name="o"></param>
//         public static void SetOpacity (GameObject uiObj, float o) {
//             var chilren = uiObj.transform.GetComponentsInChildren<Graphic> ();
//             foreach (var child in chilren) {
//                 child.SetOpacity(o);
//             }
//         }
//         
//         public static void SetOpacity (this Graphic g, float o) {
//             var col = g.color;
//             g.color = new Color (col.r, col.g, col.b, o);
//         }
//
//         /// <summary>
//         /// Image换图
//         /// </summary>
//         /// <param name="imageObj"></param>
//         /// <param name="tex"></param>
//         /// <param name="setNative"></param>
//         public static void ChangeSprite (this Image imageObj, Texture2D tex, bool setNative = false) {
//             if (tex == null)
//                 return;
//             var sp = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f)); //注意居中显示采用0.5f值  //创建一个精灵(图片，纹理，二维浮点型坐标)
//
//             ChangeSprite(imageObj, sp, setNative);
//         }
//
//         public static void ChangeSprite (this Image imageObj, Sprite sp, bool setNative = false) {
//             if (sp == null || imageObj == null)
//                 return;
//             imageObj.sprite = sp;
//             if (setNative) {
//                 imageObj.SetNativeSize ();
//             }
//         }
//
//         /// <summary>
//         /// Sprite Renderer 换图
//         /// </summary>
//         /// <param name="sr"></param>
//         /// <param name="tex"></param>
//         public static void ChangeSprite (this SpriteRenderer sr, Texture2D tex) {
//             if (tex == null || sr == null)
//                 return;
//             var sp = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f)); //注意居中显示采用0.5f值  //创建一个精灵(图片，纹理，二维浮点型坐标)
//             ChangeSprite (sr, sp);
//         }
//
//         /// <summary>
//         /// Sprite Renderer 换图
//         /// </summary>
//         /// <param name="sr"></param>
//         /// <param name="tex"></param>
//         public static void ChangeSprite (this SpriteRenderer sr, Sprite sp) {
//             if (sr == null || sp == null)
//                 return;
//             sr.sprite = sp;
//         }
//
//         /// <summary>
//         /// 获取UI对象的尺寸(local)
//         /// </summary>
//         /// <param name="obj"></param>
//         /// <returns></returns>
//         public static Vector2 GetSize (GameObject obj) {
//             var rt = obj.GetComponent<RectTransform> ();
//             return GetSize(rt);
//         }
//         public static Vector2 GetSize (this RectTransform rt) {
//             return rt.rect.size;
//         }
//         public static Vector2 GetSize (this Graphic g) {
//             return GetSize(g.rectTransform);
//         }
//
//         
//
//         /// <summary>
//         /// 获取UI对象的宽度(local)
//         /// </summary>
//         /// <param name="obj"></param>
//         /// <returns></returns>
//         public static float GetWidth (GameObject obj) {
//             return GetSize(obj).x;
//         }
//         public static float GetWidth (this RectTransform rt) {
//             return GetSize(rt).x;
//         }
//         public static float GetWidth (this Graphic g) {
//             return GetSize(g).x;
//         }
//         
//         
//         /// <summary>
//         /// 获取UI对象的高度(local)
//         /// </summary>
//         /// <param name="obj"></param>
//         /// <returns></returns>
//         public static float GetHeight (GameObject obj) {
//             return GetSize(obj).y;
//         }
//         public static float GetHeight (RectTransform rt) {
//             return GetSize(rt).y;
//         }
//         public static float GetHeight (Graphic g) {
//             return GetSize(g).y;
//         }
//         
//         /// <summary>
//         /// 设置UI对象的尺寸(local), 只能对锚点在中心的ui使用
//         /// </summary>
//         /// <param name="rt"></param>
//         /// <param name="size"></param>
//         public static void SetSize (this RectTransform rt, Vector2 size) {
//             rt.sizeDelta = size;
//         }
//         
//         /// <summary>
//         /// 设置UI对象的宽度(local), 只能对锚点在中心的ui使用
//         /// </summary>
//         /// <param name="obj"></param>
//         /// <param name="w"></param>
//         public static void SetWidth (GameObject obj, float w) {
//             var rt = obj.GetComponent<RectTransform> ();
//             if (rt != null) {
//                 SetWidth(rt, w);
//             }
//         }
//         public static void SetWidth (this RectTransform tran, float w) {
//             tran.sizeDelta = new Vector2 (w, tran.sizeDelta.y);
//         }
//         /// <summary>
//         /// 设置UI对象的高度(local), 只能对锚点在中心的ui使用
//         /// </summary>
//         /// <param name="obj"></param>
//         /// <param name="h"></param>
//         public static void SetHeight (GameObject obj, float h) {
//             var rt = obj.GetComponent<RectTransform> ();
//             if (rt != null) {
//                 SetHeight(rt, h);
//             }
//         }
//         public static void SetHeight (this RectTransform tran, float h) {
//             tran.sizeDelta = new Vector2 (tran.sizeDelta.x, h);
//         }
//
//         /// <summary>
//         /// 计算ui的屏幕rect, 注意不要在刚打开ui的时候马上调用，位置可能会计算错误
//         /// </summary>
//         /// <param name="ui"></param>
//         /// <param name="cam"></param>
//         /// <returns></returns>
//         public static Rect GetScreenRect (this RectTransform ui, Camera cam) {
//             if (ui == null) return default (Rect);
//             if (cam == null) return ui.rect;
//             var corners = new Vector3[4];
//             ui.GetWorldCorners (corners);
//
//             // to screen
//             StaticUtils.ForEachInArray (corners, (point, index) => {
//                 corners[index] = cam.WorldToScreenPoint (point);
//             });
//
//             float height = Vector3.Distance (corners[0], corners[1]);
//             float width = Vector3.Distance (corners[1], corners[2]);
//
//             return new Rect(corners[0], new Vector2(width, height));
//         }
//         
//         /// <summary>
//         /// 计算ui的屏幕尺寸
//         /// </summary>
//         /// <param name="ui"></param>
//         /// <param name="cam"></param>
//         /// <returns></returns>
//         public static Vector2 GetScreenSize (this RectTransform ui, Camera cam) {
//             var rect = GetScreenRect(ui, cam);
//             return rect.size;
//         }
//         /// <summary>
//         /// 设置UI对象的材质
//         /// </summary>
//         /// <param name="uiEle"></param>
//         /// <param name="mat"></param>
//         public static void SetMaterial (this Graphic uiEle, Material mat = null) {
//             if (uiEle == null) return;
//             uiEle.material = mat;
//         }
//  
//         /// <summary>
//         /// 用屏幕坐标设置ui位置
//         /// </summary>
//         /// <param name="ui">ui的rectTransform</param>
//         /// <param name="containerRect">ui的父级rectTransform</param>
//         /// <param name="screenPos">屏幕位置</param>
//         /// <param name="cam">ui的渲染相机</param>
//         public static void SetAnchoredPosByScreenPos (this RectTransform ui, RectTransform containerRect, Vector2 screenPos, Camera cam) {
//             if (ui == null || containerRect == null || cam == null) return;
//
//             RectTransformUtility.ScreenPointToLocalPointInRectangle (containerRect,
//                 screenPos, cam, out var finalPos);
//
//             ui.anchoredPosition = finalPos;
//         }
//
//         public const float defaultShakeDur = 0.03f;
//         public const float defaultShakeAmplitudeX = 0;
//         public const float defaultShakeAmplitudeY = -18;
//
//         /// <summary>
//         /// UI物体震动
//         /// </summary>
//         /// <param name="rt"></param>
//         /// <param name="shakeAmplitudeX">震动X轴幅度</param>
//         /// <param name="shakeAmplitudeY">震动Y轴幅度</param>
//         /// <param name="shakeDur">单次震动时间</param>
//         /// <returns></returns>
//         public static Sequence Shake (this RectTransform rt, 
//             float shakeAmplitudeX = defaultShakeAmplitudeX, 
//             float shakeAmplitudeY = defaultShakeAmplitudeY, 
//             float shakeDur = defaultShakeDur) {
//             
//             var shakeAmplitude = new Vector2(shakeAmplitudeX, shakeAmplitudeY);
//             
//             var curPos = rt.anchoredPosition;
//
//             //			rt.DOShakeAnchorPos (0.135f);
//
//             rt.DOKill(true);
//             var shakeSequence = DOTween.Sequence ();
//             shakeSequence.SetUpdate (true);
//             shakeSequence.Append (rt.DOAnchorPos (curPos + shakeAmplitude, shakeDur))
//                 .Append (rt.DOAnchorPos (curPos - shakeAmplitude, shakeDur))
//                 .Append (rt.DOAnchorPos (curPos, shakeDur))
//                 .Append (rt.DOAnchorPos (curPos + shakeAmplitude, shakeDur / 2f))
//                 .Append (rt.DOAnchorPos (curPos - shakeAmplitude, shakeDur / 2f))
//                 .Append (rt.DOAnchorPos (curPos, shakeDur / 2f));
//
//             return shakeSequence;
//         }
//
//
//         #region Event
//
//         /// <summary>
//         /// 点击事件向下传递(遇到第一个RaycastTarget即返回，无论这个target有没有实际处理事件)
//         /// 注意触发的时候，clickObj自身必须仍然在点击位置下面，否则无法向下传递
//         /// </summary>
//         /// <param name="clickObj"></param>
//         /// <param name="data"></param>
//         /// <param name="logDetail"></param>
//         /// <returns></returns>
//         public static GameObject PassClickEventDownstairs(this GameObject clickObj, PointerEventData data, bool logDetail = false) {
//             PassPointerEventDownstairs(clickObj, data, ExecuteEvents.pointerClickHandler, out var revGo, false,
//                 logDetail);
//
//             return revGo;
//         }
//
//         /// <summary>
//         /// 将PointerEventData事件向下传递一层
//         /// </summary>
//         /// <param name="checkObj">发起物体</param>
//         /// <param name="data">事件数据</param>
//         /// <param name="func">处理方法</param>
//         /// <param name="handlerGo">接受者物体,如果没有接受者返回null，如果事件被处理了，接受者就是处理者</param>
//         /// <param name="waitHandler">是否持续向下直到找到处理者为止</param>
//         /// <param name="logDetail"></param>
//         /// <typeparam name="T"></typeparam>
//         /// <returns>事件是否被处理（事件未被处理的情况，如果revGo不为null说明事件被revGo拦截，revGo是一个RayCastTarget）</returns>
//         public static bool PassPointerEventDownstairs<T> (
//             GameObject checkObj, 
//             PointerEventData data, 
//             ExecuteEvents.EventFunction<T> func,
//             out GameObject handlerGo,
//             bool waitHandler = false,
//             bool logDetail = false)
//         where T : IEventSystemHandler {
//             
//             //注意Drag事件的情况，会因为无法找到Drag的原物体而无法传递
//             var downstairs = RaycastDownstairs(checkObj, data);
// //            Debug.Log($"PassPointerEventDownstairs RaycastDownstairs {downstairs.Count}");
// //            var dragName = data.pointerDrag != null ? data.pointerDrag.name : "empty";
// //            Debug.Log($"Draging {dragName}");
//             var downstairsCount = downstairs.Count;
//
//             bool handled = false;
//             handlerGo = null;
//             for (int i = 0; i < downstairsCount; i++) {
//                 var touchedGo = downstairs[i].gameObject;
//                 var handler = ExecuteEvents.GetEventHandler<T>(touchedGo);
//                 
//                 if (handler == null) {
//                     if (logDetail) {
//                         Debug.Log($"{typeof(T).Name} : touch on [{touchedGo.name}], no handler");
//                     }
//                     continue;
//                 }
//                 
//                 if (handler.gameObject == checkObj) {
//                     // 避免重复执行
//                     continue;
//                 }
//                
//
//                 handlerGo = handler;
//                 handled = ExecuteEvents.Execute (handler, data, func);
//                 if (logDetail) {
//                     Debug.Log($"{typeof(T).Name} : touch on [{touchedGo.name}], handler [{handlerGo.name}] handled [{handled}]");
//                 }
//
//                 if (waitHandler && ! handled) {
//                     handlerGo = null;
//                     continue;
//                 }
//
//                 break;
//             }
//
//             return handled;
//         }
//
//         /// <summary>
//         /// 取得Raycast结果中 位于参考物体下层的列表
//         /// 注意如果参考物体没有在Raycast结果中，那么无法判断其他物体是否在参考物体下面，返回空表
//         /// </summary>
//         /// <param name="checkObj">参考物体</param>
//         /// <param name="pointerData"></param>
//         /// <returns></returns>
//         public static List<RaycastResult> RaycastDownstairs(this GameObject checkObj, PointerEventData pointerData) {
//             var allResults = new List<RaycastResult> ();
//             EventSystem.current.RaycastAll (pointerData, allResults);
//
//             var allCount = allResults.Count;
//             var filterResults = new List<RaycastResult> ();
//             bool foundCheckObj = false;
//             for (var i = 0; i < allCount; ++i) {
//                 if (foundCheckObj) {
//                     filterResults.Add(allResults[i]);
//                 }
//                 else {
//                     if (allResults[i].gameObject == checkObj) {
//                         foundCheckObj = true;
//                     }
//                 }
//             }
//
//             return filterResults;
//         }
//
//         /// <summary>
//         /// 清理层级中的RaycastTarget
//         /// </summary>
//         /// <param name="root"></param>
//         /// <param name="clearSelf">是否同时清理自己</param>
//         public static void ClearRaycastTargetInChildren(this Transform root, bool clearSelf = true) {
//             if (clearSelf) {
//                 var selfGraphic = root.GetComponent<Graphic>();
//                 if (selfGraphic != null) {
//                     selfGraphic.raycastTarget = false;
//                 }
//             }
//             
//             var childenGraphics = root.GetComponentsInChildren<Graphic>(true);
//             foreach (var g in childenGraphics) {
//                 g.raycastTarget = false;
//             }
//         }
//
//         /// <summary>
//         /// 设置单一事件
//         /// </summary>
//         /// <param name="ue"></param>
//         /// <param name="func"></param>
//         public static void SetListener (this UnityEvent ue, UnityAction func) {
//             if (ue == null) return;
//             
//             ue.RemoveAllListeners ();
//
//             if (func != null) {
//                 ue.AddListener (func);
//             }
//         }
//         
//         public static void SetListener<T0> (this UnityEvent<T0> ue, UnityAction<T0> func) {
//             if (ue == null) return;
//             
//             ue.RemoveAllListeners ();
//
//             if (func != null) {
//                 ue.AddListener (func);
//             }
//         }
//         
//         public static void SetListener<T0, T1> (this UnityEvent<T0, T1> ue, UnityAction<T0, T1> func) {
//             if (ue == null) return;
//             
//             ue.RemoveAllListeners ();
//
//             if (func != null) {
//                 ue.AddListener (func);
//             }
//         }
//         
//         public static void SetListener<T0, T1, T2> (this UnityEvent<T0, T1, T2> ue, UnityAction<T0, T1, T2> func) {
//             if (ue == null) return;
//             
//             ue.RemoveAllListeners ();
//
//             if (func != null) {
//                 ue.AddListener (func);
//             }
//         }
//         
//         public static void SetListener<T0, T1, T2, T3> (this UnityEvent<T0, T1, T2, T3> ue, UnityAction<T0, T1, T2, T3> func) {
//             if (ue == null) return;
//             
//             ue.RemoveAllListeners ();
//
//             if (func != null) {
//                 ue.AddListener (func);
//             }
//         }
//
//
//         /// <summary>
//         /// 添加事件，避免重复
//         /// </summary>
//         /// <param name="ue"></param>
//         /// <param name="func"></param>
//         public static void AddListenerNoRepeat(this UnityEvent ue, UnityAction func) {
//             if (ue == null || func == null) return;
//             
//             ue.RemoveListener(func);
//             ue.AddListener(func);
//         }
//         
//         public static void AddListenerNoRepeat<T0>(this UnityEvent<T0> ue, UnityAction<T0> func) {
//             if (ue == null || func == null) return;
//             
//             ue.RemoveListener(func);
//             ue.AddListener(func);
//         }
//         
//         public static void AddListenerNoRepeat<T0, T1>(this UnityEvent<T0, T1> ue, UnityAction<T0, T1> func) {
//             if (ue == null || func == null) return;
//             
//             ue.RemoveListener(func);
//             ue.AddListener(func);
//         }
//         
//         public static void AddListenerNoRepeat<T0, T1, T2>(this UnityEvent<T0, T1, T2> ue, UnityAction<T0, T1, T2> func) {
//             if (ue == null || func == null) return;
//             
//             ue.RemoveListener(func);
//             ue.AddListener(func);
//         }
//         
//         public static void AddListenerNoRepeat<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> ue, UnityAction<T0, T1, T2, T3> func) {
//             if (ue == null || func == null) return;
//             
//             ue.RemoveListener(func);
//             ue.AddListener(func);
//         }
//         
//         /// <summary>
//         /// 添加一次性监听，触发后，直接删除
//         /// </summary>
//         /// <param name="ue"></param>
//         /// <param name="func"></param>
//         public static void AddListenerOnce(this UnityEvent ue, UnityAction func) {
//             if (ue == null || func == null) return;
//
//             UnityAction newFunc = null;
//             newFunc = () =>
//             {
//                 func();
//                 ue.RemoveListener(newFunc);
//             };
//             ue.AddListener(newFunc);
//         }
//         
//         public static void AddListenerOnce<T0>(this UnityEvent<T0> ue, UnityAction<T0> func) {
//             if (ue == null || func == null) return;
//             
//             UnityAction<T0> newFunc = null;
//             newFunc = (a) =>
//             {
//                 func(a);
//                 ue.RemoveListener(newFunc);
//             };
//             ue.AddListener(newFunc);
//         }
//         
//         public static void AddListenerOnce<T0, T1>(this UnityEvent<T0, T1> ue, UnityAction<T0, T1> func) {
//             if (ue == null || func == null) return;
//             
//             UnityAction<T0, T1> newFunc = null;
//             newFunc = (a, b) =>
//             {
//                 func(a, b);
//                 ue.RemoveListener(newFunc);
//             };
//             ue.AddListener(newFunc);
//         }
//         
//         public static void AddListenerOnce<T0, T1, T2>(this UnityEvent<T0, T1, T2> ue, UnityAction<T0, T1, T2> func) {
//             if (ue == null || func == null) return;
//             
//             UnityAction<T0, T1, T2> newFunc = null;
//             newFunc = (a, b, c) =>
//             {
//                 func(a, b, c);
//                 ue.RemoveListener(newFunc);
//             };
//             ue.AddListener(newFunc);
//         }
//         
//         public static void AddListenerOnce<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> ue, UnityAction<T0, T1, T2, T3> func) {
//             if (ue == null || func == null) return;
//             
//             UnityAction<T0, T1, T2, T3> newFunc = null;
//             newFunc = (a, b, c, d) =>
//             {
//                 func(a, b, c, d);
//                 ue.RemoveListener(newFunc);
//             };
//             ue.AddListener(newFunc);
//         }
//         
//         #endregion // Event
//
//     }
// }