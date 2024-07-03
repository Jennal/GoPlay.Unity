using System;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class MaterialResolver : TypeResolverBase<MaterialRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("Material", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("MaterialRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<MaterialRefer>(sheet, columnName, value);
        }
    }
}