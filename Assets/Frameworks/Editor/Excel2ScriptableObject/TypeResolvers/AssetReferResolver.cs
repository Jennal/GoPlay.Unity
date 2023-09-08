using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class AssetReferResolver : TypeResolverBase<AssetRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("Asset", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("AssetRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }

        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<AssetRefer>(sheet, columnName, value);
        }
    }
}