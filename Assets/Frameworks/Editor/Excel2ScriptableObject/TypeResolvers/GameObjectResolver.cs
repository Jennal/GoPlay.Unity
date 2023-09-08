using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class GameObjectResolver : TypeResolverBase<GameObjectRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("GameObject", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("GameObjectRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<GameObjectRefer>(sheet, columnName, value);
        }
    }
}