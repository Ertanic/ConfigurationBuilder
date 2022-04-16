namespace observe.ConfigurationBuilder
{
    public class Consolable
    {
        public string GetPrintableObject(string? title = null)
        {
            var options = new System.Text.Json.JsonSerializerOptions() { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(this, this.GetType(), options);

            return String.IsNullOrWhiteSpace(title) ? json : $"{title} {json}";
        }
    }
}