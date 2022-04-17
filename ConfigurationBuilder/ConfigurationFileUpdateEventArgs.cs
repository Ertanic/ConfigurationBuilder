namespace observe.ConfigurationBuilder
{
    public class ConfigurationFileUpdateEventArgs
    {
        public FileInfo file { get; private set; } = null!;
        public object? Configuration { get; private set; }
        public List<(string fieldName, object? newValue)> FieldsUpdate { get; } = new();

        public ConfigurationFileUpdateEventArgs(FileInfo file, object? configuration, List<(string fieldName, object? newValue)> fieldsUpdate)
        {
            this.file = file;
            Configuration = configuration;
            FieldsUpdate = fieldsUpdate;
        }
    }
}
