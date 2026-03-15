#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Interface publishes text range functionality.
    /// </summary>
    public interface ITextBodyItem : IEntity
    {
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        int Replace(Regex pattern, string replace);
        /// <summary>
        /// Replaces by specified given string.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        int Replace(string given, string replace, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces by specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        int Replace(Regex pattern, TextSelection textSelection);
    }
}
