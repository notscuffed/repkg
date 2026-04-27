namespace RePKG
{
    public static class Extensions
    {
        public static bool Contains(this string haystack, string needle, StringComparison comparer)
        {
            return haystack?.IndexOf(needle, comparer) >= 0;
        }

        public static string GetSafeFilename(this string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}