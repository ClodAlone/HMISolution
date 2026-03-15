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

#region file using directives
using System;
#endregion

namespace Syncfusion.HTMLUI.Base.Parser.CSS
{
  /// <summary>
  /// Specifies tokens which represent elements in a CSS document.
  /// </summary>
  internal enum Token
  {
    /// <summary>
    /// End of file.
    /// </summary>
    EOF,
	  /// <summary>
	  /// Element which must be skipped (Whitespaces).
	  /// </summary>
    S,
    /// <summary>
    /// Comment is opened.
    /// </summary>
    CDO,
    /// <summary>
    /// Comment is closed.
    /// </summary>
    CDC,
    /// <summary>
    /// Attribute has a list of values.
    /// </summary>
    INCLUDES,
    /// <summary>
    /// Attribute has a hyphen-separated attribute selectors list of values.
    /// </summary>
    DASHMATCH,
    /// <summary>
    /// String token.
    /// </summary>
    STRING,
    /// <summary>
    /// Identificator (name of function or keyword).
    /// </summary>
    IDENT,
    /// <summary>
    /// Hash value (for instance, color #xxxxxx).
    /// </summary>
    HASH,
    /// <summary>
    /// Token is "@import" directive.
    /// </summary>
    IMPORT_SYM,
    /// <summary>
    /// Token is "@page" directive.
    /// </summary>
    PAGE_SYM,
    /// <summary>
    /// Token is "@media" directive.
    /// </summary>
    MEDIA_SYM,
    /// <summary>
    /// Token is "@font-face" directive.
    /// </summary>
    FONT_FACE_SYM,
    /// <summary>
    /// Token is "@charset" directive.
    /// </summary>
    CHARSET_SYM,
    /// <summary>
    /// Token is "@"{IDENT} directive.
    /// </summary>
    ATKEYWORD,
    /// <summary>
    /// Token is !important directive of priority.
    /// </summary>
    IMPORTANT_SYM,
    /// <summary>
    /// Token is NUMBERems.
    /// </summary>
    EMS,
    /// <summary>
    /// Token is NUMBERex.
    /// </summary>
    EXS,
    /// <summary>
    /// Token is NUMBERYY where YY = {px| in | cm | mm | pt | pc}.
    /// </summary>
    LENGTH,
    /// <summary>
    /// Token is NUMBERYY where YY = {deg | rad | grad}.
    /// </summary>
    ANGLE,
    /// <summary>
    /// Token is NUMBERYY where YY = {ms | s}.
    /// </summary>
    TIME,
    /// <summary>
    /// Token is NUMBERYY where YY = {hz | Khz}.
    /// </summary>
    FREQ,
    /// <summary>
    /// Token is NUMBERIDENT.
    /// </summary>
    DIMEN,
    /// <summary>
    /// Token is NUMBER%.
    /// </summary>
    PERCENTAGE,
    /// <summary>
    /// Token is number.
    /// </summary>
    NUMBER,
    /// <summary>
    /// Token is url(STRING).
    /// </summary>
    URI,
    /// <summary>
    /// Token is IDENT.
    /// </summary>
    FUNCTION,
    /// <summary>
    /// Token is U+RANGE.
    /// </summary>
    UNICODERANGE,
    /// <summary>
    /// Token is just a symbol.
    /// </summary>
    SYMBOL,
    /// <summary>
    /// Not a recognized token.
    /// </summary>
    unrecognized
  };
}