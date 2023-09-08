using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace GoPlay.Editor.Excel2ScriptableObject.YamlTypeConverters
{
    public class BoolTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(bool);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var val = (bool)value!;
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, val ? "1" : "0", ScalarStyle.Any, true, false, true));
        }
    }
}