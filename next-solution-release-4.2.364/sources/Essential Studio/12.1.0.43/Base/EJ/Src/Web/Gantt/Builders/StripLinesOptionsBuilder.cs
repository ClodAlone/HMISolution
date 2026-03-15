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
    public class StripLinesOptionsBuilder
    {
         private StripLinesOptions stripLinesOptions = new StripLinesOptions();
         public StripLinesOptionsBuilder(StripLinesOptions stripLines)
        {
            stripLinesOptions = stripLines;
        }
         public StripLinesOptionsBuilder StripLines(Action<StripLinesBuilder> stripLines)
        {
            var builder = new StripLinesBuilder(stripLinesOptions);
            if (builder != null)
                stripLines.Invoke(builder);
            return this;
        }
         public StripLinesOptionsBuilder StripLines(List<StripLines> stripLines)
        {
            stripLinesOptions.StripLines = stripLines;
            return this;
        }
    }
}