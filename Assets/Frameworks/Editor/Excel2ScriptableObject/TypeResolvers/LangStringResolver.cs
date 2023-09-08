using System.Text;
using GoPlay.Translate;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class LangStringResolver : TypeResolverBase<LangString>
    {
        public override string TypeName => "LangString";

        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var val = value.Value.ToString();
            var bytes = Encoding.Default.GetBytes(val);
            return new LangString
            {
                Text = Encoding.UTF8.GetString(bytes)
            };
        }
    }
}