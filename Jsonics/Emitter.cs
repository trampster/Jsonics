using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public abstract class Emitter
    {
        protected readonly TypeBuilder _typeBuilder;
        protected readonly StringBuilder _appendQueue;
        protected readonly ListMethods _listMethods;
        protected readonly FieldBuilder _stringBuilderField;

        public Emitter(TypeBuilder typeBuilder, StringBuilder appendQueue, ListMethods listMethods, FieldBuilder stringBuilderField)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
            _listMethods = listMethods;
            _stringBuilderField = stringBuilderField;
        }
    }
}