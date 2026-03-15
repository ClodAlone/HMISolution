using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities
{
    // http://www.codeproject.com/Articles/368258/I-dont-like-Regex
    public static class RegexExt
    {
        public static int IndexOfNth(this string input,
                                     string value, int startIndex, int nth)
        {
            if (nth < 1)
                throw new NotSupportedException("Param 'nth' must be greater than 0!");
            if (nth == 1)
                return input.IndexOf(value, startIndex);
            var idx = input.IndexOf(value, startIndex);
            if (idx == -1)
                return -1;
            return input.IndexOfNth(value, idx + 1, --nth);
        }

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        public static string StringCheck(string str)
        {
            string newstr = null;
            var regexItem = new Regex("[^a-zA-Z0-9_.]+");
            if (regexItem.IsMatch(str[str.Length - 1].ToString()))
            {
                newstr = str.Remove(str.Length - 1);
            }
            string replacestr = Regex.Replace(newstr, "[^a-zA-Z0-9_]+", "-");
            return replacestr;
        }

        public static bool Like(this string item, string searchPattern)
        {
            return GetRegex("^" + searchPattern).IsMatch(item);
        }

        public static string Search(this string item, string searchPattern)
        {
            var match = GetRegex(searchPattern).Match(item);
            if (match.Success)
            {
                return item.Substring(match.Index, match.Length);
            }
            return null;
        }

        public static List<string> Extract(this string item, string searchPattern)
        {
            var result = item.Search(searchPattern);
            if (!string.IsNullOrWhiteSpace(result))
            {
                var splitted = searchPattern.Split(new[] { '?', '%', '*', '#' }, StringSplitOptions.RemoveEmptyEntries);
                var temp = result;
                var final = new List<string>();
                Array.ForEach(splitted, x =>
                {
                    var pos = temp.IndexOf(x);
                    if (pos > 0)
                    {
                        final.Add(temp.Substring(0, pos));
                        temp = temp.Substring(pos);
                    }
                    temp = temp.Substring(x.Length);
                });
                if (temp.Length > 0) final.Add(temp);
                return final;
            }
            return null;
        }

        // private method which accepts the simplified pattern and transform it into a valid .net regex pattern:
        // it escapes standard regex syntax reserved characters 
        // and transforms the simplified syntax into the native Regex one
        static Regex GetRegex(string searchPattern)
        {
            return new Regex(searchPattern
                    .Replace("\\", "\\\\")
                    .Replace(".", "\\.")
                    .Replace("{", "\\{")
                    .Replace("}", "\\}")
                    .Replace("[", "\\[")
                    .Replace("]", "\\]")
                    .Replace("+", "\\+")
                    .Replace("$", "\\$")
                    .Replace(" ", "\\s")
                    .Replace("#", "[0-9]")
                    .Replace("?", ".")
                    .Replace("*", "\\w*")
                    .Replace("%", ".*")
                    , RegexOptions.IgnoreCase);
        }
    }
}
