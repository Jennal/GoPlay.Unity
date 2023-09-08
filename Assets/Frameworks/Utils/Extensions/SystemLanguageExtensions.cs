using UnityEngine;

namespace GoPlay.Utils
{
    public static class SystemLanguageExtensions
    {
        public static string GetVariant(this SystemLanguage val)
        {
            switch (val)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseTraditional:
                    return "zh_tw";
                case SystemLanguage.ChineseSimplified:
                    return "zh_cn";
                case SystemLanguage.Afrikaans:
                case SystemLanguage.Arabic:
                case SystemLanguage.Basque:
                case SystemLanguage.Belarusian:
                case SystemLanguage.Bulgarian:
                case SystemLanguage.Catalan:
                case SystemLanguage.Czech:
                case SystemLanguage.Danish:
                case SystemLanguage.Dutch:
                case SystemLanguage.English:
                case SystemLanguage.Estonian:
                case SystemLanguage.Faroese:
                case SystemLanguage.Finnish:
                case SystemLanguage.French:
                case SystemLanguage.German:
                case SystemLanguage.Greek:
                case SystemLanguage.Hebrew:
                case SystemLanguage.Hungarian:
                case SystemLanguage.Icelandic:
                case SystemLanguage.Indonesian:
                case SystemLanguage.Italian:
                case SystemLanguage.Japanese:
                case SystemLanguage.Korean:
                case SystemLanguage.Latvian:
                case SystemLanguage.Lithuanian:
                case SystemLanguage.Norwegian:
                case SystemLanguage.Polish:
                case SystemLanguage.Portuguese:
                case SystemLanguage.Romanian:
                case SystemLanguage.Russian:
                case SystemLanguage.SerboCroatian:
                case SystemLanguage.Slovak:
                case SystemLanguage.Slovenian:
                case SystemLanguage.Spanish:
                case SystemLanguage.Swedish:
                case SystemLanguage.Thai:
                case SystemLanguage.Turkish:
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Vietnamese:
                case SystemLanguage.Unknown:
                default:
                    return "en_us";
            }
        }
    }
}