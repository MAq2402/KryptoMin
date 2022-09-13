namespace KryptoMin.Application.Settings
{
    public class EmailSettings
    {
        public EmailSettings(string apiKey)
        {
            ApiKey = apiKey;
        }

        public string ApiKey { get; }
    }
}