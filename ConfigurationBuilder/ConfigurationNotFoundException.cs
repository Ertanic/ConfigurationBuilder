namespace observe.ConfigurationBuilder
{
    public class ConfigurationNotFoundException : SystemException
    {
        public ConfigurationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
