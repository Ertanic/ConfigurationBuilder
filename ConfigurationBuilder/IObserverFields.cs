namespace observe.ConfigurationBuilder
{
    public interface IObserverFields
    {
        public delegate void FieldChangeHandler(FieldChangeEventArgs eventArgs);
        public event FieldChangeHandler? FieldChange;
    }
}
