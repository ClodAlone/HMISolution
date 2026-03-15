#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

#if ( WINRT )
using Windows.UI;

#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation.XmlReaders
{
  public class TextSettings
  {
    /// <summary>
    /// Font name.
    /// </summary>
    public string FontName;
    /// <summary>
    /// Font size.
    /// </summary>
    public float? FontSize;
    /// <summary>
    /// Value indicating whether font is bold.
    /// </summary>
    public bool? Bold;
    /// <summary>
    /// Value indicating whether text 
    /// </summary>
    public bool? Italic;
    /// <summary>
    /// Value indicating whether text is underlined.
    /// </summary>
    public bool? Underline;
    /// <summary>
    /// Value indicating whether text is striked.
    /// </summary>
    public bool? Striked;
    /// <summary>
    /// Language used to display text.
    /// </summary>
    public string Language;
    /// <summary>
    /// Font color.
    /// </summary>
    public Color? FontColor;
    /// <summary>
    /// Represents the baseline properties
    /// </summary>
    public int Baseline;
    public bool? HasLatin;
    public bool? HasComplexScripts;
    public bool? HasEastAsianFont;
    public string ActualFontName;
    internal bool? ShowSizeProperties;
  }
}
