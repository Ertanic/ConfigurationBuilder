namespace observe.ConfigurationBuilder
{
    public class FieldChangeEventArgs
    {
        public string FieldName = null!;
        public object? FieldOldValue;
        public object? FieldNewValue;

        public FieldChangeEventArgs(string fieldName, object? fieldOldValue, object? fieldNewValue)
        {
            FieldName = fieldName;
            FieldOldValue = fieldOldValue;
            FieldNewValue = fieldNewValue;
        }
    }
}
