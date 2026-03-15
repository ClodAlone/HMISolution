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
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class StripLinesBuilder
    {
        private StripLines stripLines;
        public StripLinesOptions stripLinesOptions = new StripLinesOptions();

        public StripLinesBuilder(StripLinesOptions stripLinesOptions)
        {
            this.stripLinesOptions = stripLinesOptions;
            stripLines = new StripLines();
        }
        public StripLinesBuilder Day(String day)
        {
            stripLines.Day = day;
            return this;
        }
        public StripLinesBuilder Label(String label)
        {
            stripLines.Label = label;
            return this;
        }
        public StripLinesBuilder LineStyle(String lineStyle)
        {
            stripLines.LineStyle = lineStyle;
            return this;
        }
        public StripLinesBuilder LineColor(String lineColor)
        {
            stripLines.LineColor = lineColor;
            return this;
        }
        public StripLinesBuilder LineWidth(int lineWidth)
        {
            stripLines.LineWidth = lineWidth;
            return this;
        }

        public void Add()
        {
            this.stripLinesOptions.StripLines.Add(stripLines);
            stripLines = new StripLines();
            //return this; 
        }
    }
}