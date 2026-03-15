#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.PdfViewer
{
    [Flags]
    internal enum PIFontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }


    internal class PIFont
    {
        public string Name { get; set; }
        public float Size { get; set; }
        public PIFontStyle Style { get; set; }

        public PIFont(string name, float size, PIFontStyle style)
        {
            Name = name;
            Size = size;
            Style = style;
        }
    }
}
