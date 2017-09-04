using System;
using System.Reflection;

namespace Jsonics.ToJson
{
    public class NullableIntEmitter : ToJsonEmitter
    {
        public NullableIntEmitter(JsonILGenerator generator) : base(generator)
        {
        }

        public override void EmitProperty(PropertyInfo property, Action<JsonILGenerator> getValueOnStack)
        {
            var propertyValueLocal = _generator.DeclareLocal(property.PropertyType);
            var endLabel = _generator.DefineLabel();
            var nonNullLabel = _generator.DefineLabel();

            getValueOnStack(_generator);
            _generator.GetProperty(property);
            _generator.StoreLocal(propertyValueLocal);
            _generator.LoadLocalAddress(propertyValueLocal);

            //check for null
            _generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            _generator.BrIfTrue(nonNullLabel);
            
            //property is null
            _generator.Append($"\"{property.Name}\":null");
            _generator.Branch(endLabel);

            //property is not null
            _generator.Mark(nonNullLabel);
            _generator.Append($"\"{property.Name}\":");
            _generator.LoadLocalAddress(propertyValueLocal);
            _generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_Value", new Type[0]));
            _generator.AppendInt();
            _generator.Mark(endLabel);
        }

        public override void EmitValue(Type type, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(_generator);
            var hasValueLabel = _generator.DefineLabel();
            var endLabel = _generator.DefineLabel();

            _generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            _generator.BrIfTrue(hasValueLabel);

            _generator.Append("null");
            _generator.Branch(endLabel);

            _generator.Mark(hasValueLabel);
            getValueOnStack(_generator);
            _generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_Value", new Type[0]));
            _generator.AppendInt();
            _generator.Mark(endLabel);
        }

        public override bool TypeSupported(Type type)
        {
            return type == typeof(int?);
        }
    }
}