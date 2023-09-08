using System;
using GoPlay.Managers;
using GoPlay.Modules.Translate;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using TMPro;
using UnityEngine.UI;
using GoPlay.Utils;

namespace GoPlay.Translate
{
    public class LangText : LangObjectBase<string>
    {
        public static LangText BindTo(TextMeshProUGUI tmp, bool auto = false)
        {
            var lText = tmp.gameObject.GetOrAddComponent<LangText>();
            lText.AutoLocalize = auto;
            return lText;
        }
        
        public static LangText BindTo(Text text, bool auto = false)
        {
            var lText = text.gameObject.GetOrAddComponent<LangText>();
            lText.AutoLocalize = auto;
            return lText;
        }
        
        private string _text;
        public string text
        {
            get => _text;
            set
            {
                _text = value;
                SetData();
            }
        }
        
        private LangModule _module;
        protected LangModule Module
        {
            get
            {
                if (!_module) _module = ModuleManager.Get<LangModule>();
                return _module;
            }
        }
        
        private Text _uiText;
        private TextMeshPro _textMeshPro;
        private TextMeshProUGUI _textMeshProUgui;
#if ODIN_INSPECTOR_3
        [ShowInInspector, ReadOnly]
#endif
        string _textMatTag = "";

        string TextMatTag
        {
            get
            {
                if (_textMatTag.Equals(""))
                {
                    if (_textMeshPro)
                        _textMatTag = _textMeshPro.fontSharedMaterial.name.Substring(_textMeshPro.font.name.Length);
                    if (_textMeshProUgui)
                        _textMatTag = _textMeshProUgui.fontSharedMaterial.name.Substring(_textMeshProUgui.font.name.Length);

                    _textMatTag = _textMatTag.Replace(" (instance)", "");
                }
                return _textMatTag;
            }
        }
        
        /// <summary>
        /// 优化考虑，先判断能赋值的对象，再获取对象
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void SetData()
        {
            var data = GetData();
            
            if (_uiText)
            {
                _uiText.text = data;
                return;
            }
            
            if (_textMeshPro)
            {
                _textMeshPro.text = data;
                return;
            }
            
            if (_textMeshProUgui)
            {
                _textMeshProUgui.text = data;
                return;
            }

            _uiText = GetComponent<Text>();
            _textMeshPro = GetComponent<TextMeshPro>();
            _textMeshProUgui = GetComponent<TextMeshProUGUI>();
            if (!_uiText && !_textMeshPro && !_textMeshProUgui) return;
            
            SetData();
        }

        public void SetLocalizedText(string txtAlreadyLocalized)
        {
            if (_uiText)
            {
                _uiText.text = txtAlreadyLocalized;
                return;
            }
            
            if (_textMeshPro)
            {
                _textMeshPro.text = txtAlreadyLocalized;
                return;
            }
            
            if (_textMeshProUgui)
            {
                _textMeshProUgui.text = txtAlreadyLocalized;
                return;
            }
            
            _uiText = GetComponent<Text>();
            _textMeshPro = GetComponent<TextMeshPro>();
            _textMeshProUgui = GetComponent<TextMeshProUGUI>();
            if (!_uiText && !_textMeshPro && !_textMeshProUgui) throw new Exception("No Text/TextMeshPro/TextMeshProUGUI Component");
            
            SetLocalizedText(txtAlreadyLocalized);
        }

        public override string GetData()
        {
            var data = Module.GetText(Id);
            if (string.IsNullOrEmpty(text)) return data;

            return string.Format(data, text);
        }
    }
}