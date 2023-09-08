using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class IntArrayResolver : TypeResolverBase<int[]>
    {
        public override string TypeName => "int[]";
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var content = value.Value.ToString();
            content = content.Replace("\r", "");
            content = content.Replace("\n", "");
            var val = ExporterUtils.ConvertInt32Array(sheet.Name, columnName, TypeName, value.End.Row, content);
            return val;
        }
    }
}