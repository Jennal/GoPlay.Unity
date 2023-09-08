using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class SpriteResolver : TypeResolverBase<SpriteRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("Sprite", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("SpriteRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<SpriteRefer>(sheet, columnName, value);
        }
    }
}