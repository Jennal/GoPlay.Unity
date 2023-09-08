using System;
using GoPlay.Managers;
using GoPlay.Modules.Translate;
using UnityEngine;

namespace GoPlay.Translate
{
    public class ActiveWhenLangSystemReady : MonoBehaviour
    {
        public GameObject[] GameObjects;

        private void Awake()
        {
            var active = true;

            var langSystem = ModuleManager.Check<LangModule>();
            if (!langSystem || !langSystem.IsReady) active = false;

            foreach (var go in GameObjects)
            {
                if (!go) continue;
                go.SetActive(active);
            }
        }

        private void Update()
        {
            var langSystem = ModuleManager.Check<LangModule>();
            if (!langSystem || !langSystem.IsReady) return;

            foreach (var go in GameObjects)
            {
                if (!go) continue;
                go.SetActive(true);
            }

            Destroy(this);
        }
    }
}