using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    /// <summary>
    /// 为了避免退出Application时，再次创建Instance
    /// </summary>
    protected static bool s_instantiated = false;
    protected static T s_instance;
    public static T Instance
    {
        get
        {
            if (!s_instantiated && s_instance == null)
            {
                var go = new GameObject(typeof(T).Name);
                s_instance = go.AddComponent<T>();
                s_instantiated = true;
            }
            
            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        gameObject.name = typeof(T).Name;
        s_instance = this as T;
    }

    private void OnDestroy()
    {
        if (s_instance == this) s_instantiated = false;
    }
}