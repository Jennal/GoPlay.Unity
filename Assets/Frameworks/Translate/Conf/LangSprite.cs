using System;
using GoPlay.Managers;
using GoPlay.Modules.Translate;
using UnityEngine;

namespace GoPlay.Translate
{
    [Serializable]
    public class LangSprite
    {
        public Sprite Sprite;

        public static implicit operator Sprite(LangSprite data)
        {
            var langSystem = ModuleManager.Check<LangModule>();
            if (!langSystem) return data.Sprite;

            return langSystem.GetSpriteByOrigin(data.Sprite);
        }
    }
}