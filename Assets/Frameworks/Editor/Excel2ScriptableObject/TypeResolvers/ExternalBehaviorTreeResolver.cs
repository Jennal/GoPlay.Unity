// using System;
// using OfficeOpenXml;
//
// namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
// {
//     public class ExternalBehaviorTreeResolver : TypeResolverBase<ExternalBehaviorTreeRefer>
//     {
//         public override bool RecognizeType(string typeName)
//         {
//             return string.Equals("ExternalBehaviorTreeRefer", typeName, StringComparison.OrdinalIgnoreCase) ||
//                    string.Equals("ExternalBehaviorTree", typeName, StringComparison.OrdinalIgnoreCase);
//         }
//         
//         public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
//         {
//             return ExporterUtils.ConvertAssetRefer<ExternalBehaviorTreeRefer>(sheet, columnName, value);
//         }
//     }
// }