namespace KryptoMin.Infra.Exceptions
{
    public class NullHttpResponseException : Exception
    {
        public NullHttpResponseException(string message) : base(message)
        {
        }
    }
}