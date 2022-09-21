namespace KryptoMin.Application.Settings
{
    public class EmailSettings
    {
        public EmailSettings(string apiKey, string address, string name)
        {
            ApiKey = apiKey;
            Address = address;
            Name = name;
        }

        public string ApiKey { get; }
        public string Address { get; }
        public string Name { get; }
    }
}