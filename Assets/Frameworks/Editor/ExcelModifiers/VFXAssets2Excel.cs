using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using GoPlay.Attributes;
using GoPlay.Editor.Excel2ScriptableObject;
using GoPlay.Services;

namespace GoPlay.Editor.ExcelModifiers
{
    public class VFXAssets2Excel
    {
        public static readonly string xlsFile = Path.Combine(ExporterConsts.xlsFolder, "特效表.xlsx");
        public static readonly string xlsSheet = "#VFXAssets@ZH_CN";
        public static readonly string[] projectVFXdirs = new []
        {
            "Assets/Game/Res",
            "Assets/Res"
        };
        public static int xlsStartLine => ExporterConsts.LINE_START;
        
        [MenuItem("GoPlay/Excel/Export VFXAssets", false)]
        public static void Execute()
        {
            ExportExcelSheet();
            ExporterUtils.ShowInfo("Generate Complete!");
        }

        static void ExportExcelSheet()
        {
            var wb = new ExcelPackage(new FileInfo(xlsFile));
            var sheet = wb.Workbook.Worksheets[xlsSheet];

            var line = xlsStartLine;
            var progress = 0f;

            var assets = new List<string>();
            foreach (var projectVfxDir in projectVFXdirs)
            {
                if (!Directory.Exists(projectVfxDir)) continue;
                
                var dirInfo = new DirectoryInfo(projectVfxDir);
                var assetPaths = dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories).Select(o => o.FullName).ToList();
                assets.AddRange(assetPaths);
            }

            var index = 1;
            foreach (var assetFullPath in assets)
            {
                var path = ExporterUtils.GetProjectRelativePath(assetFullPath);
                var cls = AssetDatabase.LoadAssetAtPath<PoolableMonoBehaviour>(path);
                if ( ! Verify(cls)) continue;
             
                progress += 1f / assets.Count;
                if (EditorUtility.DisplayCancelableProgressBar("Exporting", assetFullPath, progress)) break;
                
                var particles =  cls.GetComponentsInChildren<ParticleSystem>();
                var duration = particles.Length <= 0 ? 0f :
                    particles.Select(o => o.main.duration + o.main.startDelay.constant).Max();
                
                sheet.Cells[line, 1].Value = index;
                sheet.Cells[line, 2].Value = cls.GetType().Name;
                sheet.Cells[line, 3].Value = duration;
                sheet.Cells[line, 4].Value = path;
                
                index++;
                line++;
            }
            EditorUtility.ClearProgressBar();

            while (sheet.Cells[line, 1].Value != null)
            {
                sheet.DeleteRow(line);
                line++;
            }
            
            wb.Save();
        }

        private static bool Verify(PoolableMonoBehaviour cls)
        {
            if (!cls) return false;

            var attrs = cls.GetType().GetCustomAttributes(typeof(ExportToExcel), true);
            return attrs.Length > 0;
        }
    }
}