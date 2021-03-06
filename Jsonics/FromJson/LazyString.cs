using System;
using System.Text;
using Jsonics.FromJson;

namespace Jsonics.FromJson
{
    public struct LazyString
    {
        readonly string _buffer;
        readonly int _start;
        readonly int _length;

        public LazyString(string buffer)
        {
            _buffer = buffer;
            _start = 0;
            _length = buffer.Length;
        }

        public string Buffer => _buffer;
        public int Start => _start;
        public int Length => _length;

        public LazyString(string buffer, int start, int length)
        {
            _buffer = buffer;
            _start = start;
            _length = length;
        }

        public LazyString SubString(int start, int length)
        {
            return new LazyString(_buffer, _start + start, length);
        }

        public override string ToString()
        {
            return _buffer.Substring(_start, _length);
        }

        public int ReadTo(int start, char value)
        {
            int end = _start + _length;
            for(int index = _start + start; index < end; index++)
            {
                if(_buffer[index] == value)
                {
                    return index - _start;
                }
            }
            return -1;
        }

        public (int, char) ReadToAny(int start, char value1, char value2)
        {
            int end = _start + _length;
            for(int index = _start + start; index < end; index++)
            {
                if(_buffer[index] == value1)
                {
                    return (index - _start, value1);
                }
                else if(_buffer[index] == value2)
                {
                    return (index - _start, value2);
                }
            }
            return (-1, ' ');
        }

        public int SkipWhitespace(int start)
        {
            int end = _start + _length;
            for(int index = _start + start; index < end; index++)
            {
                var character = _buffer[index];
                switch(character)
                {
                    case ' ':
                    case '\r':
                    case '\n':
                    case '\t':
                        continue;
                    default:
                        return index - _start;
                }
            }
            return -1;
        }

        public int ReadToPropertyValueEnd(int start)
        {
            int unclosedBrakets = 0;
            int end = _start + _length;
            for(int index = _start + start; index < end; index++)
            {
                int value = _buffer[index];
                if(value == '{')
                {
                    unclosedBrakets++;
                }
                else if(value == '}')
                {
                    if(unclosedBrakets == 0)
                    {
                        //end of object
                        return index;
                    }
                    unclosedBrakets--;
                }
                else if(value == '\\')
                {
                    index++; //skip because next character is escaped
                    continue;
                }
                
                if(value == ',' && unclosedBrakets == 0)
                {
                    return index;
                }
            }
            throw new InvalidJsonException($"Property starting at {start + _start} didn't end.");
        }

        public char At(int index)
        {
            return _buffer[index + _start];
        }

        public bool EqualsString(string other)
        {
            if(_length != other.Length)
            {
                return false;
            }
            for(int index = 0; index < other.Length; index++)
            {
                if(other[index] != _buffer[index + _start])
                {
                    return false;
                }
            }
            return true;
        }

        bool IsNumber(char character)
        {
            return character >= '0' && character <= '9';
        }
        
        [ThreadStatic]
        static StringBuilder _builder = new StringBuilder();

        public (string, int) ToString(int start)
        {
            int index = start + _start;
            while(true)
            {
                var value = _buffer[index];
                if(value == 'n')
                {
                    //null
                    return (null, index + 4 - _start);
                }
                else if(value == '\"')
                {
                    break;
                }
                index++;
            }
            _builder.Clear();

            index++;
            while(true)
            {
                char value = _buffer[index];
                if(value == '\\')
                {
                    //escape character
                    index++;
                    value = _buffer[index];
                    switch(value)
                    {
                        case '\"':
                        case '\\':
                        case '/':
                            _builder.Append(value);
                            break;
                        case 'b':
                            _builder.Append('\b');
                            break;
                        case 'f':
                            _builder.Append('\f');
                            break;
                        case 'n':
                            _builder.Append('\n');
                            break;
                        case 'r':
                            _builder.Append('\r');
                            break;
                        case 't':
                            _builder.Append('\t');
                            break;
                        case 'u':
                            index++;
                            _builder.Append(FromHex(index));
                            index += 3;
                            break;
                    }

                    index++;
                    continue;
                }
                else if(value == '\"')
                {
                    //end of string value
                    return (_builder.ToString(), index + 1 - _start);
                }
                _builder.Append(value);
                index++;
                
            }
            //we got to the end of the lazy string without finding the end of string character
            throw new InvalidJsonException("Missing end of string value");
        }

