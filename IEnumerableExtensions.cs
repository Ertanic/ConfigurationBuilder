namespace observe
{
    public static class IEnumerableExtensions
    {
        public static string Join<T>(this IEnumerable<T> data, string? separator) => String.Join(separator, data);
        public static string Join<T>(this IEnumerable<T> data, string? separator, Func<T, string> callback)
        {
            var mappedData = data.Select(x => callback(x));

            return String.Join(separator, mappedData);
        }
    }
}
