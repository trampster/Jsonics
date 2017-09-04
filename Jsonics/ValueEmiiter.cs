using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ValueEmitter : Emitter
    {
        public ValueEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters, FieldBuilder stringBuilderField)
            : base(typeBuilder, appendQueue, emitters, stringBuilderField)
        {
        }
        
        public void CreateString(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            generator.EmitAppendEscaped();
        }

        public void CreateInt(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            generator.AppendInt();
        }

        public void CreateNullableInt(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            var hasValueLabel = generator.DefineLabel();
            var endLabel = generator.DefineLabel();

            generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_HasValue", new Type[0]));
            generator.BrIfTrue(hasValueLabel);

            generator.Append("null");
            generator.Branch(endLabel);

            generator.Mark(hasValueLabel);
            getValueOnStack(generator);
            generator.Call(typeof(int?).GetTypeInfo().GetMethod("get_Value", new Type[0]));
            generator.AppendInt();
            generator.Mark(endLabel);
        }

        public void CreateNumber(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack, Type numberType)
        {
            getValueOnStack(generator);
            generator.EmitAppend(numberType);
        }

        public void CreateBool(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
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

        public void CreateArrayValue(Type type, JsonILGenerator generator, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = _emitters.GetMethod(type, generator.AppendQueue, (gen, getElementOnStack) => _emitters.TypeEmitter.EmitType(type.GetElementType(), gen, getElementOnStack));
            generator.Pop(); //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0);  //load this
            generator.LoadStaticField(_stringBuilderField);
            getTypeOnStack(generator);
            generator.Call(methodInfo);
        }

        public void CreateListValue(Type type, JsonILGenerator generator, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = _emitters.GetMethod(type, generator.AppendQueue, (gen, getElementOnStack) => _emitters.TypeEmitter.EmitType(type.GenericTypeArguments[0], gen, getElementOnStack));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0);
            generator.LoadStaticField(_stringBuilderField);
            getTypeOnStack(generator);
            generator.Call(methodInfo);
        }

        public void CreateDictionaryValue(Type type, JsonILGenerator generator, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = _emitters.GetMethod(type, generator.AppendQueue, (gen, getElementOnStack) => _emitters.TypeEmitter.EmitType(type.GenericTypeArguments[0], gen, getElementOnStack));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(typeof(object), 0);
            generator.LoadStaticField(_stringBuilderField);
            getTypeOnStack(generator);
            generator.Call(methodInfo);
        }

        public void CreateDateTime(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            getValueOnStack(generator);
            generator.AppendDate();
        }

        public void CreateGuid(JsonILGenerator generator, Action<JsonILGenerator> getValueOnStack)
        {
            var propertyValueLocal = generator.DeclareLocal<Guid>();
            
            generator.Append($"\"");

            getValueOnStack(generator);

            generator.StoreLocal(propertyValueLocal);
            generator.LoadLocalAddress(propertyValueLocal);

            generator.Constrain<Guid>();
            generator.CallToString();
            generator.EmitAppend(typeof(string));

            generator.Append($"\"");
            generator.EmitQueuedAppends();
        }
    }
}