        public (char, int) ToChar(int start)
        {
            int index = start + _start;
            char value;
            while(true)
            {
                value = _buffer[index];
                if(value == '\"')
                {
                    break;
                }
                index++;
            }

            index++;
            value = _buffer[index];
            if(value == '\\')
            {
                //escape character
                index++;
                value = _buffer[index];
                switch(value)
                {
                    case '\"':
                    case '\\':
                    case '/':
                        return (value, index + 2 - _start);
                    case 'b':
                        return ('\b', index + 2 - _start);
                    case 'f':
                        return ('\f', index + 2 - _start);
                    case 'n':
                        return ('\n', index + 2 - _start);
                    case 'r':
                        return ('\r', index + 2 - _start);
                    case 't':
                        return ('\t', index + 2 - _start);
                    case 'u':
                        index++;
                        return (FromHex(index), index + 5 - _start);
                    default:
                        throw new InvalidJsonException("Invalid char escape sequence");
                }
            }
            index++;
            return (value, index + 1 - _start);
        }

        public (char?, int) ToNullableChar(int start)
        {
            int index = start + _start;
            char value;
            while(true)
            {
                value = _buffer[index];
                if(value == 'n')
                {
                    //null
                    return (null, index + 4 - _start);
                }
                else if(value == '\"')
                {
                    break;
                }
                index++;
            }

            index++;
            value = _buffer[index];
            if(value == '\\')
            {
                //escape character
                index++;
                value = _buffer[index];
                switch(value)
                {
                    case '\"':
                    case '\\':
                    case '/':
                        return (value, index + 2 - _start);
                    case 'b':
                        return ('\b', index + 2 - _start);
                    case 'f':
                        return ('\f', index + 2 - _start);
                    case 'n':
                        return ('\n', index + 2 - _start);
                    case 'r':
                        return ('\r', index + 2 - _start);
                    case 't':
                        return ('\t', index + 2 - _start);
                    case 'u':
                        index++;
                        return (FromHex(index), index + 5 - _start);
                    default:
                        throw new InvalidJsonException("Invalid char escape sequence");
                }
            }
            index++;
            return (value, index + 1 - _start);
        }

        byte FromHexChar(char character)
        {
            switch(character)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'A': return 10;
                case 'b': return 11;
                case 'B': return 11;
                case 'c': return 12;
                case 'C': return 12;
                case 'd': return 13;
                case 'D': return 13;
                case 'e': return 14;
                case 'E': return 14;
                case 'f': return 15;
                case 'F': return 15;
                default: 
                    throw new InvalidJsonException("character must be a hex value");
            }
        }

        char FromHex(int internalStart)
        {
            int value =
                (FromHexChar(_buffer[internalStart]) << 12) +
                (FromHexChar(_buffer[internalStart + 1]) << 8) +
                (FromHexChar(_buffer[internalStart + 2]) << 4) +
                (FromHexChar(_buffer[internalStart + 3]));
            return (char)value;
        }

        public (bool, int) ToBool(int start)
        {
            int index = start + _start;
            while(true)
            {
                var value = _buffer[index];
                if(value == 't')
                {
                    return (true, index + 4 - _start);
                }
                else if(value == 'f')
                {
                    return (false, index + 5 - _start);
                }
                index++;
            }
        }

        public (bool?, int) ToNullableBool(int start)
        {
            int index = start + _start;
            while(true)
            {
                var character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(character == 't')
                {
                    return (true, index + 4 - _start);
                }
                else if(character == 'f')
                {
                    return (false, index + 5 - _start);
                }
                index++;
            }
        }

        public (sbyte, int) ToSByte(int start)
        {
            (int result, int index) = ToInt(start);
            return ((sbyte)result, index);
        }

        public (byte,int) ToByte(int start)
        {
            int index = start + _start;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                index++;
            }

            int end = _start + _length;
            //read byte
            int soFar = 0;
            if(!IsNumber(character)) goto Return;
            soFar += character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            
            index++;

