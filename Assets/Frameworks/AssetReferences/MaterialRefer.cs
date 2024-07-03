using System;
using UnityEngine;

namespace GoPlay
{
    [Serializable]
    public class MaterialRefer : AssetRefer
    {
#if UNITY_EDITOR
        private Material _material;
#endif
        
        public static implicit operator Material(MaterialRefer data)
        {
#if UNITY_EDITOR
            if (data._material) return data._material;
#endif
            
            if (!data) return null;
            
            var material = data.LoadMaterial();
#if UNITY_EDITOR
            data._material = material;
#endif
            return material;
        }
    }
}