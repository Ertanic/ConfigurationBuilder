using observe.ConfigurationBuilder;

namespace observe.ConfigurationsModels
{
    public class AppleModel : Consolable, IObserverFields
    {
        public event IObserverFields.FieldChangeHandler? FieldChange;

        public string AppleColor { get; set; } = "Green";
        public int Size { get; set; } = 5;
    }
}
