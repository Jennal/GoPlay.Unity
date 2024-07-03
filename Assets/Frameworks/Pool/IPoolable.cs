using UnityEngine;

namespace GoPlay.Services
{
    public interface IPoolable
    {
        PoolData Pool { get; set; }
        
        GameObject GameObject { get; }
        
        Transform Transform {get;}

        /// <summary>
        /// 生成或者从Pool取出时
        /// </summary>
        void OnSpawn(params object[] args);

        /// <summary>
        /// 返回Pool时 
        /// </summary>
        void OnDespawn();

        /// <summary>
        /// 回归Pool
        /// </summary>
        void Despawn();
    }
}