using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public abstract class Emitter
    {
        protected readonly TypeBuilder _typeBuilder;
        protected readonly StringBuilder _appendQueue;

        public Emitter(TypeBuilder typeBuilder, StringBuilder appendQueue)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
        }
    }
}