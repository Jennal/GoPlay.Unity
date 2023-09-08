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
    public class Excel2CSharp
    {
        static List<TypeResolverBase> _typeResolvers = new List<TypeResolverBase>();
        private static HashSet<string> _finishedTypeNames = new HashSet<string>();
        private static List<ScriptGenHookBase> _hooks = new List<ScriptGenHookBase>();

        private static readonly string[] BASIC_TYPES = new[]
        {
            "int",
            "float",
            "bool",
            "string"
        };

        private static readonly string[] BASIC_NAMESPACES = new[]
        {
            "System",
            "System.Linq",
            "System.Text",
            "System.Collections.Generic",
            "UnityEngine",
        };

        private const string FIELD_DESC_PREFIX = "		/// ";
        
        const string FIELDS_TEMPLETE = @"
		/// <summary>
{fieldDesc}
		/// </summary>
		public {fieldType} {fieldName};
";

        const string SB_FIELDS_TEMPLETE = @"
			sb.AppendFormat(""{fieldName}: {0}, "", {fieldName});";
        
        const string SB_FIELDS_ARRAY_TEMPLETE = @"
			sb.AppendFormat(""{fieldName}: [{0}], "", string.Join("", "", {fieldName}.Select(o => o.ToString())));";

        const string EQ_FIELDS_TEMPLETE = @"
			if ({fieldName} != o.{fieldName}) return false;";
        
        const string EQ_FIELDS_ARRAY_TEMPLETE = @"
            if ({fieldName} == null && o.{fieldName} != null) return false;
            if ({fieldName} != null && o.{fieldName} == null) return false;
            if ({fieldName} != null && o.{fieldName} != null) {
                if ({fieldName}.Length != o.{fieldName}.Length) return false;
                for (var i=0; i<{fieldName}.Length; i++) {
			        if ({fieldName}[i] != o.{fieldName}[i]) return false;
                }
            }";

        const string EQ_FLOAT_FIELDS_TEMPLETE = @"
			if (Mathf.Approximately({fieldName}, o.{fieldName}) == false) return false;";
        
        const string EQ_FLOAT_FIELDS_ARRAY_TEMPLETE = @"
            if ({fieldName} == null && o.{fieldName} != null) return false;
            if ({fieldName} != null && o.{fieldName} == null) return false;
            if ({fieldName} != null && o.{fieldName} != null) {
                if ({fieldName}.Length != o.{fieldName}.Length) return false;
                for (var i=0; i<{fieldName}.Length; i++) {
			        if (Mathf.Approximately({fieldName}[i], o.{fieldName}[i]) == false) return false;
                }
            }";

        const string CLONE_FIELDS_TEMPLATE = @"
				{fieldName} = {cloneValue},";

        private const string NAMESPACE_TEMPLATE = @"using {namespace};
";

        const string CLASS_TEMPLETE =
            @"// Code generated by Excel2ScriptableObject. DO NOT EDIT.
// source file: {excelName}
// source sheet: {tableName}

{namespaces}
namespace GoPlay.Data.Config
{
    /// <summary>
	/// {entityName}表单行结构
    ///
	/// {tableDesc}
	/// </summary>
	[Serializable]
	public partial struct {entityName}
	{
		{fields}
		
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(""{entityName} : { "");
			{sbFields}
			sb.Append("" }"");
			return sb.ToString();
		}
		
		public override bool Equals(object obj)
		{
			if (obj is {entityName} == false) return false;
			
			var o = ({entityName})obj;
			{eqFields}
			
			return true;
		}

        public override int GetHashCode()
		{
			return base.GetHashCode();
		}

        public static bool operator ==({entityName} lhs, {entityName} rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=({entityName} lhs, {entityName} rhs)
		{
			return !lhs.Equals(rhs);
		}
	}

    /// <summary>
    /// {entityName}表结构
    ///
	/// {tableDesc}
	/// </summary>
	[Serializable]
	[CreateAssetMenu(menuName=""ExcelObject/{entityName}"")]
	public class {entityName}s : ScriptableObject
	{	
		public List<{entityName}> Values = new List<{entityName}>(); 
	}
}
";

        private const string FIELDS_MANAGER_TEMPLATE =
            @"        [ShowInInspector] private {typeName}s {privateName};
        public static List<{typeName}> {typeName}s {
            get 
            {                
                if (Instance.{privateName} == null)
                {
                    Instance.{privateName} = ConfigLoader.LoadConfig<{typeName}s>(""{typeName}s"");
                }

                return Instance.{privateName}.Values;
            }
        }
";

        const string CLASS_MANAGER_TEMPLETE =
            @"// Code generated by Excel2ScriptableObject. DO NOT EDIT.
using System.Collections.Generic;
using GoPlay.AssetManagement.Loaders;
using GoPlay.Data.Config;
using Sirenix.OdinInspector;

namespace GoPlay.Managers
{
    public partial class ConfigData
    {
{fields}
    }
}

";

        [MenuItem("Excel/Generate Code", false, 3)]
        public static void Execute()
        {
            if (!DoExecute()) return;
            ExporterUtils.ShowInfo("Generate Complete!");
        }
        
        public static bool DoExecute()
        {
            PrepareResolvers();
            PrepareHooks();
            
            var cache = ExportCache.Load();
            _finishedTypeNames = new HashSet<string>();

            if (!Directory.Exists(ExporterConsts.xlsFolder))
            {
                if (ExporterUtils.ShowError("Excel目录不存在：" + ExporterConsts.xlsFolder + "\n请设置后再试...", "OK", "Cancel"))
                {
                    ExportConfEditorWindow.Open();
                }
                return false;
            }

            CheckConflictNames();

            var files = Directory.EnumerateFiles(ExporterConsts.xlsFolder, "*.*")
                                 .Where(p => ExporterConsts.extensionPattern.Any(p.EndsWith))
                                 .Where(xls => !xls.EndsWith(".converting") && 
                                               !ExporterConsts.ignorePattern.Any(o => Path.GetFileName(xls).StartsWith(o)))
                                 .ToList();
            for (var i=0; i<files.Count; i++)
            {
                var xls = files[i];
                if (!cache.FilterExportCSharp(xls))
                {
                    foreach (var entity in cache.GetSheetEntities(xls))
                    {
                        var typeName = ExporterUtils.GetVariantMainName(entity);
                        _finishedTypeNames.Add(typeName);
                    }
                    continue;
                }

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

//                            Debug.Log($"{xls} => {name}");   
                            if (EditorUtility.DisplayCancelableProgressBar("",
                                    $"正在导出 {Path.GetFileNameWithoutExtension(xls)} => {name.Substring(ExporterConsts.exportPrefix.Length)} ...",
                                    (float) i / files.Count)) return false;
                            ConvertToClasses(xls, sheet);
                        }
                    }
                }
                finally
                {
                    File.Delete(tmpFileName);
                }
            }

            CreateManagerCode();
            HookAllFinish();
            cache.RefreshExportCSharp(files);

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            _finishedTypeNames = null;
            
            return true;
        }

        private static void HookAllFinish()
        {
            foreach (var hook in _hooks)
            {
                hook.OnExportAllFinished();
            }
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
                t != typeof(ScriptGenHookBase) &&
                t.InheritsFrom(typeof(ScriptGenHookBase)));

            foreach (var type in types)
            {
                var resolver = (ScriptGenHookBase)Activator.CreateInstance(type);
                _hooks.Add(resolver);
            }
        }

        private static void CheckConflictNames()
        {
            var set = new Dictionary<string, string>();
            
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
                                var err = $"{name} exists in {file} and {xls}";
                                EditorUtility.DisplayDialog("", err, "OK");
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
        }

        private static void CreateManagerCode()
        {
            var fields = "";

            foreach (var typeName in _finishedTypeNames)
            {
                var privateName = ExporterUtils.ToPrivateName(typeName);
                var propertyName = ExporterUtils.ToPropertyName(typeName);
                fields += FIELDS_MANAGER_TEMPLATE
                    .Replace("{typeName}", typeName)
                    .Replace("{propertyName}", propertyName)
                    .Replace("{privateName}", privateName);
            }

            var code = CLASS_MANAGER_TEMPLETE
                .Replace("{fields}", fields)
                .Replace("{assetBundle}", ExporterConsts.assetBundleName);
            WriteManagerFile(code);
        }

        static void ConvertToClasses(string xls, ExcelWorksheet table)
        {
            var tableName = table.Name.Substring(ExporterConsts.exportPrefix.Length);
            var entityName = ExporterUtils.EntityNameFromTable(table);
            if (_finishedTypeNames.Contains(entityName)) return;
            
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
            HookBegin(xls, table, rowColumn);
            
            var tableName = table.Name.Substring(ExporterConsts.exportPrefix.Length);
            if (rowColumn.x <= 0 || rowColumn.y <= 0) return;

            var tableDesc = FixMultilineComment(table.Cells[ExporterConsts.LINE_TABLE_DESC, 1].GetValue<string>());
            var fieldNames = ExporterUtils.GetFieldNames(table);
            var fieldTypes = ExporterUtils.GetFieldTypes(table);
            var fieldDescs = ExporterUtils.GetFieldDescs(table);
            var fieldPlatforms = ExporterUtils.GetFieldPlatform(table);
            var entityName = ExporterUtils.EntityNameFromTable(table);

            //name => exists
            var arrDict = new Dictionary<string, bool>();

            var namespaces = GetBasicNamespaces();
            var fields = "";
            var sbFields = "";
            var eqFields = "";
            var cloneFields = "";
            for (var i = 0; i < rowColumn.x; i++)
            {
                var platform = fieldPlatforms[i];
                var name = fieldNames[i];
                var type = fieldTypes[i];
                var desc = fieldDescs[i];
                var isArray = false;

                //校验平台 c/s : 客户端/服务端
                if (!platform.Contains(ExporterConsts.exportPlatform)) continue;
                
                //忽略为空的字段
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)) continue;

                //Type Resolver
                var resolver = GetResolver(xls, table, type);
                
                //重设类型
                type = resolver.TypeName;
                
                //自动引用名称空间
                var typeNs = resolver.Namespace;
                if (!string.IsNullOrEmpty(typeNs))
                {
                    if (!BASIC_TYPES.Contains(type) && !BASIC_NAMESPACES.Contains(typeNs))
                    {
                        var ns = NAMESPACE_TEMPLATE.Replace("{namespace}", typeNs);
                        if (!namespaces.Contains(ns)) namespaces += ns;
                        Debug.Log($"--------------------> {type} => {typeNs}");
                    }
                }
                else
                {
                    var csType = ReflectionHelpers.GetTypeInAllLoadedAssemblies(type);
                    if (!BASIC_TYPES.Contains(type))
                    {
                        if (csType != null)
                        {
                            if (!string.IsNullOrEmpty(csType.Namespace) && !BASIC_NAMESPACES.Contains(csType.Namespace))
                            {
                                var ns = NAMESPACE_TEMPLATE.Replace("{namespace}", csType.Namespace);
                                if (!namespaces.Contains(ns)) namespaces += ns;
                                Debug.Log($"--------------------> {type} => {csType.Namespace}");
                            }
                        }
                    }
                }

                var fieldName = ExporterUtils.ToCamelCase(name);
                string fieldType = resolver.TypeName;
                if (!GetFieldType(type, resolver.Type, out fieldType))
                {
                    ExporterUtils.ShowError("[错误]存在错误字段类型：" + type + " - " + tableName + "." + name);
                    return;
                }

                if (fieldName.EndsWith("[]"))
                {
                    if (arrDict.ContainsKey(fieldName)) continue;

                    arrDict[fieldName] = true;
                    fieldName = fieldName.Substring(0, fieldName.Length - 2);
                    fieldType = $"{fieldType}[]";

                    isArray = true;
                }

                desc = string.Join("\n", desc.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => $"{FIELD_DESC_PREFIX}{o}"));
                
                fields += FIELDS_TEMPLETE
                    .Replace("{fieldName}", fieldName)
                    .Replace("{fieldType}", fieldType)
                    .Replace("{fieldDesc}", desc);

                //ToString
                if (isArray) sbFields += SB_FIELDS_ARRAY_TEMPLETE.Replace("{fieldName}", fieldName);
                else sbFields += SB_FIELDS_TEMPLETE.Replace("{fieldName}", fieldName);

                //Equals
                if (fieldType == "float")
                {
                    if (isArray) eqFields += EQ_FLOAT_FIELDS_ARRAY_TEMPLETE.Replace("{fieldName}", fieldName);
                    else eqFields += EQ_FLOAT_FIELDS_TEMPLETE.Replace("{fieldName}", fieldName);
                }
                else
                {
                    if (isArray) eqFields += EQ_FIELDS_ARRAY_TEMPLETE.Replace("{fieldName}", fieldName);
                    else eqFields += EQ_FIELDS_TEMPLETE.Replace("{fieldName}", fieldName);
                }

                //Clone
                var cloneValue = resolver.GetScriptClone(fieldName);
                cloneFields += CLONE_FIELDS_TEMPLATE
                    .Replace("{fieldName}", fieldName)
                    .Replace("{cloneValue}", cloneValue);
            }

            string xlsPath = ExporterUtils.GetProjectRelativePath(xls);
            string content = CLASS_TEMPLETE
                .Replace("{namespaces}", namespaces)
                .Replace("{excelName}", xlsPath)
                .Replace("{tableName}", tableName)
                .Replace("{tableDesc}", tableDesc)
                .Replace("{entityName}", entityName)
                .Replace("{cloneFields}", cloneFields)
                .Replace("{fields}", fields)
                .Replace("{sbFields}", sbFields)
                .Replace("{eqFields}", eqFields);

            var path = WriteEntityFile(entityName, content);
            _finishedTypeNames.Add(entityName);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var guid = AssetDatabase.AssetPathToGUID(ExporterUtils.GetProjectRelativePath(path));
            Debug.Log($"{path} => {guid}");
            
            HookFinish(xls, table, rowColumn, path);
        }

        private static void HookFinish(string xls, ExcelWorksheet table, Vector2Int rowColumn, string path)
        {
            foreach (var hook in _hooks)
            {
                if (!hook.Recognize(xls, table, rowColumn)) continue;
                hook.OnExportFinish(xls, table, rowColumn, path);
            }
        }

        private static void HookBegin(string xls, ExcelWorksheet table, Vector2Int rowColumn)
        {
            foreach (var hook in _hooks)
            {
                if (!hook.Recognize(xls, table, rowColumn)) continue;
                hook.OnExportBegin(xls, table, rowColumn);
            }
        }

        private static TypeResolverBase GetResolver(string xls, ExcelWorksheet table, string typeName)
        {
            foreach (var resolver in _typeResolvers)
            {
                if (resolver.RecognizeType(typeName)) return resolver;
            }

            throw new Exception($"无法识别类型：{typeName}\n\n{xls}\n{table.Name}");
        }
        
        private static string GetBasicNamespaces()
        {
            var arr = BASIC_NAMESPACES.Select(o => NAMESPACE_TEMPLATE.Replace("{namespace}", o));
            return string.Join("", arr);
        }

        private static bool GetFieldType(string type, Type csType, out string fieldType)
        {
            //引用类型: ref(TypeName,FieldName)
            if (type.StartsWith("ref("))
            {
                fieldType = type.Substring(4, type.IndexOf(",") - 4);
                fieldType = ExporterUtils.FixRefType(fieldType);
                return true;
            }

            //普通类型
//            return TYPE_MAP.TryGetValue(type, out fieldType);
            if (csType == null || BASIC_TYPES.Contains(type) || BASIC_TYPES.Contains(type.Replace("[]", "")))
            {
                fieldType = type;
            }
            else
            {
                fieldType = csType.FullName
                    .Replace(csType.Namespace + ".", "")
                    .Replace("+", ".");
                
                //类型会被转成普通类型传进来，
                // if (type.EndsWith("[]")) fieldType += "[]";
            }
            
            return true;
        }

        static string WriteEntityFile(string entityName, string content)
        {
            ExporterUtils.Log("写入文件：" + entityName);
            var path = Path.Combine(ExporterConsts.csFolder, entityName + "s.cs");
            ExporterUtils.CreateFileFolderIfNotExists(path);
            File.Delete(path);
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }

        static void WriteManagerFile(string content)
        {
            var path = ExporterConsts.mgrFile;
            var fileName = Path.GetFileNameWithoutExtension(path);
            ExporterUtils.Log("写入文件：" + fileName);
            ExporterUtils.CreateFileFolderIfNotExists(path);
            File.Delete(path);
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        static string FixMultilineComment(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return string.Empty;
            
            var arr = comment.Split("\n".ToCharArray());
            for (var i = 1; i < arr.Length; i++)
            {
                arr[i] = "	/// " + arr[i];
            }

            return string.Join("\r\n", arr);
        }
    }
}