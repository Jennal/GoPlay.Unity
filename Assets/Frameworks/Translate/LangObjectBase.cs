using System;
using System.Collections;
using System.Linq;
using GoPlay.Managers;
using GoPlay.Modules.Translate;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace GoPlay.Translate
{
    public abstract class LangObject : MonoBehaviour
    {
        public bool AutoLocalize = true;
        public int Id;
#if ODIN_INSPECTOR_3
        [ValueDropdown("AllLanguages")]
#endif
        public string Lang;
        
#if UNITY_EDITOR
        public string[] AllLanguages()
        {
            var system = ModuleManager.Get<LangModule>();
            return new [] {""}.Union(system.AllLanguages).ToArray();
        }
#endif
        
        public abstract void SetData();
        
        protected virtual IEnumerator Start()
        {
            var langSystem = ModuleManager.Check<LangModule>();
            while (!langSystem || !langSystem.IsReady)
            {
                langSystem = ModuleManager.Check<LangModule>();
                yield return null;
            }

            if (AutoLocalize)
            {
                langSystem.Regist(this);
                if (langSystem.CurrentLang != Lang) SetData();
            }
            
            Lang = langSystem.CurrentLang;
        }

        private void OnDestroy()
        {
            var langSystem = ModuleManager.Check<LangModule>();
            if (langSystem) langSystem.Unregist(this);
        }
    }
    
    public abstract class LangObjectBase<T> : LangObject
    {
        public abstract T GetData();
    }
}