using System;
using OfficeOpenXml;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers
{
    public class AudioClipResolver : TypeResolverBase<AudioClipRefer>
    {
        public override bool RecognizeType(string typeName)
        {
            return string.Equals("AudioClip", typeName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals("AudioClipRefer", typeName, StringComparison.OrdinalIgnoreCase);
        }
        
        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            return ExporterUtils.ConvertAssetRefer<AudioClipRefer>(sheet, columnName, value);
        }
    }
}