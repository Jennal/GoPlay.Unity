using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GoPlay.Editor.Excel2ScriptableObject.Hooks;
using GoPlay.Editor.Excel2ScriptableObject.TypeResolvers;
using GoPlay.Editor.Utils;
using OfficeOpenXml;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using GoPlay.Editor.Excel2ScriptableObject.YamlTypeConverters;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class Excel2ScriptableObjectsWithoutCompile
    {
        public class ConfValues : List<Dictionary<string, object>> {}

        static List<TypeResolverBase> _typeResolvers = new List<TypeResolverBase>();
        static List<DataExportHookBase> _hooks = new List<DataExportHookBase>();
        static ISerializer _serializer;
        
        static readonly Dictionary<string, object> TYPE_DEFAULTS = new Dictionary<string, object>
        {
            {"int", 0},
            {"float", 0f},
            {"string", ""},
            {"bool", false},
        };
        
        public static Dictionary<string, ConfValues> finishList;
        
        const string TPL_ASSET_HEADER = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: {script_guid}, type: 3}
  m_Name: {type_name}
  m_EditorClassIdentifier: 
  Values: ";

        const string TPL_LINE_PREFIX = "  ";
        
        // [MenuItem("Excel/Export Assets (Pure)", false, 5)]
        public static void Execute()
        {
            PrepareHooks();
            PrepareResolvers();
            ExportFolder(ExporterConsts.xlsFolder);
        }

        private static void PrepareResolvers()
        {
            _typeResolvers.Clear();
            var types = ReflectionHelper.GetTypesInAllLoadedAssemblies(t => 
                t != typeof(TypeResolverBase) &&
                t != typeof(TypeResolverBase<>) && 
                (t.InheritsFrom(typeof(TypeResolverBase)) || t.InheritsFrom(typeof(TypeResolverBase<>))));

            foreach (var type in types)
            {
                var resolver = (TypeResolverBase)Activator.CreateInstance(type);
                _typeResolvers.Add(resolver);
            }
        }
        
        private static void PrepareHooks()
        {
            _hooks.Clear();
            var types = ReflectionHelper.GetTypesInAllLoadedAssemblies(t => 
                t != typeof(DataExportHookBase) &&
                t.InheritsFrom(typeof(DataExportHookBase)));

            foreach (var type in types)
            {
                var resolver = (DataExportHookBase)Activator.CreateInstance(type);
                _hooks.Add(resolver);
            }
        }

        static void ExportFolder(string folder)
        {            
            var cache = ExportCache.Load();
            _serializer = new SerializerBuilder()
                .DisableAliases()
                .WithTypeConverter(new BoolTypeConverter())
                .WithTypeConverter(new ColorTypeConverter())
                .Build();
            finishList = new Dictionary<string, ConfValues>();
//            refKeyMap = new Dictionary<string, string>();

            if (!Directory.Exists(folder))
            {
                ExporterUtils.ShowError("Excel目录不存在，请检查：" + folder);
                return;
            }

            if (!Directory.Exists(ExporterConsts.exportFolder))
            {
                ExporterUtils.ShowError("输出目录不存在，请检查：" + ExporterConsts.exportFolder);
                return;
            }

            var files = Directory.EnumerateFiles(folder, "*.*")
                                 .Where(p => ExporterConsts.extensionPattern.Any(p.EndsWith))
                                 .Where(o => !Path.GetFileName(o).StartsWith("~$") && !o.EndsWith(".converting"))
                                 .ToList();
            try
            {
                var i = 0f;
                foreach (var xls in files)
                {
                    i += 1f;
                    EditorUtility.DisplayProgressBar("", $"正在导出 {Path.GetFileNameWithoutExtension(xls)} ...",
                        i / files.Count);

                    if (cache.FilterExportScriptableObject(xls))
                    {
                        ExportFile(xls);
                    }
                }

                FixAllLanguages();
            }
            finally
            {
                EditorUtility.ClearProgressBar();                
            }

            //clear memory
            finishList = null;

            cache.RefreshExportScriptableObject(files);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ExporterUtils.ShowInfo("Export Complete!");
        }

        private static void ExportFile(string xls)
        {
            if (ExporterConsts.ignorePattern.Any(o => Path.GetFileName(xls).StartsWith(o))) return;

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

                    var ExcelWorksheetList = new List<ExcelWorksheet>();
                    foreach (var table in excelReader.Workbook.Worksheets)
                    {
                        var name = table.Name;
                        if (name == null || !name.StartsWith(ExporterConsts.exportPrefix)) continue;

//                            Debug.Log($"{xls} => {name}");
                        ExcelWorksheetList.Add(table);
                    }

                    //执行导出
                    OnAllExportBegin(xls, excelReader);
                    foreach (var table in ExcelWorksheetList)
                    {
                        Export(xls, table, excelReader);
                    }
                    OnAllExportFinish(xls, excelReader);
                }
            }
            finally
            {
                File.Delete(tmpFileName);
            }
        }

        private static void OnAllExportBegin(string xls, ExcelPackage excel)
        {
            foreach (var hook in _hooks)
            {   
                hook.OnAllExportBegin(xls, excel);
            }
        }

        private static void OnAllExportFinish(string xls, ExcelPackage excel)
        {
            foreach (var hook in _hooks)
            {
                hook.OnAllExportFinish(xls, excel);
            }
        }
      
        private static void Export(string xls, ExcelWorksheet table, ExcelPackage package)
        {
            var tableName = GetTableName(table);
            if (string.IsNullOrEmpty(tableName))
            {
                ExporterUtils.ShowError($"[错误]表名不存在：{xls} => {table.Name}");
                return;
            }

            var mainName = ExporterUtils.GetVariantMainName(tableName);
            var variantName = ExporterUtils.GetVariantName(tableName);

            var typeName = GetTypeNameByTableName(mainName);
            var asset = CreateAsset();
            var outPath = Path.Combine(ExporterConsts.exportFolder, variantName, typeName + ".asset");
            
            OnExportBegin(xls, table);
            
            FillAsset(asset, table, package);
            SaveAsset(asset, table, outPath);
            
            //Assetbundle
            try
            {
                AssetDatabase.ImportAsset(outPath, ImportAssetOptions.ForceSynchronousImport);
                var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(outPath);
                AssetDatabase.SetMainObject(obj, outPath);
                var importer = AssetImporter.GetAtPath(outPath);
                importer.SetAssetBundleNameAndVariant(ExporterConsts.assetBundleName, variantName);

                OnExportFinish(xls, table, obj);

                finishList[tableName] = asset;
            }
            catch
            {
                Debug.LogError($"导出失败：{outPath}");
                throw;
            }
        }

        private static void FixAllLanguages()
        {
            var i = 0f;
            var count = finishList.Count;
            foreach (var item in finishList)
            {
                var tableName = item.Key;
                var mainName = ExporterUtils.GetVariantMainName(tableName);
                var typeName = GetTypeNameByTableName(mainName);

                var defaultFile = Path.Combine(ExporterConsts.exportFolder, ExporterConsts.defaultVariant, typeName + ".asset");
                foreach (var variantName in ExporterConsts.languages)
                {
                    EditorUtility.DisplayProgressBar("", $"正在检查多语言 {tableName} @ {variantName} ...",
                        i / count);
                    i += 1f;
                    
                    var file = Path.Combine(ExporterConsts.exportFolder, variantName, typeName + ".asset");
                    if (File.Exists(file)) continue;
                    
                    File.Copy(defaultFile, file);
                    AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceSynchronousImport);
                    var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(file);
                    AssetDatabase.SetMainObject(obj, file);
                    var importer = AssetImporter.GetAtPath(file);
                    importer.SetAssetBundleNameAndVariant(ExporterConsts.assetBundleName, variantName);
                }
            }
        }
        
        private static void SaveAsset(ConfValues asset, ExcelWorksheet table, string outPath)
        {
            var sb = new StringBuilder();
            var header = TPL_ASSET_HEADER.Replace("{script_guid}", GetScriptGuid(table))
                                              .Replace("{type_name}", GetTypeName(table));
            sb.Append(header);

            if (asset.Count > 0)
            {
                sb.AppendLine();
                var yaml = _serializer.Serialize(asset);
                var arr = yaml.Split("\r\n").ToList();
                arr = TrimEndEmptyLines(arr);
                
                foreach (var line in arr)
                {
                    sb.AppendLine($"{TPL_LINE_PREFIX}{line}");
                }
            }
            else
            {
                sb.AppendLine(" []");
            }

            ExporterUtils.CreateFileFolderIfNotExists(outPath);
            File.WriteAllText(outPath, sb.ToString());
        }

        private static List<string> TrimEndEmptyLines(List<string> arr)
        {
            for (int i = arr.Count-1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(arr[i])) arr.RemoveAt(i);

                break;
            }

            return arr;
        }

        private static string GetScriptGuid(ExcelWorksheet table)
        {
            var typeName = GetTypeName(table);
            var filePath = Path.Combine(ExporterConsts.csFolder, typeName + ".cs");
            return AssetDatabase.AssetPathToGUID(ExporterUtils.GetProjectRelativePath(filePath));
        }

        private static string GetTypeName(ExcelWorksheet table)
        {
            var tableName = GetTableName(table);
            var mainName = ExporterUtils.GetVariantMainName(tableName);
            var typeName = GetTypeNameByTableName(mainName);
            return typeName;
        }

        private static void OnExportBegin(string xls, ExcelWorksheet table)
        {
            foreach (var hook in _hooks)
            {
                if (!hook.Recognize(xls, table)) continue;
                
                hook.OnExportBegin(xls, table);
            }
        }

        private static void OnExportFinish(string xls, ExcelWorksheet table, Object asset)
        {
            foreach (var hook in _hooks)
            {
                if (!hook.Recognize(xls, table)) continue;
                
                hook.OnExportFinish(xls, table, asset);
            }
        }

        private static string GetTableName(ExcelWorksheet table)
        {
            return table.Name.Substring(ExporterConsts.exportPrefix.Length);
        }

        private static string GetTypeNameByTableName(string tableName)
        {
            return tableName + ExporterConsts.confClassSuffix + "s";
        }

        private static ConfValues CreateAsset()
        {
            return new ConfValues();
        }

        private static void FillAsset(ConfValues asset, ExcelWorksheet table, ExcelPackage package)
        {
            var fieldNames = ExporterUtils.GetFieldNames(table);
            var fieldTypes = ExporterUtils.GetFieldTypes(table);
            var fieldPlatforms = ExporterUtils.GetFieldPlatform(table);

            var rowColumn = ExporterUtils.GetRowColumn(table);
            for (int line = ExporterConsts.LINE_START; line <= rowColumn.y; line++)
            {
                var item = new Dictionary<string, object>();

                //忽略注释行
                if(ExporterUtils.IsCommentLine(table, line)) continue;
                
                //忽略空行
                if(ExporterUtils.IsEmptyLine(table, line, rowColumn.x)) continue;
                
                for (int i = 0; i < rowColumn.x; i++)
                {
                    var platform = fieldPlatforms[i];
                    var name = fieldNames[i];
                    var type = fieldTypes[i];

                    //校验平台 c/s : 客户端/服务端
                    if (!platform.Contains(ExporterConsts.exportPlatform)) continue;
                    
                    //忽略无类型字段
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                    {
                        continue;
                    }

                    var isArray = name.EndsWith("[]");
                    if (isArray) name = name.Substring(0, name.Length - 2);
                    
                    var cell = table.Cells[line, i + 1];

                    //忽略引用类型，第二遍遍历补足
                    //!!取消对引用的支持
//                    if (type.StartsWith("ref("))
//                    {
//                        refList[name] = column;
//                        continue;
//                    }

                    try
                    {
                        if (isArray)
                        {
                            AppendItemToArray(table, type, name, item, cell);
                        }
                        else
                        {
                            //普通类型
                            object val = GetValue(table, type, cell, name);
                            item[name] = val;
                        }
                    }
                    catch (Exception err)
                    {
                        throw new Exception($"{table.Name}@[{cell.Address}] => {err}");
                    }
                }

                asset.Add(item);
            }
        }

        private static void AppendItemToArray(ExcelWorksheet table, string type, string name, Dictionary<string, object> item, ExcelRange cell)
        {
            //ignore
            if (IsIgnore(cell.GetValue<string>())) return;

            object val = GetValue(table, type, cell, name);
            var list = item.ContainsKey(name) ? (ArrayList)item[name] : new ArrayList();
            list.Add(val);
            item[name] = list;
        }

        private static object GetValue(ExcelWorksheet table, string type, ExcelRange value, string name)
        {
            foreach (var typeResolver in _typeResolvers)
            {
                if (!typeResolver.RecognizeType(type)) continue;

                if (typeResolver.IsEmpty(table, value))
                {
                    return typeResolver.Default;
                }
                else
                {
                    return typeResolver.GetValue(table, name, value);
                }
            }

            return null;
        }

        private static bool IsIgnore(string value)
        {
            return value == "-";
        }
    }
}