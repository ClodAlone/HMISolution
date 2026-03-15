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

namespace Syncfusion.HTMLUI.Base.Parser.HTML
{
  /// <summary>
  /// Type indicates where parser is currently.
  /// </summary>
  internal enum ParserPosition
  {
    /// <summary>
    /// Before text token.
    /// </summary>
    Text,
    /// <summary>
    /// Before tag token.
    /// </summary>
    Tag,
    /// <summary>
    /// Before attribute token.
    /// </summary>
    Attribute,
    /// <summary>
    /// Before CData element.
    /// </summary>
    CData
  };
  /// <summary>
  ///  Indicates which type of data is currently being parsed.
  /// </summary>
  internal enum ParserToken
  {
    /// <summary>
    /// Default value.
    /// </summary>
    None,
    /// <summary>
    ///  Token is text.
    /// </summary>
    Text,
    /// <summary>
    /// Token is whitespace.
    /// </summary>
    Space,
    /// <summary>
    /// Token is definition (&lt;!...&gt;).
    /// </summary>
    ProcessingInstruction,
    /// <summary>
    /// Token is &lt;![ .. ]&gt;.
    /// </summary>
    MarkedSection,
    /// <summary>
    /// Token is comment.
    /// </summary>
    Comment,
    /// <summary>
    /// Token is &lt;.
    /// </summary>
    StartTag,
    /// <summary>
    ///  Token is &gt;.
    /// </summary>
    StartTagEnd,
    /// <summary>
    /// Token is /&gt;.
    /// </summary>
    StartTagClosedEnd,
    /// <summary>
    /// Token is &lt;/.
    /// </summary>
    EndTag,
    /// <summary>
    /// Token is attribute name.
    /// </summary>
    Attribute,
    /// <summary>
    /// Token is attribute value.
    /// </summary>
    AttributeValue
  };

}