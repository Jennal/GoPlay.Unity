using GoPlay.Interfaces;
using UnityEngine;

namespace GoPlay.Modules.Base
{
    /// <summary>
    /// System 游戏相关逻辑
    /// </summary>
    public abstract class ModuleBase : MonoBehaviour, IModule
    {
        protected bool Inited;

        protected virtual bool OnlyInitOnce {
            get { return true; }
        }

        public void Initialize() {
            if (OnlyInitOnce && Inited) return;
        
            OnInitialize();
            Inited = true;
        }

        public void SetEnable(bool bEnable) {
            gameObject.SetActive(bEnable);
        }

        protected virtual void OnInitialize()
        {}

        protected virtual void OnDestroy()
        {    
        }
    }
}