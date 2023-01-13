namespace KryptoMin.Application.Settings
{
    public class EmailSettings
    {
        public EmailSettings(string apiKey, string address, string name, string content, bool sendingTurnedOn)
        {
            ApiKey = apiKey;
            Address = address;
            Name = name;
            Content = content;
            SendingTurnedOn = sendingTurnedOn;
        }

        public string Content { get; }
        public string ApiKey { get; }
        public string Address { get; }
        public string Name { get; }
        public bool SendingTurnedOn { get; }
    }
}