using System.Collections;
using GoPlay.Globals;
using GoPlay.Managers;
using GoPlay.Services;
using UnityEngine;

namespace GoPlay.Framework.UI
{
    public class UIPoolablePageChild : UIPageChild, IPoolable
    {
        #region IPoolable

        private PoolData _poolData;
        
        public PoolData Pool { get => _poolData; set => _poolData = value; }
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public virtual void OnSpawn(params object[] args)
        {
            var root = UIManager.GetRoot<UIPoolablePageChild>();
            transform.SetParent(root, false);
            transform.localScale = Vector3.one;
            
            Open(args);
        }

        public virtual void OnDespawn()
        {
            OnClose();
            gameObject.SetActive(false);
        }

        public virtual void Despawn()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CloseCoroutine());
            }
            else
            {
                if (GlobalVariables.IsQuitting) return;
                PoolService.Instance.Despawn(this);
            }
        }
        
        /// <summary>
        /// 回归Pool
        /// </summary>
        public virtual void Despawn(float delay)
        {
            PoolService.Instance.Despawn(this, delay);
        }
        
        #endregion

        protected override void OnClose()
        {
            /* DO NOTHING */
        }

        public override void Close()
        {
            var poolService = PoolService.Instance;
            if (Pool == null || !poolService.IsRegisted(gameObject))
            {
                base.Close();
                return;
            }
            
            Despawn();
        }
        
        protected override IEnumerator CloseCoroutine()
        {
            yield return CloseDisplay();
            var poolService = PoolService.Instance;
            if (Pool == null || !poolService.IsRegisted(gameObject))
            {
                Destroy(gameObject);
                yield break;
            }
            
            if (GlobalVariables.IsQuitting) yield break;
            PoolService.Instance.Despawn(this);
        }
    }
}