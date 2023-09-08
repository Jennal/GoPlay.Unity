using System;

namespace GoPlay.Data.Config
{
    public interface ILangItem
    {
        int ID { get; }
        void Deconstruct(out string text, out SpriteRefer sprite, out GameObjectRefer gameObject);
    }
}