using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Unity.Plastic.Newtonsoft.Json;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class ExportCacheData
    {
        public DateTime ExportCSharpTime;
        public DateTime ExportDataTime;

        public DateTime ModifiedTime;
        public List<string> SheetEntities = new List<string>();
    }
    
    public class ExportCache
    {
        public Dictionary<string, ExportCacheData> Dict;
        
        public static ExportCache Load()
        {
            var result = new ExportCache();
            var file = ExporterConsts.cacheFile;
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<Dictionary<string, ExportCacheData>>(json);
                result.Dict = data;
            }

            return result;
        }

        public List<string> GetSheetEntities(string file)
        {
            if (!Dict.ContainsKey(file)) return null;

            return Dict[file].SheetEntities;
        }
        
        public bool FilterExportCSharp(string file)
        {
            if (Dict == null) return true;
            if (!Dict.ContainsKey(file)) return true;

            var item = Dict[file];
            if (GetModifyTime(file) >= item.ExportCSharpTime) return true;
            
            foreach (var entity in item.SheetEntities)
            {
                var mainName = ExporterUtils.GetVariantMainName(entity);
                var filePath = Path.Combine(ExporterConsts.csFolder, mainName + "s.cs");
                if (!File.Exists(filePath)) return true;
            }

            return false;
        }
        
        public bool FilterExportScriptableObject(string file)
        {
            if (Dict == null) return true;
            if (!Dict.ContainsKey(file)) return true;

            var item = Dict[file];
            if (GetModifyTime(file) >= item.ExportDataTime) return true;

            foreach (var entity in item.SheetEntities)
            {
                var mainName = ExporterUtils.GetVariantMainName(entity);
                var variantName = ExporterUtils.GetVariantName(entity);
                var filePath = Path.Combine(ExporterConsts.exportFolder, variantName, mainName + "s.asset");
                if (!File.Exists(filePath)) return true;
            }

            return false;
        }

        public void RefreshExportCSharp(List<string> files)
        {
            RefreshAndSet(files, item =>
            {
                item.ExportCSharpTime = DateTime.Now;
            });
        }

        public void RefreshExportScriptableObject(List<string> files)
        {
            RefreshAndSet(files, item =>
            {
                item.ExportDataTime = DateTime.Now;
            });
        }

        private void RefreshAndSet(List<string> files, Action<ExportCacheData> refresh)
        {
            if (Dict == null) Dict = new Dictionary<string, ExportCacheData>();

            foreach (var file in files)
            {
                if (!Dict.ContainsKey(file))
                {
                    Dict[file] = new ExportCacheData();
                }

                refresh(Dict[file]);
            }

            var keys = Dict.Keys.Where(o => !files.Any(f => f == o)).ToList();
            foreach (var key in keys)
            {
                Dict.Remove(key);
            }

            foreach (var item in Dict)
            {
                RefreshEntities(item.Key, item.Value);
            }
            
            var json = JsonConvert.SerializeObject(Dict, Formatting.Indented);
            File.WriteAllText(ExporterConsts.cacheFile, json);
        }

        private void RefreshEntities(string file, ExportCacheData data)
        {
            var modifyTime = GetModifyTime(file);
            if (modifyTime <= data.ModifiedTime) return;

            data.ModifiedTime = modifyTime;
            data.SheetEntities.Clear();
            
            var tmpFileName = file + ".converting";
            if (File.Exists(tmpFileName))
            {
                File.Delete(tmpFileName);
            }

            File.Copy(file, tmpFileName);

            try
            {
                using (var stream = File.Open(tmpFileName, FileMode.Open, FileAccess.Read))
                {
                    var excelReader = new ExcelPackage(stream);
                    foreach (var sheet in excelReader.Workbook.Worksheets)
                    {
                        var name = sheet.Name;
                        if (name == null || !name.StartsWith(ExporterConsts.exportPrefix)) continue;

                        var entityName = ExporterUtils.EntityNameFromTable(sheet, true);
                        data.SheetEntities.Add(entityName);
                    }
                }
            }
            finally
            {
                File.Delete(tmpFileName);
            }
        }

        private DateTime GetModifyTime(string file)
        {
            var fileInfo = new FileInfo(file);
            return fileInfo.LastWriteTime;
        }
    }
}