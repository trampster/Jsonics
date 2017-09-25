using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class BoolEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            getValueOnStack(generator);
            property.EmitGetValue(generator);
            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString($"\"{property.Name}\":false");
            generator.Branch(callAppend);

            //true case
            generator.Mark(trueLabel);
            generator.LoadString($"\"{property.Name}\":true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator, bool> getValueOnStack, JsonILGenerator generator)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            getValueOnStack(generator, false);
            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString("false");
            generator.Branch(callAppend);

            //true calse
            generator.Mark(trueLabel);
            generator.LoadString($"true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(bool);
        }
    }
}