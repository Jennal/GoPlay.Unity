using System.Collections.Generic;
using System.Linq;
using GoPlay.Data.Config;
using GoPlay.Globals;
using GoPlay.Managers;
using GoPlay.Translate;
#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using GoPlay.Modules.Base;

namespace GoPlay.Modules.Translate
{
    public partial class LangModule : ModuleBase
    {
        public static LangModule Instance => ModuleManager.Get<LangModule>();
        public const string ORIGIN_LANG = "ZHS";
        
        public string CurrentLang
        {
            get => GlobalVariables.Instance.LocalData.Language;
            set => GlobalVariables.Instance.LocalData.Language = value;
        }

        public Dictionary<string, IEnumerable<ILangItem>> Dict = new Dictionary<string, IEnumerable<ILangItem>>();
        public List<LangObject> LangObjects = new List<LangObject>();
        public bool IsReady = false;

        private List<string> _allLanguages;
        public List<string> AllLanguages {
            get
            {
                if (_allLanguages == null) InitAllLanguages();
                return _allLanguages;
            }
        } 
        
        partial void InitAllLanguages();
        
        public void Regist(LangObject obj)
        {
            LangObjects.Add(obj);
        }

        public void Unregist(LangObject obj)
        {
            LangObjects.Remove(obj);
        }

#if ODIN_INSPECTOR_3
        [Button(ButtonSizes.Large)]
#endif
        public void ChangeLang(string lang)
        {
            if (lang == CurrentLang) return;
            
            CheckLanguage(lang);
            CurrentLang = lang;

            foreach (var langObject in LangObjects)
            {
                langObject.Lang = CurrentLang;
                langObject.SetData();
            }
        }

        private void CheckLanguage(string lang)
        {
            Assert.IsTrue(Dict.ContainsKey(lang), $"Language \"{lang}\" not exists!");
        }

        public string FormatText(int id, params object[] args)
        {
            return FormatText(CurrentLang, id, args);
        }
        
        public string GetText(int id)
        {
            return GetText(CurrentLang, id);
        }

        public Sprite GetSprite(int id)
        {
            return GetSprite(CurrentLang, id);
        }

        public GameObject GetGameObject(int id)
        {
            return GetGameObject(CurrentLang, id);
        }

        public string FormatText(string lang, int id, params object[] args)
        {
            return string.Format(GetText(lang, id), args);
        }
        
        public string GetText(string lang, int id)
        {
            CheckLanguage(lang);
            var conf = Dict[lang].FirstOrDefault(o => o.ID == id);
            if (conf == null) return "NOT_TRANSLATED";
            var (text, _, _) = conf;
            return text;
        }

        public string GetTextByOrigin(string originText)
        {
            CheckLanguage(ORIGIN_LANG);
            var conf = Dict[ORIGIN_LANG].FirstOrDefault(o =>
            {
                var (text, _, _) = o;
                return text == originText;
            });
            if (conf == null) return originText;
            
            return GetText(CurrentLang, conf.ID);
        }
        
        public Sprite GetSprite(string lang, int id)
        {
            CheckLanguage(lang);
            var conf = Dict[lang].FirstOrDefault(o => o.ID == id);
            var (_, sprite, _) = conf;
            return sprite;
        }
        public Sprite GetSpriteByOrigin(Sprite originSprite)
        {
            CheckLanguage(ORIGIN_LANG);
            var conf = Dict[ORIGIN_LANG].FirstOrDefault(o =>
            {
                var (_, sprite, _) = o;
                return sprite == originSprite;
            });
            if (conf == null) return originSprite;
            
            return GetSprite(CurrentLang, conf.ID);
        }

        public GameObject GetGameObject(string lang, int id)
        {
            CheckLanguage(lang);
            var conf = Dict[lang].FirstOrDefault(o => o.ID == id);
            var (_, _, go) = conf;
            return go;
        }

        public static string GetSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.ChineseSimplified:
                    return "ZHS";
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseTraditional:
                    return "ZHT";
                default:
                    return "ENG";
            }
        }
    }
}