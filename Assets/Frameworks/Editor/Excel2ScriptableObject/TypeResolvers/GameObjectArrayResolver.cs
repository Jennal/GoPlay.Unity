using System;
using OfficeOpenXml;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class GameObjectArrayResolver : TypeResolverBase<GameObject[]>
    {
        public override string TypeName => "GameObjectRefer[]";

        public override bool RecognizeType(string typeName)
        {
            return string.Equals("GameObject[]", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("GameObjectRefer[]", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetReferArray<GameObjectRefer>(sheet, columnName, value);
        }
    }
}