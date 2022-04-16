using observe.ConfigurationBuilder;

namespace observe.ConfigurationsModels
{
    [Serializable]
    public class ConfigModel : Consolable, IObserverFields
    {
        public event IObserverFields.FieldChangeHandler? FieldChange;

        public string Name { get; set; } = "Super App!";
        public string? ConnectionString { get; set; }
    }
}
