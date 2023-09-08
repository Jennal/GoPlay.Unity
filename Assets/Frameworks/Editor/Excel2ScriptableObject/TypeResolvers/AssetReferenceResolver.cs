//using OfficeOpenXml;
//using UnityEditor;
//using UnityEditor.AddressableAssets;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//
//namespace Veewo.Editor.Excel2ScriptableObject.TypeResolvers
//{
//    public class AssetReferenceResolver : TypeResolverBase<AssetReference>
//    {
//        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
//        {
//            var path = value.Value.ToString();
//            var goPath = ExporterUtils.FixPath(path);
//            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(goPath);
//            if (!string.IsNullOrEmpty(goPath) && asset == null) {
//                Debug.LogWarning($"[Excel] Can't Find Asset [{goPath}], sheet name : {sheet.Name}, column name : {columnName}");
//            }
//
//            var e = AddressableHelper.GetOrCreateEntry(goPath);
//            e.address = goPath;
//
//            return AddressableAssetSettingsDefaultObject.Settings.CreateAssetReference(e.guid);
//        }
//    }
//}