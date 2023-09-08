using System;
using GoPlay.Managers;
using GoPlay.Modules.Translate;

namespace GoPlay.Translate
{
    [Serializable]
    public class LangString : ICloneable
    {
        public string Text;
        public bool IsEmpty => string.IsNullOrEmpty(Text);

        public LangString(){}

        public LangString(string text)
        {
            Text = text;
        }
        
        public static implicit operator bool(LangString data)
        {
            if (data == null) return false;
            if (data.IsEmpty) return false;

            return true;
        }
        
        public static implicit operator string(LangString data)
        {
            if (data == null || string.IsNullOrEmpty(data.Text)) return "";
            
            var langSystem = ModuleManager.Check<LangModule>();
            if (!langSystem) return data.Text;

            return langSystem.GetTextByOrigin(data.Text);
        }

        public override string ToString()
        {
            return this;
        }

        public object Clone()
        {
            return new LangString(Text);
        }
    }
}