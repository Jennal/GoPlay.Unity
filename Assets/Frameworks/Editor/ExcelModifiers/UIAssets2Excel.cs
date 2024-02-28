using System.Collections.Generic;
using System.IO;
using GoPlay.Editor.Excel2ScriptableObject;
using GoPlay.Framework.UI;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;

namespace GoPlay.Editor.ExcelModifiers
{
    public class UIAssets2Excel
    {
        public static readonly string xlsFile = Path.Combine(ExporterConsts.xlsFolder, "UI资源表.xlsx");
        public static readonly string xlsSheet = "#UIAssets";
        public static int xlsStartLine => ExporterConsts.LINE_START;
        
        [MenuItem("GoPlay/Excel/Export UIAssets", false)]
        public static void Execute()
        {
            ExportExcelSheet();
            ExporterUtils.ShowInfo("Generate Complete!");
        }

        static void ExportExcelSheet()
        {
            var set = new HashSet<string>();
            var wb = new ExcelPackage(new FileInfo(xlsFile));
            var sheet = wb.Workbook.Worksheets[xlsSheet];

            var line = xlsStartLine;
            var names = AssetDatabase.GetAllAssetBundleNames();
            var progress = 0f;
            foreach (var assetbundle in names)
            {
                progress += 1f / names.Length;
                EditorUtility.DisplayProgressBar("Exporting", assetbundle, progress);
                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetbundle);
                foreach (var assetPath in assets)
                {
                    if (set.Contains(assetPath)) continue;
                    
                    UIPanel cls = AssetDatabase.LoadAssetAtPath<UIWindow>(assetPath);
                    if (!cls) cls = AssetDatabase.LoadAssetAtPath<UIPoolableWindow>(assetPath);
                    if (!cls) continue;

                    // Debug.Log($"Checking {assetPath}");
                    set.Add(assetPath);
                    
                    var assetName = Path.GetFileNameWithoutExtension(assetPath);
                    var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetbundle, assetName);
                    var guid = AssetDatabase.AssetPathToGUID(paths[0]);

                    sheet.Cells[line, 1].Value = guid;
                    sheet.Cells[line, 2].Value = assetbundle;
                    sheet.Cells[line, 3].Value = assetPath;
                    sheet.Cells[line, 4].Value = cls.GetType().Name;

                    line++;
                }
            }
            EditorUtility.ClearProgressBar();

            while (sheet.Cells[line, 1].Value != null)
            {
                sheet.DeleteRow(line);
                line++;
            }
            
            wb.Save();
        }
    }
}