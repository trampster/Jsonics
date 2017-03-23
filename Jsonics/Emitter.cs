using System.Reflection.Emit;
using System.Text;

namespace Jsonics
{
    public abstract class Emitter
    {
        protected readonly TypeBuilder _typeBuilder;
        protected readonly StringBuilder _appendQueue;

        protected readonly Emitters _emitters;

        public Emitter(TypeBuilder typeBuilder, StringBuilder appendQueue, Emitters emitters)
        {
            _typeBuilder = typeBuilder;
            _appendQueue = appendQueue;
            _emitters = emitters;
        }
    }
}