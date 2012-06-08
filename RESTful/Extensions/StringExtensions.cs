using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string ToCommaDelimitedString<T>(this IEnumerable<T> source)
        {
            int count = 10;

            ICollection<T> col;
            if ((col = source as ICollection<T>) != null)
                count = col.Count;

            const int guessItemSize = 2;

            StringBuilder sb = new StringBuilder(count * guessItemSize);
            using (var en = source.GetEnumerator())
            {
                bool first = true;
                while (en.MoveNext())
                {
                    if (!first) sb.Append(',');
                    else first = false;
                    sb.Append(en.Current);
                }
            }

            return sb.ToString();
        }
    }
}
