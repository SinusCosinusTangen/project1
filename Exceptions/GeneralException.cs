using System;

namespace project1.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Data not found")
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
