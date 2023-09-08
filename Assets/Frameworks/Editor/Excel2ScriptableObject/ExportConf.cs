using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    [CreateAssetMenu(menuName="Excel/ExportConf")]
    public class ExportConf : ScriptableObject
    {
        public string xlsFolder;
        public string defaultLanguage;
        public string languages;
    }
}