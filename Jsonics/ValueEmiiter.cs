using System;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ValueEmitter : Emitter
    {
        public ValueEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters)
            : base(typeBuilder, appendQueue, emitters)
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

        public void CreateArrayValue(Type type, TypeBuilder typeBuilder, JsonILGenerator generator, FieldBuilder stringBuilderField, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = _emitters.GetMethod(type, typeBuilder, generator.AppendQueue, (gen, getElementOnStack) => _emitters.TypeEmitter.EmitType(type.GetElementType(), gen, typeBuilder, stringBuilderField, getElementOnStack));
            generator.Pop(); //remove StringBuilder from the stack
            generator.LoadArg(0);  //load this
            generator.LoadStaticField(stringBuilderField);
            getTypeOnStack(generator);
            generator.Call(methodInfo);
        }

        public void CreateListValue(Type type, TypeBuilder typeBuilder, JsonILGenerator generator, FieldBuilder stringBuilderField, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = _emitters.GetMethod(type, typeBuilder, generator.AppendQueue, (gen, getElementOnStack) => _emitters.TypeEmitter.EmitType(type.GenericTypeArguments[0], gen, typeBuilder, stringBuilderField, getElementOnStack));
            generator.Pop();     //remove StringBuilder from the stack
            generator.LoadArg(0);
            generator.LoadStaticField(stringBuilderField);
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