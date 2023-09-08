using System.Linq;
using GoPlay.Translate;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class LangStringArrayResolver : TypeResolverBase<LangString[]>
    {
        public override string TypeName => "LangString[]";
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var val = ExporterUtils.ConvertStringArray(sheet.Name, columnName, TypeName, value.End.Row, value.Value.ToString());
            return val.Select(o => new LangString
            {
                Text = o
            }).ToArray();
        }
    }
}