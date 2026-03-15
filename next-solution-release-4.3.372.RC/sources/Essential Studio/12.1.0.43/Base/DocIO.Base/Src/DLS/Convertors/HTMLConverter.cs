#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !(SILVERLIGHT || WP) || WINRT

#region file using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The HTML converter factory class.
    /// </summary>
    public class HtmlConverterFactory
    {
        #region Class members

        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        private static IHtmlConverter s_htmlConverter = null;
        #endregion

        #region Class static methods

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static IHtmlConverter GetInstance()
        {
            if (s_htmlConverter == null)
            {
                s_htmlConverter = new HTMLConverterImpl();
            }
            return s_htmlConverter;
        }

        /// <summary>
        /// Registers the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public static void Register(IHtmlConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException("convertor");

            s_htmlConverter = converter;
        }

        #endregion
    }

    /// <summary>
    /// HtmlConverter`s Interface.
    /// </summary>
    public interface IHtmlConverter
    {
        /// <summary>
        /// Appends to text body without style
        /// </summary>
        /// <param name="dlsTextBody">The DLS text body.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="paragraphIndex">Index of the paragraph.</param>
        /// <param name="paragraphItemIndex">Index of the paragraph item.</param>
        void AppendToTextBody(ITextBody dlsTextBody, string html, int paragraphIndex, int paragraphItemIndex);
        /// <summary>
        /// Appends to text body with style
        /// </summary>
        /// <param name="dlsTextBody">The DLS text body.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="paragraphIndex">Index of the paragraph.</param>
        /// <param name="paragraphItemIndex">Index of the paragraph item.</param>
        void AppendToTextBody(ITextBody dlsTextBody, string html, int paragraphIndex, int paragraphItemIndex, IWParagraphStyle style, ListStyle listStyle);
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Validates the specified HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	if the specified HTML is valid, set to <c>true</c>.
        /// </returns>
        bool IsValid(string html, XHTMLValidationType type);
        /// <summary>
        /// Determines whether the specified HTML is valid.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="type">The validation type.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>
        /// 	If the specified HTML is valid, set to <c>true</c>.
        /// </returns>
        bool IsValid(string html, XHTMLValidationType type, out string exceptionMessage);
#endif
    }
}

#endif
