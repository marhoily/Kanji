using System.Collections.Generic;

namespace Kanji.Tests
{
    public static class StringUtils
    {
        public static string Quote(this string str) => $"\"{str}\"";

        public static string StrJoin<T>(this IEnumerable<T> src, string separator = ", ")
        {
            return string.Join(separator, src);
        }
    }
}