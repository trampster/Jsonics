using System;
using System.Text;

namespace Jsonics
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

        public LazyString(string buffer, int start, int length)
        {
            _buffer = buffer;
            _start = start;
            _length = length;
        }

        public int Length
        {
            get
            {
                return _length;
            }
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
                    return index;
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
            return -1;
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
        static StringBuilder _builder = new StringBuilder();

        public (string, int) ToString(int start)
        {
            _builder.Clear();
            start = _start + start;

            int index = start+1; //first char must be a " so we will skip it
            while(true)
            {
                if(index > _start + _length)
                {
                    throw new Exception("Missing end of string value");
                }
                char value = _buffer[index];
                if(value == '\\')
                {
                    //excape character which means the next char should always be included
                    index++;
                    _builder.Append(_buffer[index]);
                    index++;
                    continue;
                }
                if(value == '\"')
                {
                    //end of string value
                    return (_builder.ToString(), _start + index);
                }
                _builder.Append(value);
                index++;
                
            }
            //we got to the end of the lazy string without finding the end of string character
            throw new Exception("Missing end of string value");
        }

        public (int,int) ToInt(int start)
        {
            start = start + _start;
            int sign = 1;
            if(_buffer[start] == '-')
            {
                start++;
                sign = -1;
            }
            int soFar = 0;

            char character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar += character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';

            start++;
            if(start >= _length) goto Return;
            character = _buffer[start];
            if(!IsNumber(character)) goto Return;
            soFar = (soFar*10) + character - '0';
            start++;

            Return:
            return (soFar * sign, start - _start) ;
        }
    }
}