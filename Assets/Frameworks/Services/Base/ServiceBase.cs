using GoPlay.Interfaces;
using UnityEngine;

namespace GoPlay.Services.Base
{
    /// <summary>
    /// Service 提供通用逻辑，与游戏逻辑不相关
    /// </summary>
    public abstract class ServiceBase : MonoBehaviour, IService {
        protected bool Inited;
        public bool DebugMode = false;

        protected virtual bool OnlyInitOnce {
            get { return true; }
        }

        public void Initialize() {
            if (OnlyInitOnce && Inited) return;
            
            OnInitialize();
            
            Inited = true;
        }

        protected virtual void OnInitialize() {
            
        }
    }
}
