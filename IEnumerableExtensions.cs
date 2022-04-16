namespace observe
{
    public static class IEnumerableExtensions
    {
        public static string Join<T>(this IEnumerable<T> data, string? separator) => String.Join(separator, data);
    }
}
