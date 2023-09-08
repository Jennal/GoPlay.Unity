#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GoPlay.Services
{
    public class PoolableMonoBehaviour : MonoBehaviour, IPoolable
    {
#if ODIN_INSPECTOR_3
        [BoxGroup("Poolable"), ShowInInspector, ReadOnly]
#endif
        private PoolData _pool;

        public PoolData Pool { get => _pool; set => _pool = value; }

        public GameObject GameObject => gameObject;
        
        public Transform Transform => transform;

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="args"></param>
        public virtual void SetData(params object[] args)
        {
        }
        
        /// <summary>
        /// 生成或者从Pool取出时
        /// </summary>
        public virtual void OnSpawn(params object[] args)
        {
            SetData(args);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 返回Pool时 
        /// </summary>
        public virtual void OnDespown()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 回归Pool
        /// </summary>
        public virtual void Despawn()
        {
            PoolService.Instance.Despawn(this);
        }
    }
}