#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Tools.Controls
{
    public class HtmlTagsParser
    {
        #region Regex constants

        public const string HtmlTag = @"<[^<>]*>";
        public const string HmlTagAttributes = "[^\\s]*\\s*=\\s*(\"[^\"]*\"|\'[^\']*'|[^\\s]*)";
        public const string WhiteSpaces = @"\s{2,}";        
        public const string CssProperties = @";?[^;\s]*:[^\{\}:;]*(\}|;)?";
        public const string CssFont = "(" + @"([0-9]+|[0-9]*\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)" + "|" + @"([0-9]+|[0-9]*\.[0-9]+)\%" + 
            "|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)" + @"(\/" + LineHeight + @")?(\s|$)";
        public const string LineHeight = "(normal|" + @"{[0-9]+|[0-9]*\.[0-9]+}" + "|" + @"([0-9]+|[0-9]*\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)" + 
            "|" + @"([0-9]+|[0-9]*\.[0-9]+)\%" + ")";
        public const string FontStyle = "(normal|italic|oblique)";
        public const string FontVariant = "(normal|small-caps)";
        public const string FontWeight = "(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)";
        public const string CssPropertyBox = @"[^\{\}]*\{[^\{\}]*\}";

        #endregion

        #region Methods
        /// <summary>
        /// Returns the matched collection
        /// </summary>
        public static MatchCollection Match(string regex, string source)
        {
            Regex r = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return r.Matches(source);
        }

        /// <summary>
        /// Searches the specified regex on the source
        /// </summary>
        public static string Search(string regex, string source)
        {
            int position;
            return Search(regex, source, out position);
        }

        /// <summary>
        /// Searches the specified regex on the source
        /// </summary>
        public static string Search(string regex, string source, out int position)
        {
            MatchCollection matches = Match(regex, source);

            if (matches.Count > 0)
            {
                position = matches[0].Index;
                return matches[0].Value;
            }
            else
            {
                position = -1;
            }

            return null;
        }

        #endregion
    }
}
