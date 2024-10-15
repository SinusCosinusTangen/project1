using System;

namespace project1.Exceptions
{
    public class UserExistsException : Exception
    {
        public UserExistsException() : base("User already exists")
        {
        }

        public UserExistsException(string message) : base(message)
        {
        }

        public UserExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class WrongPasswordException : Exception
    {
        public WrongPasswordException() : base("The password is incorrect")
        {
        }

        public WrongPasswordException(string message) : base(message)
        {
        }

        public WrongPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EmptyTokenException : Exception
    {
        public EmptyTokenException() : base("Token is null or empty")
        {
        }

        public EmptyTokenException(string message) : base(message)
        {
        }

        public EmptyTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}