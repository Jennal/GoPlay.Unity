using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public static class ExporterConsts
    {
        public const string assetBundleName = "config";
        public const string exportPrefix = "#";
        public static readonly string[] exportEnumPrefix = new string[]{"&", "%"};
        public const string exportPlatform = "c";
        
        public const string exportVariantSplit = "@";
        public const string defaultVariant = "zh_cn";

        public const string splitOutterArr = "$";
        public const string splitOutter = "|";
        public const string splitInner = ";";
        
        public static readonly string[] xlsFolderDefaults = new[]
        {
            "../../Excels",
            "../../excels"
        };

        public static readonly string[] languageDefaults = new[]
        {
            "zh_cn",
            "en_us",
        };

        public static readonly string[] enumNamespaces = new[]
        {
            "GoPlay"
        };
        
        public static readonly string confFile = Path.Combine("Assets/Frameworks/Editor/Excel2ScriptableObject/Resources/Excel2Unity.asset");

        public static string xlsFolder
        {
            get
            {
                var conf = GetExportConf();
                if (string.IsNullOrEmpty(conf.xlsFolder)) return string.Empty;

                return Path.Combine(Application.dataPath, conf.xlsFolder);
            }
        }

        public static string defaultLanguage
        {
            get
            {
                var conf = GetExportConf();
                return conf.defaultLanguage;
            }
        }
        
        public static string[] languages
        {
            get
            {
                var conf = GetExportConf();
                if (string.IsNullOrEmpty(conf.languages)) return languageDefaults;

                return conf.languages.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }
        }
        
        public static readonly string cacheFile = Path.Combine(xlsFolder, ".exportCache");

        public static readonly string csFolder = Path.Combine(Application.dataPath, "Game/Scripts/Data/Config/Generated");
        public static readonly string enumFolder = Path.Combine(Application.dataPath, "Game/Scripts/Data/Config/Generated/Enum");
        public static readonly string mgrFile = Path.Combine(Application.dataPath, "Game/Scripts/Data/Config/Generated/Manager/ConfigData.cs");

        public const string confClassSuffix = "Conf";

        public static readonly string exportFolder = Path.Combine("Assets/Game/Res/Config");
        //        public const string confItemClassSuffix = "ConfItem";

        public static readonly string[] extensionPattern =
        {
            ".xlsx",
            ".xlsm"
        };

        public static readonly string[] ignorePattern =
        {
            "~$",
        };

        public const int LINE_TABLE_DESC = 1;
        public const int LINE_TABLE_PLATFORM = 2;
        public const int LINE_FIELD_DESC = 3;
        public const int LINE_FIELD_NAME = 4;
        public const int LINE_FIELD_TYPE = 5;
        public const int LINE_START = 6;

        public static ExportConf GetExportConf()
        {
            if (File.Exists(ExporterConsts.confFile))
            {
                return AssetDatabase.LoadAssetAtPath<ExportConf>(confFile);
            }

            var data = ScriptableObject.CreateInstance<ExportConf>();
            foreach (var folder in xlsFolderDefaults)
            {
                var path = Path.Combine(Application.dataPath, folder);
                if (Directory.Exists(path))
                {
                    data.xlsFolder = folder;
                    break;
                }
            }

            data.defaultLanguage = defaultVariant;
            data.languages = string.Join(",", languageDefaults);
            
            AssetDatabase.CreateAsset(data, confFile);
            AssetDatabase.SaveAssets();
            return data;
        }
    }
}