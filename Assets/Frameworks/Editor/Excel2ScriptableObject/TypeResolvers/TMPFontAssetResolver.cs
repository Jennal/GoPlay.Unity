using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class TMPFontAssetResolver : TypeResolverBase<TMP_FontAssetRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("TMP_FontAssetRefer", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("TMP_FontAsset", typeName, StringComparison.OrdinalIgnoreCase);
        }

        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<TMP_FontAssetRefer>(sheet, columnName, value);
        }
    }
}