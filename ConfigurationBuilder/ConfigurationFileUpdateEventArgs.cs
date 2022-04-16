namespace observe.ConfigurationBuilder
{
    public class ConfigurationFileUpdateEventArgs
    {
        public FileInfo file { get; private set; } = null!;
        public object? Configuration { get; private set; }
        public Dictionary<string, object?> FieldsUpdate { get; } = new();

        public ConfigurationFileUpdateEventArgs(FileInfo file, object? configuration, Dictionary<string, object?> fieldsUpdate)
        {
            this.file = file;
            Configuration = configuration;
            FieldsUpdate = fieldsUpdate;
        }
    }
}
