using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GoPlay.Editor.Excel2ScriptableObject.Hooks;
using GoPlay.Editor.Excel2ScriptableObject.TypeResolvers;
using GoPlay.Editor.Utils;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class Excel2Enum
    {
        [MenuItem("GoPlay/Excel/Generate Enum", false, 3)]
        public static void Execute()
        {
            if (!DoExecute()) return;
            ExporterUtils.ShowInfo("Generate Complete!");
        }
        
        public static bool DoExecute()
        {
            if (!Directory.Exists(ExporterConsts.xlsFolder))
            {
                if (ExporterUtils.ShowError("Excel目录不存在：" + ExporterConsts.xlsFolder + "\n请设置后再试...", "OK", "Cancel"))
                {
                    ExportConfEditorWindow.Open();
                }
                return false;
            }

            if (CheckConflictNames()) return false;
            
            var files = Directory.EnumerateFiles(ExporterConsts.xlsFolder, "*.*")
                                 .Where(p => ExporterConsts.extensionPattern.Any(p.EndsWith))
                                 .Where(xls => !xls.EndsWith(".converting") && 
                                               !ExporterConsts.ignorePattern.Any(o => Path.GetFileName(xls).StartsWith(o)))
                                 .ToList();
            for (var i=0; i<files.Count; i++)
            {
                var xls = files[i];
                var tmpFileName = xls + ".converting";
                if (File.Exists(tmpFileName)) File.Delete(tmpFileName);
                File.Copy(xls, tmpFileName);

                try
                {
                    using (var stream = File.Open(tmpFileName, FileMode.Open, FileAccess.Read))
                    {
                        var excelReader = new ExcelPackage(stream);
                        foreach (var sheet in excelReader.Workbook.Worksheets)
                        {
                            var name = sheet.Name;
                            if (name == null || !ExporterConsts.exportEnumPrefix.Any(o => name.StartsWith(o))) continue;

//                            Debug.Log($"{xls} => {name}");   
                            if (EditorUtility.DisplayCancelableProgressBar("",
                                    $"正在导出 {Path.GetFileNameWithoutExtension(xls)} => {name.Substring(ExporterConsts.exportPrefix.Length)} ...",
                                    (float) i / files.Count)) return false;
                            ConvertToEnum(xls, sheet);
                        }
                    }
                }
                finally
                {
                    File.Delete(tmpFileName);
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            
            return true;
        }

        private static bool CheckConflictNames()
        {
            var set = new Dictionary<string, string>();
            var err = "";
            
            foreach (var xls in Directory.EnumerateFiles(ExporterConsts.xlsFolder, "*.xlsx"))
            {
                if (ExporterConsts.ignorePattern.Any(o => Path.GetFileName(xls).StartsWith(o))) continue;

                var tmpFileName = xls + ".converting";
                if (File.Exists(tmpFileName))
                {
                    File.Delete(tmpFileName);
                }

                File.Copy(xls, tmpFileName);

                try
                {
                    using (var stream = File.Open(tmpFileName, FileMode.Open, FileAccess.Read))
                    {
                        var excelReader = new ExcelPackage(stream);
                        foreach (var sheet in excelReader.Workbook.Worksheets)
                        {
                            var name = sheet.Name;
                            if (name == null || !name.StartsWith(ExporterConsts.exportPrefix)) continue;

                            if (set.ContainsKey(name))
                            {
                                var file = set[name];
                                err += $"{name} exists in {file} and {xls}\n";
                            }

                            set[name] = xls;
                        }
                    }
                }
                finally
                {
                    File.Delete(tmpFileName);
                }
            }

            if (!string.IsNullOrEmpty(err))
            {
                EditorUtility.DisplayDialog("", err, "OK");
                return true;
            }

            return false;
        }

        static void ConvertToEnum(string xls, ExcelWorksheet table)
        {
            var tableName = table.Name.Substring(ExporterConsts.exportPrefix.Length);
            if (string.IsNullOrEmpty(tableName))
            {
                ExporterUtils.ShowError($"[错误]表名不存在：{xls} => {table.Name}");
                return;
            }

            var rowColumns = ExporterUtils.GetRowColumn(table);
            ConvertTable(xls, table, rowColumns);
        }

        static void ConvertTable(string xls, ExcelWorksheet table, Vector2Int rowColumn)
        {
            var tableName = table.Name.Substring(ExporterConsts.exportPrefix.Length);
            if (rowColumn.x <= 0 || rowColumn.y <= 0) return;

            var entityName = table.Name.Substring(1);
            var isFlag = table.Name.StartsWith("&");

            var fields = "";
            for (var i = 3; i <= rowColumn.y; i++)
            {
                var val = table.Cells[i, 1].GetValue<string>();
                var name = table.Cells[i, 2].GetValue<string>();
                var comment = table.Cells[i, 3].GetValue<string>();

                fields += $"        {name} = {val},";
                if (!string.IsNullOrEmpty(comment)) fields += $" //{comment}";
                if (i != rowColumn.y) fields += "\n";
            }

            var xlsPath = ExporterUtils.GetProjectRelativePath(xls);
            var enumName = $"public enum {entityName}";
            if (isFlag) enumName = "[Flags]\n    " + enumName;
            var content = $@"// Code generated by Excel2ScriptableObject. DO NOT EDIT.
// source file: {xlsPath}
// source sheet: {table.Name}

using System;

namespace GoPlay.Data.Config {{
    {enumName}
    {{
{fields}
    }}
}}
";

            WriteEntityFile(entityName, content);
        }

        static string WriteEntityFile(string entityName, string content)
        {
            ExporterUtils.Log("写入文件：" + entityName);
            var path = Path.Combine(ExporterConsts.enumFolder, entityName + ".cs");
            ExporterUtils.CreateFileFolderIfNotExists(path);
            File.Delete(path);
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }
    }
}