using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    public class BoolEmitter : ToJsonEmitter
    {
        public override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            getValueOnStack(generator);
            generator.GetProperty(property);
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

        public override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            getValueOnStack(generator);
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

        public override bool TypeSupported(Type type)
        {
            return type == typeof(bool);
        }
    }
}