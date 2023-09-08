using OfficeOpenXml;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject.Hooks
{
    public abstract class DataExportHookBase
    {
        public abstract bool Recognize(string xls, ExcelWorksheet table);
        public abstract void OnExportBegin(string xls, ExcelWorksheet table);
        public abstract void OnExportFinish(string xls, ExcelWorksheet table, Object asset);

        public virtual void OnAllExportBegin(string xls, ExcelPackage excel)
        {
        }

        public virtual void OnAllExportFinish(string xls, ExcelPackage excel)
        {
        }
    }
}