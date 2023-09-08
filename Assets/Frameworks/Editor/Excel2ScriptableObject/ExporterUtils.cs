using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public static class ExporterUtils
    {
        public static bool IsEmptyLine(ExcelWorksheet table, int line, int fieldNamesCount)
        {
            for (var i = 1; i <= fieldNamesCount; i++)
            {
                var cell = table.Cells[line, i];
                if (cell.Value == null) continue;
                if (cell.GetValue<string>().Trim() != "") return false;
            }

            return true;
        }

        public static bool IsCommentLine(ExcelWorksheet table, int line)
        {
            try
            {
                var str = table.Cells[line, 1].GetValue<string>();
                if (str == null) return false;
                return str.StartsWith("//");
            }
            catch
            {
                Debug.LogError($"单元格数据无法读取: {table.Name}: Row {line}");
                throw;
            }
        }

        public static string FixPath(string column)
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

        public static bool IsEmptyColumn(string column)
        {
            if (string.IsNullOrEmpty(column)) return true;
            if (string.IsNullOrEmpty(column.Trim())) return true;
//            if (column.Trim() == "0") return true;

            return false;
        }

        public static List<string> GetFieldTypes(ExcelWorksheet table)
        {
            return GetListAtRow(table, ExporterConsts.LINE_FIELD_TYPE);
        }

        public static List<string> GetFieldNames(ExcelWorksheet table)
        {
            return GetListAtRow(table, ExporterConsts.LINE_FIELD_NAME);
        }

        public static List<string> GetFieldDescs(ExcelWorksheet table)
        {
            return GetListAtRow(table, ExporterConsts.LINE_FIELD_DESC);
        }
        
        public static List<string> GetFieldPlatform(ExcelWorksheet table)
        {
            return GetListAtRow(table, ExporterConsts.LINE_TABLE_PLATFORM);
        }

        public static List<string> GetListAtRow(ExcelWorksheet table, int row)
        {
            var rowColumn = GetRowColumn(table);
            var list = new List<string>();
            for (int i = 1; i <= rowColumn.x; i++)
            {
                var val = table.Cells[row, i].Value;
                list.Add(val != null ? val.ToString() : "");
            }

            return list;
        }

        public static Vector2Int GetRowColumn(ExcelWorksheet table)
        {
            var result = Vector2Int.zero;
            foreach (var cell in table.Cells)
            {
                result.x = Mathf.Max(cell.End.Column, result.x);
                result.y = Mathf.Max(cell.End.Row, result.y);
            }

            return result;
        }

        public static string FixRefType(string fieldType)
        {
            if (fieldType.EndsWith("[]"))
            {
                return fieldType.Substring(0, fieldType.Length - 2) + ExporterConsts.confClassSuffix + "[]";
            }

            return fieldType + ExporterConsts.confClassSuffix;
        }

        public static string ToPrivateName(string name)
        {
            if (name.StartsWith("_")) return name;

            return "_" + char.ToLower(name[0]) + name.Substring(1);
        }

        public static string ToPropertyName(string name)
        {
            const string suffix = "Conf";
            return name.EndsWith(suffix) ? name.Replace(suffix, string.Empty) : name;
        }

        public static string ToCamelCase(string str)
        {
            var temp = str.Split('_');
            for (var i = 1; i < temp.Length; i++)
            {
                temp[i] = char.ToUpper(temp[i][0]) + temp[i].Substring(1);
            }

            return string.Join("", temp);
        }

        public static string ToPascalCase(string str)
        {
            var temp = str.Split('_');
            for (var i = 0; i < temp.Length; i++)
            {
                temp[i] = char.ToUpper(temp[i][0]) + temp[i].Substring(1);
            }

            return string.Join("", temp);
        }

        public static bool ShowError(string msg, string ok="OK", string cancel=null)
        {
            return EditorUtility.DisplayDialog("Error", msg, ok, cancel);
        }

        public static bool ShowInfo(string msg)
        {
            return EditorUtility.DisplayDialog("Info", msg, "OK");
        }

        public static void Log(string msg)
        {
            Debug.Log(msg);
        }

        public static int ConvertInt32(string tableName, string name, string type, int row, string val)
        {
            int result = 0;
            if (int.TryParse(val, out result))
            {
                return result;
            }

            throw new Exception(ErrFormat(tableName, name, type, row, val));
        }
        
        public static long ConvertInt64(string tableName, string name, string type, int row, string val)
        {
            long result = 0;
            if (long.TryParse(val, out result))
            {
                return result;
            }

            throw new Exception(ErrFormat(tableName, name, type, row, val));
        }

        /// <summary>
        /// 1, 2, 3, 4
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int[] ConvertInt32Array(string tableName, string name, string type, int row, string val, string separator="")
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var result = new List<int>();
            var arr = val.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in arr)
            {
                var v = -1;
                if (!int.TryParse(item.Trim(), out v))
                {
                    throw new Exception(ErrFormat(tableName, name, type, row, val + $" => '{item}'"));
                }

                result.Add(v);
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// 0.1,0.1,0.2,0.4
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static float[] ConvertFloatArray(string tableName, string name, string type, int row, string val, string separator="")
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var result = new List<float>();
            var arr = val.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in arr)
            {
                if (!float.TryParse(item.Trim(), out var v))
                {
                    throw new Exception(ErrFormat(tableName, name, type, row, val + $" => '{item}'"));
                }

                result.Add(v);
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// 1, 2, 3, 4
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string[] ConvertStringArray(string tableName, string name, string type, int row, string val, string separator="")
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var result = new List<string>();
            var arr = val.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in arr)
            {
                result.Add(item.Trim());
            }

            return result.ToArray();
        }

        /// <summary>
        /// 1, 2
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Vector2 ConvertVector2(string tableName, string name, string type, int row, string val, string separator="")
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var result = new Vector2();
            var arr = val.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 2) throw new Exception(ErrFormat(tableName, name, type, row, val));

            for (var i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                var v = 0f;
                if (!float.TryParse(item.Trim(), out v))
                {
                    throw new Exception(ErrFormat(tableName, name, type, row, val + $" => '{item}'"));
                }

                result[i] = v;
            }

            return result;
        }
        
        /// <summary>
        /// 1, 2
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Vector2Int ConvertVector2Int(string tableName, string name, string type, int row, string val, string separator="")
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var result = new Vector2Int();
            var arr = val.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 2) throw new Exception(ErrFormat(tableName, name, type, row, val));

            for (var i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                var v = 0;
                if (!int.TryParse(item.Trim(), out v))
                {
                    throw new Exception(ErrFormat(tableName, name, type, row, val + $" => '{item}'"));
                }

                result[i] = v;
            }

            return result;
        }
        
        /// <summary>
        /// 1, 2, 3
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Vector3 ConvertVector3(string tableName, string name, string type, int row, string val)
        {
            var result = new Vector3();
            var arr = val.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 3) throw new Exception(ErrFormat(tableName, name, type, row, val));

            for (var i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                var v = 0f;
                if (!float.TryParse(item.Trim(), out v))
                {
                    throw new Exception(ErrFormat(tableName, name, type, row, val + $" => '{item}'"));
                }

                result[i] = v;
            }

            return result;
        }
        
        /// <summary>
        /// 1, 2, 3
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Vector3Int ConvertVector3Int(string tableName, string name, string type, int row, string val, string separator="")
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var result = new Vector3Int();
            var arr = val.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length != 3) throw new Exception(ErrFormat(tableName, name, type, row, val));

            for (var i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                var v = 0;
                if (!int.TryParse(item.Trim(), out v))
                {
                    throw new Exception(ErrFormat(tableName, name, type, row, val + $" => '{item}'"));
                }

                result[i] = v;
            }

            return result;
        }

        public static float ConvertFloat(string tableName, string name, string type, int row, string val)
        {
            float result = 0;
            if (float.TryParse(val, out result))
            {
                return result;
            }

            throw new Exception(ErrFormat(tableName, name, type, row, val));
        }

        public static bool ConvertBool(string tableName, string name, string type, int row, string val)
        {
            bool result = false;
            if (bool.TryParse(val, out result))
            {
                return result;
            }

            int intVal = 0;
            if (int.TryParse(val, out intVal))
            {
                return intVal != 0;
            }

            throw new Exception(ErrFormat(tableName, name, type, row, val));
        }

        public static T ConvertEnum<T>(string tableName, string name, string type, int row, string val)
            where T : Enum
        {
            //数字类型
            if (int.TryParse(val, out var intVal))
            {
                if (!typeof(T).GetEnumValues().OfType<T>().Any(o => o.Equals((T) (intVal as object))))
                {
                    Debug.LogError(ErrFormat(tableName, name, type, row, val) +
                                   ", value not in enum values, use default");
                    return default;
                }

                return (T) (intVal as object);
            }
            
            //字符类型
            if (!typeof(T).GetEnumNames().Any(o => o.ToLower().Equals(val.ToLower())))
            {
                Debug.LogError(ErrFormat(tableName, name, type, row, val) +
                               ", value not in enum values, use default");
                return default;
            }

            var method = typeof(Enum).GetMethods().FirstOrDefault(o => o.Name == "TryParse" && o.GetParameters().Length == 3);
            method = method.MakeGenericMethod(typeof(T));
            var args = new object[] { val, true, default(T) };
            method.Invoke(null, args);
            return (T)args[2];
        }

        public static T[] ConvertAssetReferArray<T>(ExcelWorksheet sheet, string columnName, ExcelRangeBase column, string separator="")
            where T : AssetRefer, new()
        {
            if (string.IsNullOrEmpty(separator)) separator = ExporterConsts.splitOutter;

            var arr = column.Value.ToString().Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length <= 0) return null;

            var result = new T[arr.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ConvertAssetRefer<T>(sheet, columnName, column, arr[i]);
            }
            
            return result;
        }
        
        public static T ConvertAssetRefer<T>(ExcelWorksheet sheet, string columnName, ExcelRangeBase column, string value=null)
            where T : AssetRefer, new()
        {
            var path = string.IsNullOrEmpty(value) ? column.Value.ToString() : value;
            var goPath = ExporterUtils.FixPath(path);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(goPath);
            if (!string.IsNullOrEmpty(goPath) && asset == null)
            {
                Debug.LogWarning(
                    $"[Excel] Can't Find Asset [{goPath}], at : {sheet.Name}@[{column.Address}], column name : {columnName}");
                return null;
            }

            var assetBundle = string.Empty;
            var assetName = string.Empty;
            var variant = string.Empty;
            var fullBundle = string.Empty;
            try
            {
                fullBundle = assetBundle = AssetDatabase.GetImplicitAssetBundleName(goPath);
                variant = AssetDatabase.GetImplicitAssetBundleVariantName(goPath);
                assetName = goPath;

                if (string.IsNullOrEmpty(assetBundle))
                {
                    // Debug.LogError(
                    //     $"[Excel] Can't Find Assetbundle of [{goPath}], at : {sheet.Name}@[{column.Address}], column name : {columnName}");
                    var name = GetVariantMainName(sheet.Name);
                    variant = GetVariantName(sheet.Name);
                    assetBundle = $"conf_{name.ToLower()}";
                    fullBundle = assetBundle;
                    var importer = AssetImporter.GetAtPath(goPath);
                    importer.SetAssetBundleNameAndVariant(assetBundle, variant);
                    importer.SaveAndReimport();
                }

                if (!string.IsNullOrEmpty(variant))
                {
                    fullBundle += $".{variant}";
                }
                
                return new T
                {
                    AssetBundle = fullBundle,
                    AssetName = assetName
                };
            }
            catch (Exception err)
            {
                Debug.LogError(
                    $"[Excel] Can't Find Assetbundle of [{goPath}], at : {sheet.Name}@[{column.Address}], column name : {columnName}\n\nERROR: {err}");
            }


            return null;
        }
        
        public static Color ConvertColor(string tableName, string name, string type, int row, string val)
        {
            val = val.Trim();
            if (!val.StartsWith("#") && IsHex(val))
            {
                val = "#" + val;
            }

            Color color;
            if (ColorUtility.TryParseHtmlString(val, out color))
            {
                return color;
            }

            throw new Exception(ErrFormat(tableName, name, type, row, val));
        }
        
        public static Color32 ConvertColor32(string tableName, string name, string type, int row, string val)
        {
            val = val.Trim();
            if (!val.StartsWith("#") && IsHex(val))
            {
                val = "#" + val;
            }

            Color color;
            if (ColorUtility.TryParseHtmlString(val, out color))
            {
                return color;
            }

            throw new Exception(ErrFormat(tableName, name, type, row, val));
        }

        private static bool IsHex(string val)
        {
            if (val.Length != 6 && val.Length != 8) return false;

            foreach (var ch in val)
            {
                var c = char.ToLower(ch);
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')) continue;

                return false;
            }

            return true;
        }

        public static string ErrFormat(string tableName, string name, string type, int row, string val)
        {
            return $"Wrong value at: {tableName}.{name}[{type}]@{row} => {val}";
        }

        public static bool IsValidTable(ExcelWorksheet table)
        {
            return table.Name.StartsWith(ExporterConsts.exportPrefix);
        }
        
        public static string EntityNameFromTable(ExcelWorksheet table, bool withVariant=false)
        {
            var tableName = GetVariantMainName(table.Name);
            var variantName = GetVariantName(table.Name);
            var entityName = ToPascalCase(tableName) + ExporterConsts.confClassSuffix;
            if (!string.IsNullOrEmpty(variantName) && withVariant) entityName += $"@{variantName}";
            
            return entityName;
        }

        public static string GetVariantMainName(string tableName)
        {
            if (tableName.StartsWith(ExporterConsts.exportPrefix))
                tableName = tableName.Substring(ExporterConsts.exportPrefix.Length);
            if (!tableName.Contains(ExporterConsts.exportVariantSplit)) return tableName;
            var arr = tableName.Split(ExporterConsts.exportVariantSplit, StringSplitOptions.RemoveEmptyEntries);
            return arr[0];
        }
        
        public static string GetVariantName(string tableName)
        {
            if (!tableName.Contains(ExporterConsts.exportVariantSplit)) return ExporterConsts.defaultLanguage;
            var arr = tableName.Split(ExporterConsts.exportVariantSplit, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 2) return ExporterConsts.defaultLanguage;
            
            return arr[1].ToLower();
        }
        
        public static void CreateFileFolderIfNotExists(string file)
        {
            var folder = Path.GetDirectoryName(file);
            CreateFolderIfNotExists(folder);
        }

        public static void CreateFolderIfNotExists(string folder)
        {
            if (Directory.Exists(folder)) return;

            var baseFolder = Path.GetDirectoryName(folder);
            CreateFolderIfNotExists(baseFolder);

            Directory.CreateDirectory(folder);
        }
        
        public static string GetProjectRelativePath(string path)
        {
            var assetIndex = path.IndexOf("Assets");
            var relaPath = assetIndex >= 0 ? path.Substring(assetIndex) : "Assets/" + path;
            return relaPath.Replace("\\", "/");
        }
        
        public static void WarningCantFindAsset(ExcelWorksheet sheet, string columnName, ExcelRangeBase value, string type, string path)
        {
            Debug.LogWarning($"[Excel] No Asset({type}) [{path}], at : {sheet.Name}@[{value.Address}], column name : {columnName}");
        }
    }
}