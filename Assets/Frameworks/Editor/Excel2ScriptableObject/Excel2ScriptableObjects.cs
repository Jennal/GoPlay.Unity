using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GoPlay.Editor.Excel2ScriptableObject.Hooks;
using GoPlay.Editor.Excel2ScriptableObject.TypeResolvers;
using GoPlay.Editor.Utils;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class Excel2ScriptableObjects
    {
        static List<TypeResolverBase> _typeResolvers = new List<TypeResolverBase>();
        static List<DataExportHookBase> _hooks = new List<DataExportHookBase>();
        
        static readonly Dictionary<string, object> TYPE_DEFAULTS = new Dictionary<string, object>
        {
            {"int", 0},
            {"float", 0f},
            {"string", ""},
            {"bool", false},
            {"GameObject", null},
            {"Sprite", null},
        };
        
        public static Dictionary<string, List<Tuple<string, string, string>>> dependancyDict =
            new Dictionary<string, List<Tuple<string, string, string>>>();

        public static Dictionary<string, Object> finishList;
        public static Dictionary<string, Object> pendingList;

        public static Dictionary<object, string> refKeyMap;
        //Struct
//        public static Dictionary<string, string> refKeyMap;

        [MenuItem("GoPlay/Excel/Clear Cache", false, 9)]
        static void ExecuteClearCacheFile() {
            FileUtil.DeleteFileOrDirectory(ExporterConsts.cacheFile);
            // Debug.Log ("缓存已清除");
            EditorUtility.DisplayDialog("", "缓存已清除", "OK");
        }
        
        [MenuItem("Assets/Export Excel to ScriptableObjects", true)]
        static bool Validate()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Path.GetExtension(path) == ".xlsx") return true;
            
            if (Directory.Exists(path))
            {
                return Directory.EnumerateFiles(path, "*.xlsx").Any();
            }

            return false;
        }
        
        [MenuItem("Assets/Export Excel to ScriptableObjects", false, 2)]
        static void ExecuteFile()
        {
            PrepareHooks();
            PrepareResolvers();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Directory.Exists(path))
            {
                ExportFolder(path);
                return;
            }
            
            dependancyDict = new Dictionary<string, List<Tuple<string, string, string>>>();
            finishList = new Dictionary<string, Object>();
            pendingList = new Dictionary<string, Object>();
            refKeyMap = new Dictionary<object, string>();