            Return:
            return ((byte)soFar, index - _start) ;
        }

        public (byte?,int) ToNullableByte(int start)
        {
            int index = start + _start;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(IsNumber(character))
                {
                    break;
                }
                index++;
            }

            int end = _start + _length;
            //read byte
            int soFar = 0;
            if(!IsNumber(character)) goto Return;
            soFar += character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            
            index++;

            Return:
            return ((byte)soFar, index - _start) ;
        }

        public (short, int) ToShort(int start)
        {
            (int result, int index) = ToInt(start);
            return ((short)result, index);
        }

        public (short?, int) ToNullableShort(int start)
        {
            (int? result, int index) = ToNullableInt(start);
            return ((short?)result, index);
        }

        public (ushort, int) ToUShort(int start)
        {
            (uint result, int index) = ToUInt(start);
            return ((ushort)result, index);
        }

        public (ushort?, int) ToNullableUShort(int start)
        {
            (uint? result, int index) = ToNullableUInt(start);
            return ((ushort?)result, index);
        }

        public (sbyte?, int) ToNullableSByte(int start)
        {
            (int? result, int index) = ToNullableInt(start);
            return ((sbyte?)result, index);
        }

        public (int,int) ToInt(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;
            int soFar = 0;
            

            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar += character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            index++;

            Return:
            return (soFar * sign, index - _start) ;
        }

        public (int?,int) ToNullableInt(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;
            int soFar = 0;
            

            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar += character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            index++;

            Return:
            return (soFar * sign, index - _start) ;
        }

        public (uint,int) ToUInt(int start)
        {
            int index = start + _start;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                index++;
            }

            int end = _start + _length;
            uint soFar = 0;
            

            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar += (uint)character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            index++;

            Return:
            return (soFar, index - _start) ;
        }

        public (uint?,int) ToNullableUInt(int start)
        {
            int index = start + _start;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if (IsNumber(character))
                {
                    break;
                }
                index++;
            }

            int end = _start + _length;
            uint soFar = 0;
            

            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar += (uint)character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            index++;
            if(index >= end) goto Return;
            character = _buffer[index];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            index++;

            Return:
            return (soFar, index - _start) ;
        }

        public (decimal, int) ToDecimal(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;


            decimal wholePart = 0;
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                wholePart = (wholePart*10M) + (character - '0');
            }

            decimal fractionalPart = 0;
            if(character == '.')
            {
                long fractionalValue = 0;
                int factionalLength = 0;
                for(index = index+1; index < end; index++)
                {
                    character = _buffer[index];
                    if(!IsNumber(character)) break;
                    fractionalValue = (fractionalValue*10) + character - '0';
                    factionalLength++;
                }
                decimal divisor = (decimal)Math.Pow(10, factionalLength);
                fractionalPart = fractionalValue/divisor;
            }

            return (sign*(wholePart + fractionalPart), index - _start);
        }

        public (decimal?, int) ToNullableDecimal(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;


            decimal wholePart = 0;
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                wholePart = (wholePart*10M) + (character - '0');
            }

            decimal fractionalPart = 0;
            if(character == '.')
            {
                long fractionalValue = 0;
                int factionalLength = 0;
                for(index = index+1; index < end; index++)
                {
                    character = _buffer[index];
                    if(!IsNumber(character)) break;
                    fractionalValue = (fractionalValue*10) + character - '0';
                    factionalLength++;
                }
                decimal divisor = (decimal)Math.Pow(10, factionalLength);
                fractionalPart = fractionalValue/divisor;
            }

            return (sign*(wholePart + fractionalPart), index - _start);
        }

        public (long, int) ToLong(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;
            long soFar = 0;
        
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                soFar = (soFar*10) + character - '0';
            }

            return (soFar * sign, index - _start) ;
        }

        public (long?, int) ToNullableLong(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;
            long soFar = 0;
        
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                soFar = (soFar*10) + character - '0';
            }

            return (soFar * sign, index - _start) ;
        }

        public (ulong, int) ToULong(int start)
        {
            int index = start + _start;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                index++;
            }

            int end = _start + _length;
            ulong soFar = 0;
        
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                soFar = (soFar*10) + character - '0';
            }

            return (soFar, index - _start) ;
        }

        public (ulong?, int) ToNullableULong(int start)
        {
            int index = start + _start;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(IsNumber(character))
                {
                    break;
                }
                index++;
            }

            int end = _start + _length;
            ulong soFar = 0;
        
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                soFar = (soFar*10) + character - '0';
            }

            return (soFar, index - _start) ;
        }

        public (double?, int) ToNullableDouble(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - _start);
                }
                else if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;


            double wholePart = 0;
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                wholePart = (wholePart*10) + character - '0';
            }

            double fractionalPart = 0;
            if(character == '.')
            {
                long fractionalValue = 0;
                int factionalLength = 0;
                for(index = index+1; index < end; index++)
                {
                    character = _buffer[index];
                    if(!IsNumber(character)) break;
                    fractionalValue = (fractionalValue*10) + character - '0';
                    factionalLength++;
                }
                double divisor = Math.Pow(10, factionalLength);
                fractionalPart = fractionalValue/divisor;
            }

            int exponentPart = 0;
            if(character == 'E' || character == 'e')
            {
                index++;
                character = _buffer[index];
                int exponentSign = 1;
                if(character == '-')
                {
                    index++;
                    exponentSign = -1;
                }
                else if(character == '+')
                {
                    index++;
                }

                for(index = index+0; index < end; index++)
                {
                    character = _buffer[index];
                    if(!IsNumber(character)) break;
                    exponentPart = (exponentPart*10) + character - '0';
                }

                exponentPart *= exponentSign;
            }
            else
            {
                return (sign*(wholePart + fractionalPart), index - _start);
            }
            double value = sign*(wholePart + fractionalPart) * Math.Pow(10, exponentPart);
            return (value, index - _start);
        }

        public (double, int) ToDouble(int start)
        {
            int index = start + _start;
            int sign = 1;
            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[index];
                if(IsNumber(character))
                {
                    break;
                }
                else if(character == '-')
                {
                    index++;
                    sign = -1;
                    break;
                }
                index++;
            }

            int end = _start + _length;


            double wholePart = 0;
            for(index = index+0; index < end; index++)
            {
                character = _buffer[index];
                if(!IsNumber(character)) break;
                wholePart = (wholePart*10) + character - '0';
            }

            double fractionalPart = 0;
            if(character == '.')
            {
                long fractionalValue = 0;
                int factionalLength = 0;
                for(index = index+1; index < end; index++)
                {
                    character = _buffer[index];
                    if(!IsNumber(character)) break;
                    fractionalValue = (fractionalValue*10) + character - '0';
                    factionalLength++;
                }
                double divisor = Math.Pow(10, factionalLength);
                fractionalPart = fractionalValue/divisor;
            }

            int exponentPart = 0;
            if(character == 'E' || character == 'e')
            {
                index++;
                character = _buffer[index];
                int exponentSign = 1;
                if(character == '-')
                {
                    index++;
                    exponentSign = -1;
                }
                else if(character == '+')
                {
                    index++;
                }

                for(index = index+0; index < end; index++)
                {
                    character = _buffer[index];
                    if(!IsNumber(character)) break;
                    exponentPart = (exponentPart*10) + character - '0';
                }

                exponentPart *= exponentSign;
            }
            else
            {
                return (sign*(wholePart + fractionalPart), index - _start);
            }
            double value = sign*(wholePart + fractionalPart) * Math.Pow(10, exponentPart);
            return (value, index - _start);
        }

        public (float, int) ToFloat(int start)
        {
            (double value, int index) = ToDouble(start);
            return ((float)value, index);
        }

        public (float?, int) ToNullableFloat(int start)
        {
            (double? value, int index) = ToNullableDouble(start);
            return ((float?)value, index);
        }

        public (Guid, int) ToGuid(int start)
        {
            start += _start;

            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[start];
                start++;
                if(character ==  '\"')
                {
                    break;
                }
            }
            uint a =
                ((uint)FromHexChar(_buffer[start]) << 28) + 
                ((uint)FromHexChar(_buffer[start + 1]) << 24) + 
                (uint)(FromHexChar(_buffer[start + 2]) << 20) +
                (uint)(FromHexChar(_buffer[start + 3]) << 16) +
                (uint)(FromHexChar(_buffer[start + 4]) << 12) +
                (uint)(FromHexChar(_buffer[start + 5]) << 8) +
                (uint)(FromHexChar(_buffer[start + 6]) << 4) +
                (uint)FromHexChar(_buffer[start + 7]);
            
            ushort b = (ushort)
                ((FromHexChar(_buffer[start + 9]) << 12) +
                (FromHexChar(_buffer[start + 10]) << 8) +
                (FromHexChar(_buffer[start + 11]) << 4) +
                FromHexChar(_buffer[start + 12]));

            ushort c = (ushort)
                ((FromHexChar(_buffer[start + 14]) << 12) +
                (FromHexChar(_buffer[start + 15]) << 8) +
                (FromHexChar(_buffer[start + 16]) << 4) +
                FromHexChar(_buffer[start + 17]));

            byte d = (byte)((FromHexChar(_buffer[start + 19]) << 4) + FromHexChar(_buffer[start + 20]));
            byte e = (byte)((FromHexChar(_buffer[start + 21]) << 4) + FromHexChar(_buffer[start + 22]));

            byte f = (byte)((FromHexChar(_buffer[start + 24]) << 4) + FromHexChar(_buffer[start + 25]));
            byte g = (byte)((FromHexChar(_buffer[start + 26]) << 4) + FromHexChar(_buffer[start + 27]));
            byte h = (byte)((FromHexChar(_buffer[start + 28]) << 4) + FromHexChar(_buffer[start + 29]));
            byte i = (byte)((FromHexChar(_buffer[start + 30]) << 4) + FromHexChar(_buffer[start + 31]));
            byte j = (byte)((FromHexChar(_buffer[start + 32]) << 4) + FromHexChar(_buffer[start + 33]));
            byte k = (byte)((FromHexChar(_buffer[start + 34]) << 4) + FromHexChar(_buffer[start + 35]));

            var guid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
            return (guid, start + 37 - _start);
        }

        public (Guid?, int) ToNullableGuid(int start)
        {
            start += _start;

            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = _buffer[start];
                start++;
                if(character == 'n')
                {
                    if(character == 'n')
                    {
                        return (null, start + 3 - _start);
                    }
                }
                else if (character ==  '\"')
                {
                    break;
                }
            }
            uint a =
                ((uint)FromHexChar(_buffer[start]) << 28) + 
                ((uint)FromHexChar(_buffer[start + 1]) << 24) + 
                (uint)(FromHexChar(_buffer[start + 2]) << 20) +
                (uint)(FromHexChar(_buffer[start + 3]) << 16) +
                (uint)(FromHexChar(_buffer[start + 4]) << 12) +
                (uint)(FromHexChar(_buffer[start + 5]) << 8) +
                (uint)(FromHexChar(_buffer[start + 6]) << 4) +
                (uint)FromHexChar(_buffer[start + 7]);
            
            ushort b = (ushort)
                ((FromHexChar(_buffer[start + 9]) << 12) +
                (FromHexChar(_buffer[start + 10]) << 8) +
                (FromHexChar(_buffer[start + 11]) << 4) +
                FromHexChar(_buffer[start + 12]));

            ushort c = (ushort)
                ((FromHexChar(_buffer[start + 14]) << 12) +
                (FromHexChar(_buffer[start + 15]) << 8) +
                (FromHexChar(_buffer[start + 16]) << 4) +
                FromHexChar(_buffer[start + 17]));

            byte d = (byte)((FromHexChar(_buffer[start + 19]) << 4) + FromHexChar(_buffer[start + 20]));
            byte e = (byte)((FromHexChar(_buffer[start + 21]) << 4) + FromHexChar(_buffer[start + 22]));

            byte f = (byte)((FromHexChar(_buffer[start + 24]) << 4) + FromHexChar(_buffer[start + 25]));
            byte g = (byte)((FromHexChar(_buffer[start + 26]) << 4) + FromHexChar(_buffer[start + 27]));
            byte h = (byte)((FromHexChar(_buffer[start + 28]) << 4) + FromHexChar(_buffer[start + 29]));
            byte i = (byte)((FromHexChar(_buffer[start + 30]) << 4) + FromHexChar(_buffer[start + 31]));
            byte j = (byte)((FromHexChar(_buffer[start + 32]) << 4) + FromHexChar(_buffer[start + 33]));
            byte k = (byte)((FromHexChar(_buffer[start + 34]) << 4) + FromHexChar(_buffer[start + 35]));

            var guid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
            return (guid, start + 37 - _start);
        }
    }
}