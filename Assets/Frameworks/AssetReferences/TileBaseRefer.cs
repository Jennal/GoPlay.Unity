using System;
using UnityEngine.Tilemaps;

namespace GoPlay
{
    [Serializable]
    public class TileBaseRefer : AssetRefer
    {
#if UNITY_EDITOR
        private TileBase _object;
#endif
        
        public static implicit operator TileBase(TileBaseRefer data)
        {
#if UNITY_EDITOR
            if (data._object) return data._object;
#endif
            
            if (!data) return null;
            
            var obj = data.Load<TileBase>();
#if UNITY_EDITOR
            data._object = obj;
#endif
            return obj;
        }
    }
}