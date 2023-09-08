using GoPlay.Managers;
using GoPlay.Modules.Translate;
using UnityEngine;
using UnityEngine.UI;

namespace GoPlay.Translate
{
    public class LangImage : LangObjectBase<Sprite>
    {
        public bool NativeSize = false;

        public override void SetData()
        {
            var data = GetData();
            var img = GetComponent<Image>();
            if (img)
            {
                img.sprite = data;
                if (NativeSize) img.SetNativeSize();
                return;
            }
            
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = data;
                return;
            }
        }

        public override Sprite GetData()
        {
            return ModuleManager.Get<LangModule>().GetSprite(Id);
        }
    }
}