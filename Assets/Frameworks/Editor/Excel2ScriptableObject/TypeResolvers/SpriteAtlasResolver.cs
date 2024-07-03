using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class SpriteAtlasResolver : TypeResolverBase<SpriteAtlasRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("SpriteAtlas", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("SpriteAtlasRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<SpriteAtlasRefer>(sheet, columnName, value);
        }
    }
}