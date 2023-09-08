using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoPlay.Framework.UI;
// using GoPlay.Tutorials;
using GoPlay.Utils;

namespace GoPlay.UI.Common
{
    public class UIListItem : MonoBehaviour
    {
        [HideInInspector]
        public int idx;
        public virtual void Init(object data) { }
        public virtual void UpdateData(object data) { }
        public virtual void UpdateData(object data, long time) { }
        public virtual void UpdateData(object data, bool type) { }
        public virtual void UpdateData(object data, int index) { }
        public virtual void UpdateData(object data, string textContent) { }
        public virtual void RegisterEvent() { }
        public virtual void UnRegisterEvent() { }
        public virtual void UpdateTime(long time){}

        // public virtual void UpdateTutorialIndexer(string val)
        // {
        //     var indexer = gameObject.GetOrAddComponent<TutorialIndexer>();
        //     indexer.Index = val;
        // }
        public virtual void OnRemove() { }

        protected virtual void Awake()
        {
            RegisterEvent();
        }

        protected virtual void OnDestroy()
        {
            UnRegisterEvent();
        }

        public Action<UIListItem> onClick;
    }

    public class UIList : UIWidget
    {
        public Transform m_template;

        Transform m_root { get { return this.transform; } }
        private string m_templateName => m_template?.name??"UIItem";

        public Action<UIListItem> onItemClick { get; set; }
        public Action<UIListItem, bool> onToggleValue { get; set; }

        public void InitTemplete<T>(object data = null) where T : UIListItem
        {
            if (m_template == null) m_template = transform.GetChild(0);
            // m_templateName = m_template.gameObject.name;

            for (int i = 0; i < m_root.childCount; i++)
            {
                Transform t = m_root.GetChild(i);
                t.gameObject.name = $"{m_templateName}_{i}";
                UIListItem item = t.GetOrAddComponent<T>();

                item.idx = i;
                item.onClick += x => onItemClick?.Invoke(x);
                item.Init(data);
            }
        }

        // public void UpdateTutorialIndexes(List<string> list)
        // {
        //     if (list == null)
        //         return;
        //
        //     for (int i = 0; i < list.Count; i++)
        //     {
        //         Transform child;
        //         var index = i;
        //         
        //         if (index < m_root.childCount) child = m_root.GetChild(index);
        //         else child = CreateChild();
        //
        //         var item = child.GetComponent<UIListItem>();
        //         if (item == null) continue;
        //         
        //         item.idx = index;
        //         item.UpdateTutorialIndexer(list[i]);
        //     }
        // }
        
