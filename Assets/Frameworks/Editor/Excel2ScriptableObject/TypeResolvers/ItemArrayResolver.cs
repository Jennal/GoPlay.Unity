using System.Collections.Generic;
using GoPlay.Common.Data;
using OfficeOpenXml;
using GoPlay.Data;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class ItemArrayResolver : TypeResolverBase<Item[]>
    {
        public override string TypeName => "Item[]";
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var content = value.Value.ToString();
            content = content.Replace("\r", "");
            content = content.Replace("\n", "");
            var arr = ExporterUtils.ConvertStringArray(sheet.Name, columnName, TypeName, value.End.Row, content, ExporterConsts.splitOutter);
            var list = new List<Item>();

            foreach (var item in arr)
            {
                var val = ExporterUtils.ConvertVector2Int(sheet.Name, columnName, TypeName, value.End.Row, item,
                    ExporterConsts.splitInner);
                list.Add(new Item
                {
                    Id    = val.x,
                    Count = val.y,
                });
            }
            
            return list.ToArray();
        }
    }
}