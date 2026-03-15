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

using System;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Holds all tokens which might appear in every PDF file.
    /// </summary>
    internal enum TokenType
    {
        Unknown,
        DictionaryStart,
        DictionaryEnd,
        StreamStart,
        StreamEnd,
        HexStringStart,
        HexStringEnd,
        String,
        UnicodeString,
        Number,
        Real,
        Name,
        ArrayStart,
        ArrayEnd,
        Reference,
        ObjectStart,
        ObjectEnd,
        Boolean,
        HexDigit,
        Eof,
        Trailer,
        StartXRef,
        XRef,
        Null,
        ObjectType,
        HexStringWeird,
        HexStringWeirdEscape,
        WhiteSpace,
    }

    /// <summary>
    /// All element types.
    /// </summary>
    internal enum PDFType
    {
        Unknown,
        String,
        Array,
        Name,
        Boolean,
        Integer,
        Real,
        Reference,
        Dictionary,
        Stream,
        Comment,
        Null
    }

    /// <summary>
    /// Specfies the status of the IPdfPrmitive. Status is registered if it has a reference or else none.
    /// </summary>
#if NETFX_CORE || WP
    public enum ObjectStatus
#else
    internal enum ObjectStatus
#endif
    {
        None,
        Registered
    }
}
