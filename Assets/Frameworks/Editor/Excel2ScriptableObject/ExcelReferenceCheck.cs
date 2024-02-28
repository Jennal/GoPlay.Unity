using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OfficeOpenXml;
using UnityEditor;
using Ionic.Zip;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class ExcelReferenceCheck
    {
        [MenuItem("GoPlay/Excel/Check Update Reference", false, 100)]
        public static void CheckUpdateReference()
        {
            var lines = CheckReference(false);
            var content = string.Join("\n", lines);
            Debug.Log(content);
            EditorUtility.DisplayDialog("Update Reference", content, "OK");
        }

        [MenuItem("GoPlay/Excel/Check All Reference", false, 101)]
        public static void CheckAllReference()
        {
            var lines = CheckReference(true);
            var content = string.Join("\n", lines);
            Debug.Log(content);
            EditorUtility.DisplayDialog("All Reference", content, "OK");
        }

        static IEnumerable<string> CheckReference(bool showAll)
        {
            var folder = ExporterConsts.xlsFolder;
            var files = Directory.EnumerateFiles(folder, "*.*")
//                                 .Where(p => new string[]{"战斗等级表.xlsx"}.Any(p.EndsWith))
                .Where(p => ExporterConsts.extensionPattern.Any(p.EndsWith))
                .Where(xls => !xls.EndsWith(".converting") &&
                              !ExporterConsts.ignorePattern.Any(o => Path.GetFileName(xls).StartsWith(o)))
                .ToList();

            var i = 0f;
            var count = files.Count;
            foreach (var xls in files)
            {
                var tmpFileName = xls + ".checking";
                if (File.Exists(tmpFileName))
                {
                    File.Delete(tmpFileName);
                }

                File.Copy(xls, tmpFileName);

                var refs = GetReferences(tmpFileName);

                try
                {
                    i += 1f;
                    EditorUtility.DisplayCancelableProgressBar("", $"正在检查{Path.GetFileName(xls)} ...", i / count);

                    using (var stream = File.Open(tmpFileName, FileMode.Open, FileAccess.Read))
                    {
                        var excelReader = new ExcelPackage(stream);
                        var refKeys = CheckPackage(excelReader);

                        if (refKeys.Count <= 0) continue;

                        var fi = new FileInfo(xls);
                        var xlsWriteTime = fi.LastWriteTimeUtc;
                        var anyNewer = refKeys.Any(o =>
                        {
                            try
                            {
                                var refFile = refs[o.Key];
                                fi = new FileInfo(refFile);
                                var refLastWrite = fi.LastWriteTimeUtc;

                                var val = refLastWrite.Subtract(xlsWriteTime);
                                return val.TotalSeconds > 1;
                            }
                            catch
                            {
                                Debug.LogError($"引用错误：{xls}\n\t=> {refs[o.Key]}");
                                return false;
                            }
                        });
                        if (!anyNewer && !showAll) continue;

                        yield return $"{ShortPath(xls)}[{xlsWriteTime:yyyy-MM-dd HH:mm:ss}]";
                        foreach (var item in refKeys)
                        {
                            var file = refs[item.Key];
                            fi = new FileInfo(file);
                            var refWriteTime = fi.LastWriteTimeUtc;
                            if (refWriteTime <= xlsWriteTime && !showAll) continue;

                            var sheets = string.Join(", ", item.Value);
                            yield return $"    [{sheets}] => {ShortPath(file)}[{refWriteTime:yyyy-MM-dd HH:mm:ss}]";
                        }

                        yield return "";
                    }
                }
                finally
                {
                    File.Delete(tmpFileName);
                }
            }
            
            EditorUtility.ClearProgressBar();
        }

        static Dictionary<string, string> GetReferences(string fileName)
        {
            var result = new Dictionary<string, string>();
            try
            {
                using (var archive = ZipFile.Read(fileName))
                {
                    var entities = archive.Entries.Where(o =>
                    {
                        var name = Path.GetFileName(o.FileName);
                        return name.StartsWith("externalLink") && name.EndsWith(".xml.rels");
                    });
                    foreach (var entity in entities)
                    {
                        using (var stream = entity.OpenReader())
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                var xml = sr.ReadToEnd();
                                var doc = XDocument.Parse(xml);
                                var node = doc.Root.Descendants().FirstOrDefault();
                                var refPath = node.Attribute("Target").Value;

                                var folder = Path.GetDirectoryName(fileName);
                                var absPath = Path.Combine(folder, refPath);

                                var regex = new Regex(@"externalLink(\d+)\.xml\.rels");
                                var name = Path.GetFileName(entity.FileName);
                                var m = regex.Match(name);

                                result.Add(m.Groups[1].Value, absPath);
                            }
                        }
                    }
                }
            }
            finally
            {
            }

            return result;
        }

        static Dictionary<string, HashSet<string>> CheckPackage(ExcelPackage excelReader)
        {
            var result = new Dictionary<string, HashSet<string>>();
            foreach (var sheet in excelReader.Workbook.Worksheets)
            {
                foreach (var item in CheckSheet(sheet))
                {
                    if (!result.ContainsKey(item)) result[item] = new HashSet<string>();

                    result[item].Add(sheet.Name);
                }
            }

            return result;
        }

        static HashSet<string> CheckSheet(ExcelWorksheet sheet)
        {
            var set = new HashSet<string>();
            foreach (var cell in sheet.Cells)
            {
                if (string.IsNullOrEmpty(cell.Formula)) continue;

                var regex = new Regex(@"\[(.+?)\].+?!");
                var m = regex.Match(cell.Formula);
                if (m.Groups.Count <= 1) continue;

                set.Add(m.Groups[1].Value);
            }

            return set;
        }

        static string ShortPath(string path)
        {
            var xlsFolder = ClearPath(ExporterConsts.xlsFolder);
            path = ClearPath(path);

            path = path.Replace(xlsFolder, "");
            if (path.StartsWith("/")) path = path.Substring(1);

            return path;
        }

        static string ClearPath(string path)
        {
            path = path.Replace("\\", "/");
            var arr = new List<string>(path.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            for (int i = arr.Count - 1; i >= 0; i--)
            {
                if (arr[i] == ".")
                {
                    arr.RemoveAt(i);
                    continue;
                }

                if (arr[i] == "..")
                {
                    arr.RemoveAt(i);
                    arr.RemoveAt(i-1);
                    i--;
                    continue;
                }
            }

            return string.Join("/", arr);
        }
    }
}