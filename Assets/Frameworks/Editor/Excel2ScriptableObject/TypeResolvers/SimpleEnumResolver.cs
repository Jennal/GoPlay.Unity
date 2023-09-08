using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoPlay.Editor.Utils;
using OfficeOpenXml;

namespace GoPlay.Editor.Excel2ScriptableObject.TypeResolvers {
    public class SimpleEnumTypeResolver : TypeResolverBase
    {
        public override object Default
        {
            get
            {
                if (_isArray) return null;
                return 0;
            }
        }
        public override string TypeName => _enumType?.ToString();
        public override string Namespace => _enumType.Namespace;
        public override Type Type => _enumType;

        private bool _isArray;
        private Type _enumType;
        private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        private static Dictionary<string, Type> _fullNametypeCache = new Dictionary<string, Type>();

        static SimpleEnumTypeResolver()
        {
//            var start = Time.realtimeSinceStartup;
            var list = ReflectionHelper.GetTypesInAllLoadedAssemblies(o => o.IsEnum && ExporterConsts.enumNamespaces.Any(n => o.Namespace?.StartsWith(n) ?? false));
            foreach (var type in list)
            {
                var name = type.Name;
                var fullName = type.Name;
                if (!string.IsNullOrEmpty(type.Namespace)) fullName = $"{type.Namespace}.{name}";

                _typeCache[name] = type;
                _fullNametypeCache[fullName] = type;
            }
//            Debug.Log($"===========> Simple Enum time: {Time.realtimeSinceStartup - start}");
        }
        
        public override string GetScriptClone(string fieldName)
        {
            return fieldName;
        }
        
        public override bool RecognizeType(string typeName)
        {
            _isArray = false;
            
            if (typeName.EndsWith("[]"))
            {
                _isArray = true;
                typeName = typeName.Substring(0, typeName.Length - 2);
            }
            
            _enumType = null;
            if (_typeCache.ContainsKey(typeName))
            {
                _enumType = _typeCache[typeName];
            }
            
            if (_fullNametypeCache.ContainsKey(typeName))
            {
                _enumType = _fullNametypeCache[typeName];
            }
            
            return _enumType != null;
        }

        public override object GetValue(ExcelWorksheet sheet, string columnName, ExcelRangeBase value)
        {
            var method = typeof(ExporterUtils).GetMethod("ConvertEnum", BindingFlags.Public | BindingFlags.Static);
            method = method.MakeGenericMethod(_enumType);

            if (_isArray)
            {
                var arr = value.Value.ToString().Split(ExporterConsts.splitOutter.ToCharArray());
                // var result = Array.CreateInstance(_enumType, arr.Length);
                var result = new int[arr.Length];
                for (var i = 0; i < arr.Length; i++)
                {
                    var v = method.Invoke(null,
                        new object[] {sheet.Name, columnName, TypeName, value.End.Row, arr[i]});
                    result.SetValue((int)v, i);
                }

                return result;
            }
            else
            {
                var v = method.Invoke(null,
                    new object[] {sheet.Name, columnName, TypeName, value.End.Row, value.Value.ToString()});
                return (int)v;
            }
        }
    }
}
