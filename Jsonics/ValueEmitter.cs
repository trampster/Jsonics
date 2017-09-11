using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public class ValueEmitter : Emitter
    {
        public ValueEmitter(TypeBuilder typeBuilder, StringBuilder appendQueue, ListMethods listMethods, FieldBuilder stringBuilderField)
            : base(typeBuilder, appendQueue, listMethods, stringBuilderField)
        {
        }

        public void CreateDictionaryValue(Type type, JsonILGenerator generator, Action<JsonILGenerator> getTypeOnStack)
        {
            var methodInfo = _listMethods.GetMethod(type, (gen, getElementOnStack) => _listMethods.TypeEmitter.EmitType(type.GenericTypeArguments[0], gen, getElementOnStack), null);
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