using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class FontAssetResolver : TypeResolverBase<FontRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("Font", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("FontRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<FontRefer>(sheet, columnName, value);
        }
    }
}