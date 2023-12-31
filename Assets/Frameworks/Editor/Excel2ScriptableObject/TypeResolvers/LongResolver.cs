using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class LongResolver : TypeResolverBase<long>
    {
        public override string TypeName => "long";
        
        public override string GetScriptClone(string fieldName)
        {
            return fieldName;
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var val = ExporterUtils.ConvertInt64(sheet.Name, columnName, TypeName, value.End.Row, value.Value.ToString());
            return val;
        }
    }
}