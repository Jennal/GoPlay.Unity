using System;
using UnityEngine;

namespace GoPlay
{
    [Serializable]
    public class SpriteRefer : AssetRefer
    {
#if UNITY_EDITOR
        private Sprite _sprite;
#endif
        
        public static implicit operator Sprite(SpriteRefer data)
        {
#if UNITY_EDITOR
            if (data._sprite) return data._sprite;
#endif
            
            if (!data) return null;
            
            var sprite = data.LoadSprite();
#if UNITY_EDITOR
            data._sprite = sprite;
#endif
            return sprite;
        }
    }
}