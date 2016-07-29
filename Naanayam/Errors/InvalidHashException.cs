using System;

namespace Naanayam.Errors
{
    public class InvalidHashException : Exception
    {
        public string Parameter { get; set; }

        public InvalidHashException()
        {
        }

        public InvalidHashException(string parameter)
        {
            Parameter = parameter;
        }
    }
}