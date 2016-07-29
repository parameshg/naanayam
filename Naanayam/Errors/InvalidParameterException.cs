using System;

namespace Naanayam.Errors
{
    public class InvalidParameterException : Exception
    {
        public string Parameter { get; set; }

        public InvalidParameterException()
        {
        }

        public InvalidParameterException(string parameter)
        {
            Parameter = parameter;
        }
    }
}