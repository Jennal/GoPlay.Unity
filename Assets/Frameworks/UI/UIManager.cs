using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoPlay.Utils;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GoPlay.Framework.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
#if ODIN_INSPECTOR_3
        [BoxGroup("Status"), ShowInInspector, ReadOnly]
#endif
        internal List<UIPage> PageStack = new List<UIPage>();

#if ODIN_INSPECTOR_3
        [BoxGroup("Status"), ShowInInspector]
#endif
        public UIPage CurrentPage => PageStack.LastOrDefault();

        public static T Create<T>()
            where T : UIPanel
        {
            var root = GetRoot<T>();
            if (typeof(T).IsSubclassOf(typeof(UIPage)))
            {
                var go = new GameObject(typeof(T).Name, typeof(RectTransform));
                go.layer = LayerMask.NameToLayer("UI");
                go.transform.SetParent(root);
                go.transform.RectTransformResizeFullScreen();
                return go.AddComponent<T>();
            }
            else
            {
                var prefab = UILoader.Load<T>();
                var go = Instantiate(prefab, root);
                var panel = go.GetComponent<T>();
                return panel;    
            }
        }
        
        public static T GetOrCreate<T>()
            where T : UIPanel
        {
            var panel = Get<T>();
            if (panel) return panel;

            return Create<T>();
        }
        
        public static T GetOrOpen<T>(object data=null)
            where T : UIPanel
        {
            var panel = Get<T>();
            if (panel) return panel;

            return Open<T>(data);
        }

        public static T Get<T>()
            where T : UIPanel
        {
            return Instance.GetComponentInChildren<T>(true);
        }

        public static T[] GetList<T>()
            where T : UIPanel
        {
            return Instance.GetComponentsInChildren<T>(true);
        }
        
        public static T Open<T>(object data=null)
            where T : UIPanel
        {
            var panel = Create<T>();
            panel.Open(data);

            return panel;
        }

        public static UIPanel Open(string typeName, object data=null)
        {
            var prefab = UILoader.Load(typeName);
            
            var root = GetRoot<UIPageChild>();
            var go = Instantiate(prefab, root);
            var panel = go.GetComponent<UIPanel>();
            panel.Open(data);

            return panel;
        }
        
        public static void Close<T>()
            where T : UIPanel
        {
            var panel = Get<T>();
            if (panel == null) return;
            
            panel.Close();
        }
        
        public static Transform GetRoot<T>()
            where T : UIPanel
        {
            if (typeof(T).IsSubclassOf(typeof(UIPageChild)))
            {
                if (Instance.CurrentPage)
                {
                    return Instance.CurrentPage.transform;
                }
            }

            return Instance.transform;
        }

        public static async Task<T> OpenAsync<T>(object data=null)
            where T : UIPanel
        {
            var prefab = await UILoader.LoadAsync<T>();
            
            var root = GetRoot<T>();
            var go = Instantiate(prefab, root);
            var panel = go.GetComponent<T>();
            
            await panel.OpenAsync(data);
            return panel;
        }
        
        public static async Task<T> GetOrOpenAsync<T>(object data=null)
            where T : UIPanel
        {
            var panel = Get<T>();
            if (panel) return panel;

            return await OpenAsync<T>(data);
        }
        
        public static async Task CloseAsync<T>()
            where T : UIPanel
        {
            var panel = Get<T>();
            if (panel == null) return;
            
            await panel.CloseAsync();
        }
        
        #region Page API

        public static T PushPage<T>(object data=null)
            where T : UIPage
        {
            var curPage = Instance.CurrentPage;
            if (curPage is T page)
            {
                page.Open(data);
                Instance.PageStack.Remove(page);
                return page;
            }
            if (curPage) curPage.Hide();
            
            return Open<T>(data);
        }

        public static UIPage PopPage()
        {
            var curPage = Instance.CurrentPage;
            if (curPage) curPage.Close();

            curPage = Instance.CurrentPage;
            curPage.Show();
            return curPage;
        }
        
        public static T ReplacePage<T>(object data=null)
            where T : UIPage
        {
            var curPage = Instance.CurrentPage;
            var page = Open<T>(data);

            if (curPage) curPage.Close();

            return page;
        }
        
        #endregion

        #region InputCtrl

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                var topWindow = Instance.CurrentPage.ChildrenWindows.LastOrDefault();
                if (topWindow != default)
                {
                    topWindow.OnInputEscape();
                }
            }
        }

        #endregion

    }
}