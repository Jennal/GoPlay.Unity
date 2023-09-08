using GoPlay.Data;
using UnityEngine;
using GoPlay.Framework.Services;
using GoPlay.Managers;
using GoPlay.Utils;

namespace GoPlay.Globals
{
    public partial class GlobalVariables : MonoSingleton<GlobalVariables>
    {
        public static bool IsQuitting = false;
        public LocalData LocalData = new LocalData();

        private void OnEnable()
        {
            Application.wantsToQuit += () => IsQuitting = true;
            OnEnabled();
        }

        partial void OnEnabled();

        public static void Init(string key)
        {
            var persistantService = ServiceManager.Get<PersistantService>();
            var gv = Instance;
            gv.LocalData = persistantService.Get($"GoPlay.{key}.local", new LocalData
            {
                Language = Application.systemLanguage.GetVariant(),
            });
        }
    }
}