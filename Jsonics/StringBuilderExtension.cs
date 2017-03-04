using System.Collections.Generic;
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

        public static StringBuilder AppendList(this StringBuilder builder, List<int> property)
        {
            builder.Append('[');
            if(property.Count >= 1)
            {
                builder.AppendInt(property[0]);
            }
            for(int index = 1; index < property.Count; index++)
            {
                builder.Append(',');
                builder.AppendInt(property[index]);
            }
            builder.Append(']');
            return builder;
        }

        public static StringBuilder AppendList<T>(this StringBuilder builder, List<T> property)
        {
            builder.Append('[');
            if(property.Count >= 1)
            {
                builder.Append(property[0]);
            }
            for(int index = 1; index < property.Count; index++)
            {
                builder.Append(',');
                builder.Append(property[index]);
            }
            builder.Append(']');
            return builder;
        }

        public static StringBuilder AppendList<T>(this StringBuilder builder, T[] property)
        {
            builder.Append('[');
            if(property.Length >= 1)
            {
                builder.Append(property[0]);
            }
            for(int index = 1; index < property.Length; index++)
            {
                builder.Append(',');
                builder.Append(property[index]);
            }
            builder.Append(']');
            return builder;
        }

        public static StringBuilder AppendList(this StringBuilder builder, int[] property)
        {
            builder.Append('[');
            if(property.Length >= 1)
            {
                builder.AppendInt(property[0]);
            }
            for(int index = 1; index < property.Length; index++)
            {
                builder.Append(',');
                builder.AppendInt(property[index]);
            }
            builder.Append(']');
            return builder;
        }

        public static StringBuilder AppendList(this StringBuilder builder, List<string> property)
        {
            if(property.Count >= 1)
            {
                builder.Append("[\"");
                builder.AppendEscaped(property[0]);
            }
            else
            {
                builder.Append("[]");
                return builder;
            }
            for(int index = 1; index < property.Count; index++)
            {
                builder.Append("\",\"");
                builder.AppendEscaped(property[index]);
            }
            builder.Append("\"]");
            return builder;
        }

        public static StringBuilder AppendList(this StringBuilder builder, string[] property)
        {
            if(property.Length >= 1)
            {
                builder.Append("[\"");
                builder.AppendEscaped(property[0]);
            }
            else
            {
                builder.Append("[]");
                return builder;
            }
            for(int index = 1; index < property.Length; index++)
            {
                builder.Append("\",\"");
                builder.AppendEscaped(property[index]);
            }
            builder.Append("\"]");
            return builder;
        }

        public static StringBuilder AppendInt(this StringBuilder builder, int val)
        {
            int number;
            
            if(val < 0)
            {
                builder.Append('-');
                uint num = (uint)(val*-1);
                if(num > int.MaxValue)
                {
                    //deal with the billions so that we can fit the rest in the faster int type
                    uint billionsUint = num/1000000000;
                    uint soFarUint = billionsUint*1000000000;
                    builder.Append((char)('0' + billionsUint));
                    num = num - soFarUint;
                }
                number = (int)num;
            }
            else
            {
                number = val;
            }

            int soFar = 0;

            if(number < 1000000)
            {
                if(number < 100)
                {
                    if(number < 10) goto Ones;
                    goto Tens;
                }
                if(number < 10000)
                {
                    if(number < 1000) goto Hundreds;
                    goto Thousands;
                }
                if(number < 100000) goto TenThousands;
                goto HundredThousands;
            }
            if(number < 100000000)
            {
                if(number < 10000000) goto Millions;
                goto TenMillions;
            }
            if(number < 1000000000) goto HundredMillions;

            int billions = (number)/1000000000;
            soFar += billions*1000000000;
            builder.Append((char)('0' + billions));

            HundredMillions:
            int hundredMillions = (number-soFar)/100000000;
            soFar += hundredMillions*100000000;
            builder.Append((char)('0' + hundredMillions));

            TenMillions:
            int tenMillions = (number-soFar)/10000000;
            soFar += tenMillions*10000000;
            builder.Append((char)('0' + tenMillions));

            Millions:
            int millions = (number-soFar)/1000000;
            soFar += millions*1000000;
            builder.Append((char)('0' + millions));

            HundredThousands:
            int hundredThousands = (number-soFar)/100000;
            soFar += hundredThousands*100000;
            builder.Append((char)('0' + hundredThousands));

            TenThousands:
            int tenThousands = (number-soFar)/10000;
            soFar += tenThousands*10000;
            builder.Append((char)('0' + tenThousands));

            Thousands:
            int thousands = (number-soFar)/1000;
            soFar += thousands*1000;
            builder.Append((char)('0' + thousands));

            Hundreds:
            int hundreds = (number-soFar)/100;
            soFar += hundreds*100;
            builder.Append((char)('0' + hundreds));
            
            Tens:
            int tens = (number - soFar)/10;
            soFar += tens*10;
            builder.Append((char)('0' + tens));

            Ones:
            int ones = number - soFar;
            return builder.Append((char)('0' + ones));
        }
    }
}