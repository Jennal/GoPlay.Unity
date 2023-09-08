// using OfficeOpenXml;
// using UnityEditor;
// using UnityEngine;
//
// namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
// {
//     public class ObjectResolver : TypeResolverBase<Object>
//     {
//         public override bool RecognizeType(string typeName)
//         {
//             return base.RecognizeType(typeName) || typeName == "UnityEngine.Object";
//         }
//
//         public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
//         {
//             var path = value.Value.ToString();
//             var goPath = ExporterUtils.FixPath(path);
//             var asset = AssetDatabase.LoadAssetAtPath<Object>(goPath);
//             if (!string.IsNullOrEmpty(goPath) && asset == null) {
//                 ExporterUtils.WarningCantFindAsset(sheet, columnName, value, TypeName, goPath);
//             }
//             return asset;
//         }
//     }
// }