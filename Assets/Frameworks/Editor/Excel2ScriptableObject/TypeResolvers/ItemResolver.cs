using GoPlay.Common.Data;
using OfficeOpenXml;
using UnityEngine;
using GoPlay.Data;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class ItemResolver : TypeResolverBase<Item>
    {
        public override string GetScriptClone(string fieldName)
        {
            return fieldName;
        }

        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var val = ExporterUtils.ConvertVector2Int(sheet.Name, columnName, TypeName, value.End.Row, value.Value.ToString());
            return new Item
            {
                Id = val.x,
                Count = val.y,
            };
        }
    }
}