        public void UpdateData(IList list, int startIndex = 0)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Transform child;
                int index = i + startIndex;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(list[i]);
                }
            }

            for (int i = list.Count + startIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }
        
        public void UpdateData(object data, int endIndex)
        {
            if (data == null)
                return;

            for (int i = 0; i < endIndex; i++)
            {
                Transform child;
                int index = i;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(data, i);
                }
            }

            for (int i = endIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }

        public void UpdateData(IList list, List<long> time, int startIndex = 0)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Transform child;
                int index = i + startIndex;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(list[i], time[i]);
                }
            }

            for (int i = list.Count + startIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }
        
        public void UpdateData(IList list, List<bool> listBools, int startIndex = 0)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Transform child;
                int index = i + startIndex;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(list[i], listBools[i]);
                }
            }

            for (int i = list.Count + startIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }
        
        public void UpdateData(IList list, long time, int startIndex = 0)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Transform child;
                int index = i + startIndex;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(list[i], time);
                }
            }

            for (int i = list.Count + startIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }
        
        public void UpdateData(IList list, string textContent, int startIndex = 0)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Transform child;
                int index = i + startIndex;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(list[i], textContent);
                }
            }

            for (int i = list.Count + startIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }
        
        public void UpdateData(IList list, bool type, int startIndex = 0)
        {
            if (list == null)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                Transform child;
                int index = i + startIndex;
                if (index < m_root.childCount)
                    child = m_root.GetChild(index);
                else
                    child = CreateChild();

                child.gameObject.SetActive(true);
                UIListItem item = child.GetComponent<UIListItem>();
                if (item != null)
                {
                    item.idx = index;
                    item.UpdateData(list[i], type);
                }
            }

            for (int i = list.Count + startIndex; i < m_root.childCount; i++)
                RemoveChild(i);
        }

        public void UpdateTime(long Time)
        {
            foreach (Transform child in this.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    child.GetComponent<UIListItem>().UpdateTime(Time);
                }
            }

        }

        public GameObject Add(object data)
        {
            var index = Count();
            Transform child = AddChild();
            UIListItem item = child.GetComponent<UIListItem>();
            if (item != null)
            {
                item.idx = index;
                item.UpdateData(data);
            }

            return child.gameObject;
        }

        private Transform AddChild()
        {
            Transform child = null;
            for (int i = 0; i < m_root.childCount; i++)
            {
                Transform t = m_root.GetChild(i);
                if (!t.gameObject.activeSelf)
                {
                    child = t;
                    break;
                }
            }

            if (child == null)
                child = CreateChild();

            child.gameObject.SetActive(true);
            return child;
        }

        private Transform CreateChild()
        {
            if (m_template == null) m_template = transform.GetChild(0);
            Transform child = Instantiate(m_template.gameObject).transform;
            child.name = $"{m_templateName}_{m_root.childCount}";
            child.SetParent(m_root, false);
            //child.localScale = m_template.localScale;
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;

            UIListItem item = child.GetComponent<UIListItem>();
            if (item != null)
            {
                item.onClick += x => onItemClick?.Invoke(x);
            }

            return child;
        }

        private void RemoveChild(int index, bool destroy = false)
        {
            var go = transform.GetChild(index).gameObject;
            if (go.activeSelf)
            {
                UIListItem item = go.GetComponent<UIListItem>();
                if (item != null)
                    item.OnRemove();

                go.SetActive(false);
            }

            if (destroy)
                Destroy(go);
        }

        public void Clear(int retainCount = -1)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                RemoveChild(i, retainCount >= 0 && i >= retainCount);
        }

        public void RemoveItem(UIListItem item)
        {
            var t = item.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) == t)
                {
                    RemoveChild(i);
                    break;
                }
            }
        }

        public int Count()
        {
            int count = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                    count++;
            }
            return count;
        }

        public int RealCount(){
            return transform.childCount;
        }

        public T GetChild<T>(int index)
        {
            if (index >= 0 && index < transform.childCount)
            {
                Transform child = transform.GetChild(index);
                return child.GetComponent<T>();
            }

            return default(T);
        }

        public UIListItem GetItem(int index,bool checkActive = true)
        {
            UIListItem item = null;
            int count = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                if ((checkActive && t.gameObject.activeSelf) || !checkActive)
                {
                    if (count == index)
                    {
                        item = t.GetComponent<UIListItem>();
                        break;
                    }

                    count++;
                }
            }

            return item;
        }

        public void Foreach<T>(Action<T> callback) where T : UIListItem
        {
            if (callback == null)
                return;

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject go = transform.GetChild(i).gameObject;
                if (go.activeSelf)
                {
                    var item = go.GetComponent<T>();
                    if (item != null)
                        callback(item);
                }
            }
        }
        
        public void Foreach<T>(Action<int, T> callback) where T : UIListItem
        {
            if (callback == null)
                return;

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject go = transform.GetChild(i).gameObject;
                if (go.activeSelf)
                {
                    var item = go.GetComponent<T>();
                    if (item != null)
                        callback(i, item);
                }
            }
        }

        public bool Contains(UIListItem item)
        {
            var trans = item.transform;
            foreach (Transform t in transform)
            {
                if (t == trans)
                    return true;
            }

            return false;
        }

        public T Find<T>(Predicate<T> match) where T : UIListItem
        {
            T target = null;
            foreach (Transform t in transform)
            {
                if (t.gameObject.activeSelf)
                {
                    var item = t.GetComponent<T>();
                    if (item != null && match(item))
                    {
                        target = item;
                        break;
                    }
                }
            }

            return target;
        }
    }
}