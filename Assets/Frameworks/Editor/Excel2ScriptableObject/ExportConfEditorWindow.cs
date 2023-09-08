using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoPlay.Editor.Excel2ScriptableObject
{
    public class ExportConfEditorWindow : EditorWindow
    {
        private ExportConf _data;
        
        [MenuItem("Excel/Setup", false)]
        public static void Open()
        {
            var win = GetWindow<ExportConfEditorWindow>();
            win.InitData();
            win.ShowModal();
        }

        private void InitData()
        {
            if (File.Exists(ExporterConsts.confFile))
            {
                _data = AssetDatabase.LoadAssetAtPath<ExportConf>(ExporterConsts.confFile);
            }
            else
            {
                _data = ScriptableObject.CreateInstance<ExportConf>();
                _data.xlsFolder = ExporterConsts.xlsFolder;
                AssetDatabase.CreateAsset(_data, ExporterConsts.confFile);
                AssetDatabase.SaveAssets();
            }
        }

        private void OnDestroy()
        {
            SaveChanges();
        }

        private void OnDisable()
        {
            SaveChanges();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    _data.xlsFolder = EditorGUILayout.TextField("Excel Folder", _data.xlsFolder);
                    if (GUILayout.Button("Open..."))
                    {
                        _data.xlsFolder = EditorUtility.OpenFolderPanel("Open Excels Folder", "", "");
                        _data.xlsFolder = FixPath(_data.xlsFolder);
                        // Debug.Log(_data.xlsFolder);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    _data.defaultLanguage = EditorGUILayout.TextField("Default Language", _data.defaultLanguage);
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    _data.languages = EditorGUILayout.TextField("Languages", _data.languages);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            if (EditorGUI.EndChangeCheck())
            {
                SaveChanges();
            }
        }

        private string FixPath(string xlsFolder)
        {
            if (string.IsNullOrEmpty(xlsFolder)) return xlsFolder;

            xlsFolder = xlsFolder.Replace("\\", "/");
            var basePath = Application.dataPath;
            var count = 0;
            while (!string.IsNullOrEmpty(basePath) && !xlsFolder.StartsWith(basePath))
            {
                count++;
                basePath = Path.GetDirectoryName(basePath);
                if (string.IsNullOrEmpty(basePath)) break;
                
                basePath = basePath.Replace("\\", "/");
            }

            if (string.IsNullOrEmpty(basePath)) return xlsFolder;

            xlsFolder = xlsFolder.Replace(basePath, "");
            for (var i = 0; i < count; i++)
            {
                if (xlsFolder.StartsWith("/")) xlsFolder = xlsFolder.Substring(1);
                xlsFolder = "../" + xlsFolder;
            }

            return xlsFolder;
        }

        public override void SaveChanges()
        {
            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssetIfDirty(_data);
            base.SaveChanges();
        }
    }
}