using System;
using UnityEngine;
using GoPlay.Services.Base;

namespace GoPlay.Framework.Services
{
    public class PersistantService : ServiceBase
    {
        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

        public void Set<T>(string key, T val)
        {
            var json = JsonUtility.ToJson(val);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public T Get<T>(string key, Func<T> defaultValue=default)
        {
            var json = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(json)) return defaultValue.Invoke();

            return JsonUtility.FromJson<T>(json);
        }
    }
}