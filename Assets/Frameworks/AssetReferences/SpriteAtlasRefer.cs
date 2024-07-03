using GoPlay.AssetManagement;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace GoPlay
{
    [Serializable]
    public class SpriteAtlasRefer : AssetRefer
    {
#if UNITY_EDITOR
        private SpriteAtlas _spriteAtlas;
#endif
        
        public static implicit operator SpriteAtlas(SpriteAtlasRefer data)
        {
#if UNITY_EDITOR
            if (data._spriteAtlas) return data._spriteAtlas;
#endif
            
            if (!data) return null;
            
            var spriteAtlas = data.Load<SpriteAtlas>();
#if UNITY_EDITOR
            data._spriteAtlas = spriteAtlas;
#endif
            return spriteAtlas;
        }
        public async Task<Sprite> LoadSpriteFromAtlasAsync(string spriteName)
        {
            SpriteAtlas spriteAtlas = await AssetManager.Instance.LoadAssetAsync<SpriteAtlas>(AssetBundle, AssetName);
            if (spriteAtlas!=null)
            {
              return spriteAtlas.GetSprite(spriteName);

            }
            return null;

        }
    }
}