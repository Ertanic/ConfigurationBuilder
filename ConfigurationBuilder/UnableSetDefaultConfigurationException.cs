using System.Runtime.Serialization;

namespace observe.ConfigurationBuilder
{
    [Serializable]
    internal class UnableSetDefaultConfigurationException : Exception
    {
        public UnableSetDefaultConfigurationException(string? message) : base(message)
        {
        }
    }
}