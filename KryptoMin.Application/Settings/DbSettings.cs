namespace KryptoMin.Application.Settings
{
    public class DbSettings
    {
        public DbSettings(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }
    }
}