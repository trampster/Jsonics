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