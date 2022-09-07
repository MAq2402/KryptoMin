using System;

namespace KryptoMin.Infra.Exceptions
{
    public class FailedToSendEmailException : Exception
    {
        public FailedToSendEmailException(string message) : base(message)
        {
        }
    }
}