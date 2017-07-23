using System;

namespace Jsonic
{
    public class InvalidJsonException : Exception
    {
        public InvalidJsonException(string message)
            : base(message)
        {
        }
    }
}