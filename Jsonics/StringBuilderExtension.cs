using System.Text;

namespace Jsonics
{
    public static class StringBuilderExtension
    {
        static StringBuilderExtension()
        {
            InitializeEscapingLookups();
        }

        static bool[] _needsEscaping = new bool[128];
        static string[] _escapeLookup = new string[128];

        static void InitializeEscapingLookups()
        {
            for(int index = 0; index < 32; index++)
            {
                _needsEscaping[index] = true;
            }
            _needsEscaping['\"'] = true;

            _needsEscaping['\\'] = true;
            _needsEscaping['/'] = true;
            _needsEscaping['\b'] = true;
            _needsEscaping['\f'] = true;
            _needsEscaping['\n'] = true;
            _needsEscaping['\r'] = true;
            _needsEscaping['\t'] = true;

            for(int index = 0; index < 32; index++)
            {
                var hex = index.ToString("X4");
                _escapeLookup[index] = "\\u" + hex;
            }
            _escapeLookup['\"'] = "\\\"";
            _escapeLookup ['\\'] = "\\\\";
            _escapeLookup['/'] = "\\/";
            _escapeLookup['\b'] = "\\b";
            _escapeLookup['\f'] = "\\f";
            _escapeLookup['\n'] = "\\n";
            _escapeLookup['\r'] = "\\r";
            _escapeLookup['\t'] = "\\t";
        }

        public static StringBuilder AppendEscaped(this StringBuilder builder, string input)
        {
            int start = 0;
            for(int index = 0; index < input.Length; index++)
            {
                char character = input[index];
                if(character < 93 && _needsEscaping[character])
                {
                    builder.Append(input, start, index - start);
                    builder.Append(_escapeLookup[character]);
                    start = index + 1;
                }
            }
            return builder.Append(input, start, input.Length - start);
        }
    }
}