//            refKeyMap = new Dictionary<string, string>();

            if (!Directory.Exists(ExporterConsts.exportFolder))
            {
                ExporterUtils.ShowError("输出目录不存在，请检查：" + ExporterConsts.exportFolder);
                return;
            }

            ExportFile(path);

            //填充依赖
            foreach (var item in pendingList)
            {
                FillDepend(item.Key, item.Value);
            }

            //clear memory
            dependancyDict = null;
            finishList = null;
            pendingList = null;
            refKeyMap = null;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ExporterUtils.ShowInfo("Export Complete!");
        }

        [MenuItem("GoPlay/Excel/Export Assets", false, 4)]
        static void Execute()
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
            dependancyDict = new Dictionary<string, List<Tuple<string, string, string>>>();
            finishList = new Dictionary<string, Object>();
            pendingList = new Dictionary<string, Object>();
            refKeyMap = new Dictionary<object, string>();
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

                //填充依赖
                foreach (var item in pendingList)
                {
                    FillDepend(item.Key, item.Value);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();                
            }

            //clear memory
            dependancyDict = null;
            finishList = null;
            pendingList = null;
            refKeyMap = null;

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

                        //创建依赖关系
                        CreateDependancy(table);

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

        private static void FillDepend(string name, Object asset)
        {
            var i = 0;
            ForEachValues(asset, item =>
            {
                foreach (var tuple in dependancyDict[name])
                {
                    var tableName = tuple.Item1;
                    var type = tuple.Item2;
                    var fieldName = tuple.Item3;

                    var fieldType = type.Substring(4, type.IndexOf(",") - 4);
                    fieldType = ExporterUtils.FixRefType(fieldType);
                    var refFieldName = type.Substring(type.IndexOf(",") + 1, type.IndexOf(")") - type.IndexOf(",") - 1);
                    var dependAsset = finishList[tableName];
                    var fieldInfo = item.GetType().GetField(fieldName);
//                    Debug.Log(item.ToString() + "." + fieldName);
//                    Debug.Log(refKeyMap.First().Key);
                    var key = refKeyMap[item];
                    //Struct
//                    var key = refKeyMap[item.ToString() + "." + fieldName];

                    if (fieldType.EndsWith("[]"))
                    {
                        //Array
                        var arr = FindRefArray(fieldType, dependAsset, refFieldName, key);
                        fieldInfo.SetValue(item, arr);
                    }
                    else
                    {
                        //Single
                        var refItem = FindRef(dependAsset, refFieldName, key);
                        fieldInfo.SetValue(item, refItem);
                    }
                }

                var valInfo = asset.GetType().GetField("Values");
                var list = (IList) valInfo.GetValue(asset);
                list[i] = item;

                ++i;
                return false;
            });
        }

        private static object FindRefArray(string fieldType, Object dependAsset, string refFieldName, string key)
        {
            var type = ReflectionHelpers.GetTypeInAllLoadedAssemblies(fieldType.Substring(0, fieldType.Length - 2));
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);

            var result = Activator.CreateInstance(constructedListType);
            var addMethod = constructedListType.GetMethod("Add");
            var toArrayMethod = constructedListType.GetMethod("ToArray");

            ForEachValues(dependAsset, item =>
            {
                var info = item.GetType().GetField(refFieldName);
                var val = info.GetValue(item);
                if (val.ToString() == key)
                {
                    addMethod.Invoke(result, new[] {item});
                }

                return false;
            });

            return toArrayMethod.Invoke(result, null);
        }

        private static object FindRef(Object dependAsset, string refFieldName, string key)
        {
            object result = null;
            ForEachValues(dependAsset, item =>
            {
                var info = item.GetType().GetField(refFieldName);
                var val = info.GetValue(item);
                if (val.ToString() == key)
                {
                    result = item;
                    return true;
                }

                return false;
            });

            return result;
        }

        private static void ForEachValues(Object asset, Func<object, bool> action)
        {
            var fieldInfo = asset.GetType().GetField("Values");
            var list = fieldInfo.GetValue(asset);
            list = CloneList(list);
            foreach (var item in list as IEnumerable)
            {
                if (action(item)) break;
            }
        }

        private static object CloneList(object list)
        {
            var type = list.GetType().GenericTypeArguments[0];
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(type);

            var result = Activator.CreateInstance(constructedListType, list);
            return result;
        }

        private static void CreateDependancy(ExcelWorksheet table)
        {
            var names = ExporterUtils.GetFieldNames(table);
            var types = ExporterUtils.GetFieldTypes(table);
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                if (!type.StartsWith("ref(")) continue;

                var tableName = GetTableName(table);
                var refTableName = GetRefTableName(type);

                var fieldType = type;
                var fieldName = names[i];

                AddDepend(tableName, refTableName, fieldType, fieldName);
            }
        }

        private static bool HasDependency(ExcelWorksheet table)
        {
            var types = ExporterUtils.GetFieldTypes(table);
            return types.Any(o => o.StartsWith("ref("));
        }

        private static string GetRefTableName(string fieldType)
        {
            fieldType = fieldType.Substring(4, fieldType.IndexOf(",") - 4);
            if (fieldType.EndsWith("[]"))
            {
                return fieldType.Substring(0, fieldType.Length - 2);
            }

            return fieldType;
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
            var asset = CreateAsset(typeName);
            var outPath = Path.Combine(ExporterConsts.exportFolder, variantName, typeName + ".asset");
            ExporterUtils.CreateFileFolderIfNotExists(outPath);
            AssetDatabase.CreateAsset(asset, outPath);
            
            OnExportBegin(xls, table);
            FillAsset(asset, table, package);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            OnExportFinish(xls, table, asset);
            //Addressable
//            var entry = AddressableHelper.GetOrCreateEntry(outPath, ExporterConsts.addressableGroupName);
//            entry.SetLabel(ExporterConsts.addressableLabelName, true, true);
//            entry.SetAddress(typeName);
            
            //Assetbundle
            var importer = AssetImporter.GetAtPath(outPath);
            importer.SetAssetBundleNameAndVariant(ExporterConsts.assetBundleName, variantName);

            finishList[tableName] = asset;
            if (HasDependency(table))
            {
                pendingList[tableName] = asset;
            }
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

        private static Object CreateAsset(string assetName)
        {
            assetName = ExporterUtils.GetVariantMainName(assetName);
            var type = ReflectionHelpers.GetTypeInAllLoadedAssemblies(assetName);
            Assert.IsNotNull(type, $"Can't find type: {assetName}");
            var method = typeof(ScriptableObject).GetMethods()
                .FirstOrDefault(o => o.IsStatic && o.IsGenericMethod && o.Name == "CreateInstance");
            method = method.MakeGenericMethod(type);
            return method.Invoke(null, null) as Object;
        }

        private static void FillAsset(Object asset, ExcelWorksheet table, ExcelPackage package)
        {
            var valueInfo = asset.GetType().GetField("Values", BindingFlags.Public | BindingFlags.Instance);
            var valueAddMethod = valueInfo.FieldType.GetMethod("Add");
            var itemType = valueInfo.FieldType.GetGenericArguments().FirstOrDefault();
            var itemList = valueInfo.GetValue(asset);

            var fieldNames = ExporterUtils.GetFieldNames(table);
            var fieldTypes = ExporterUtils.GetFieldTypes(table);
            var fieldPlatforms = ExporterUtils.GetFieldPlatform(table);

            var rowColumn = ExporterUtils.GetRowColumn(table);
            for (int line = ExporterConsts.LINE_START; line <= rowColumn.y; line++)
            {
                var item = Activator.CreateInstance(itemType);
                var refList = new Dictionary<string, string>();

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
                    var prop = itemType.GetField(name, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) throw new Exception($"{table.Name} => {name} 字段不存在");

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
                            AppendItemToArray(table, type, name, prop, item, cell);
                        }
                        else
                        {
                            //普通类型
                            object val = GetValue(table, type, cell, name);
                            prop.SetValue(item, val);
                        }
                    }
                    catch (Exception err)
                    {
                        throw new Exception($"{table.Name}@[{cell.Address}] => {err}");
                    }
                }

                valueAddMethod.Invoke(itemList, new object[] {item});
                foreach (var pair in refList)
                {
                    refKeyMap[item] = pair.Value;
                    //Struct
//                    refKeyMap[item.ToString() + "." + pair.Key] = pair.Value;                     
                }
            }
            
            valueInfo.SetValue(asset, itemList);
        }

        private static void AppendItemToArray(ExcelWorksheet table, string type, string name, FieldInfo prop, object item, ExcelRange cell)
        {
            //ignore
            if (cell.GetValue<string>() == "-") return;
            
            object val = GetValue(table, type, cell, name);
            var arr = prop.GetValue(item);
            var listType = typeof(List<>).MakeGenericType(prop.FieldType.GetElementType());
            var list = Activator.CreateInstance(listType);
            if (arr != null) list = Activator.CreateInstance(listType, arr);
            var addMethod = list.GetType().GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
            addMethod.Invoke(list, new object[] {val});
            
            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(prop.FieldType.GetElementType());
            prop.SetValue(item, toArrayMethod.Invoke(null, new object[] {list}));
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

        static bool IsEmpty(string column)
        {
            if (string.IsNullOrEmpty(column)) return true;
            if (string.IsNullOrEmpty(column.Trim())) return true;
            if (column.Trim() == "0") return true;

            return false;
        }
        
        private static string FixAssetPath(string column)
        {
            var path = column;
            if (path.StartsWith("guid://"))
            {
                var guid = path.Substring(7, 32);
                path = AssetDatabase.GUIDToAssetPath(guid);
            }
            
            if (!path.StartsWith("Assets")) path = Path.Combine("Assets", column);
            return path;
        }

        #region Dependency

        private static void AddDepend(string name, string depend, string fieldType, string fieldName)
        {
            if (!dependancyDict.ContainsKey(name))
            {
                dependancyDict[name] = new List<Tuple<string, string, string>>();
            }

            dependancyDict[name].Add(new Tuple<string, string, string>(depend, fieldType, fieldName));
        }

        #endregion
    }
}