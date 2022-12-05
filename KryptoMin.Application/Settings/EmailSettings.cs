namespace KryptoMin.Application.Settings
{
    public class EmailSettings
    {
        public EmailSettings(string apiKey, string address, string name, string content)
        {
            ApiKey = apiKey;
            Address = address;
            Name = name;
            Content = content;
        }

        public string Content { get; set; }
        public string ApiKey { get; }
        public string Address { get; }
        public string Name { get; }
    }
}