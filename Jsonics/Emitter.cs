using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public abstract class Emitter
    {
        protected readonly TypeBuilder _typeBuilder;
        protected readonly StringBuilder _appendQueue;
        protected readonly Emitters _emitters;
        protected readonly FieldBuilder _stringBuilderField;

        public Emitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters, FieldBuilder stringBuilderField)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
            _emitters = emitters;
            _stringBuilderField = stringBuilderField;
        }
    }
}