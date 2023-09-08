using System;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace GoPlay.Editor.Excel2ScriptableObject.YamlTypeConverters
{
    public class ColorTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Color);
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            throw new NotImplementedException();
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var color = (Color)value!;
            var text = $"{{r: {color.r}, g: {color.g}, b: {color.b}, a: {color.a}}}";
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, text, ScalarStyle.Plain, true, false, true));
        }
    }
}