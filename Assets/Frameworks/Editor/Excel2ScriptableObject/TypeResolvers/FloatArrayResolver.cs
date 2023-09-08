using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class FloatArrayResolver : TypeResolverBase<float[]>
    {
        public override string TypeName => "float[]";
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var val = ExporterUtils.ConvertFloatArray(sheet.Name, columnName, TypeName, value.End.Row, value.Value.ToString());
            return val;
        }
    }
}