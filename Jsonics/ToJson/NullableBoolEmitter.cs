using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.ToJson
{
    internal class NullableBoolEmitter : ToJsonEmitter
    {
        internal override void EmitProperty(IJsonPropertyInfo property, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            var propertyValueLocal = generator.DeclareLocal(property.Type);
            var endLabel = generator.DefineLabel();
            var nonNullLabel = generator.DefineLabel();

            getValueOnStack(generator);
            property.EmitGetValue(generator);
            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocalAddress(propertyValueLocal);

            //check for null
            generator.Call(typeof(bool?).GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(nonNullLabel);
            
            //property is null
            generator.Append($"\"{property.Name}\":null");
            generator.Branch(endLabel);

            //property is not null
            generator.Mark(nonNullLabel);
            generator.LoadLocalAddress(propertyValueLocal);
            generator.Call(typeof(bool?).GetTypeInfo().GetMethod("get_Value", new Type[0]));
            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString($"\"{property.Name}\":false");
            generator.Branch(callAppend);

            //true case
            generator.Mark(trueLabel);
            generator.LoadString($"\"{property.Name}\":true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));

            generator.Mark(endLabel);
        }

        internal override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack, JsonILGenerator generator)
        {
            getValueOnStack(generator);
            var hasValueLabel = generator.DefineLabel();
            var endLabel = generator.DefineLabel();

            generator.Call(typeof(bool?).GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(hasValueLabel);

            generator.Append("null");
            generator.Branch(endLabel);

            generator.Mark(hasValueLabel);
            getValueOnStack(generator);
            generator.Call(typeof(bool?).GetTypeInfo().GetMethod("get_Value", new Type[0]));

            Label trueLabel = generator.DefineLabel();
            Label callAppend = generator.DefineLabel();

            generator.BrIfTrue(trueLabel);

            //false case
            generator.LoadString("false");
            generator.Branch(callAppend);

            //true calse
            generator.Mark(trueLabel);
            generator.LoadString($"true");

            generator.Mark(callAppend);
            generator.EmitAppend(typeof(string));

            generator.Mark(endLabel);
            
        }

        internal override bool TypeSupported(Type type)
        {
            return type == typeof(bool?);
        }
    }
}