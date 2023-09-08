using System;
using TMPro;

namespace GoPlay
{
    [Serializable]
    public class TMP_FontAssetRefer : AssetRefer
    {
#if UNITY_EDITOR
        private TMP_FontAsset _object;
#endif
        
        public static implicit operator TMP_FontAsset(TMP_FontAssetRefer data)
        {
#if UNITY_EDITOR
            if (data._object) return data._object;
#endif
            
            if (!data) return null;
            
            var obj = data.Load<TMP_FontAsset>();
#if UNITY_EDITOR
            data._object = obj;
#endif
            return obj;
        }
    }
}