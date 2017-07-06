using System;

namespace Jsonics.FromJson
{
    public class InvalidJsonException : FormatException
    {
        public InvalidJsonException(string message)
            : base(message)
        {
        }
    }
}