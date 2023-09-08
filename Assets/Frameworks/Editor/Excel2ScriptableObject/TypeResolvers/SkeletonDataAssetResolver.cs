// using System;
// using OfficeOpenXml;
//
// namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
// {
//     public class SkeletonDataAssetResolver : TypeResolverBase<SkeletonDataAssetRefer>
//     {
//         public override bool RecognizeType(string typeName)
//         {
//             return string.Equals("SkeletonDataAsset", typeName, StringComparison.OrdinalIgnoreCase) ||
//                    string.Equals("SkeletonDataAssetRefer", typeName, StringComparison.OrdinalIgnoreCase);
//         }
//         
//         public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
//         {
//             return ExporterUtils.ConvertAssetRefer<SkeletonDataAssetRefer>(sheet, columnName, value);
//         }
//     }
// }