using System;
using UnityEngine;

namespace GoPlay
{
    [Serializable]
    public class FontRefer : AssetRefer
    {
#if UNITY_EDITOR
        private Font _object;
#endif
        
        public static implicit operator Font(FontRefer data)
        {
#if UNITY_EDITOR
            if (data._object) return data._object;
#endif
            
            if (!data) return null;
            
            var obj = data.Load<Font>();
#if UNITY_EDITOR
            data._object = obj;
#endif
            return obj;
        }
    }
}