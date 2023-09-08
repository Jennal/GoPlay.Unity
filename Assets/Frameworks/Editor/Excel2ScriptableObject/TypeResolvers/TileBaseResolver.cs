using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class TileBaseResolver : TypeResolverBase<TileBaseRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("Tile", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("TileBase", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("TileBaseRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<TileBaseRefer>(sheet, columnName, value);
        }
    }
}