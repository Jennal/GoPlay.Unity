using System.Collections;
using System.Threading.Tasks;
using Asyncoroutine;
using GoPlay.Globals;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GoPlay.Framework.UI
{
    public abstract class UIPanel : UIBehaviour
    {
        [HideInInspector] public UnityEvent OnOpenEvent = new UnityEvent();
        [HideInInspector] public UnityEvent OnCloseEvent = new UnityEvent();
        
        [HideInInspector] public RectTransform RootNode;
        
        protected override void Awake() 
        {
            base.Awake();
            RootNode = gameObject.GetComponent<RectTransform>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            OnCloseEvent.Invoke();
            GlobalEvents.UIClosed.Invoke(this);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        
        protected virtual void SetData(params object[] data)
        {
        }
        
        public virtual void Open(params object[] data)
        {
            SetData(data);
            OnOpen();
            StartCoroutine(OpenCoroutine());
        }
        
        public virtual void Close()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CloseCoroutine());
            }
            else
            {
                OnClose();
                Destroy(gameObject);
            }
        }

        public virtual async Task OpenAsync(params object[] data)
        {
            SetData(data);
            OnOpen();
            await OpenCoroutine();
        }
        
        public virtual async Task CloseAsync()
        {
            if (gameObject.activeInHierarchy)
            {
                await CloseCoroutine();
            }
            
            OnClose();
        }
        
        protected virtual IEnumerator OpenCoroutine()
        {
            yield return OpenDisplay();
            OnOpenEvent.Invoke();
            GlobalEvents.UIOpened.Invoke(this);
        }
        
        protected virtual IEnumerator CloseCoroutine()
        {
            yield return CloseDisplay();
            OnClose();
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 打开动画实现
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OpenDisplay()
        {
            yield break;
        }

        /// <summary>
        /// 关闭动画实现
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CloseDisplay()
        {
            yield break;
        }

        /// <summary>
        /// 打开后逻辑
        /// </summary>
        protected virtual void OnOpen()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 关闭后逻辑
        /// </summary>
        protected virtual void OnClose()
        {
            Destroy(gameObject);
        }
    }
}