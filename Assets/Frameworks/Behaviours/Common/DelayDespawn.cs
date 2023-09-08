using GoPlay.Services;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GoPlay.Behaviours.Common
{
    public class DelayDespawn : PoolableMonoBehaviour
    {
#if ODIN_INSPECTOR_3
        [BoxGroup("Settings")]
#endif
        public float Duration;
#if ODIN_INSPECTOR_3
        [BoxGroup("Status"), ShowInInspector, ReadOnly]
#endif
        protected float _duration;

        private void FixedUpdate()
        {
            if (_duration >= Duration) Despawn();
            
            _duration += Time.deltaTime;
        }

        public override void OnSpawn(params object[] args)
        {
            _duration = 0f;
        }
    }
}