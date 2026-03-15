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
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models.Controls
{
    public partial class Label : TextBlock ,ICloneable
    {

        public Label()
        {
        }
        public Label(Label src)
        {
            this.Align = src.Align;
            this.Bold = src.Bold;
            this.BorderColor = src.BorderColor;
            this.BorderWidth = src.BorderWidth;
            this.FillColor = src.FillColor;
            this.FontColor = src.FontColor;
            this.FontFamily = src.FontFamily;
            this.FontSize = src.FontSize;
            this.Italic = src.Italic;
            this.Name = src.Name;
            this.Offset = src.Offset;
            this.ReadOnly = src.ReadOnly;
            this.Text = src.Text;
            this.TextDecoration = src.TextDecoration;
            this.Visible = src.Visible;

        }
        public object Clone()
        {
            return new Label(this);
        }
    }
}
