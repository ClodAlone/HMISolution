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
using Syncfusion.JavaScript.DataVisualization.DiagramEnums;
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Controls;

namespace Syncfusion.JavaScript.DataVisualization.Builders
{
    public class HorizontalGridLinesBuilder
    {
        public SnapSettings snapSettings;
        public HorizontalGridLinesBuilder(SnapSettings snapSettings)
        {
            this.snapSettings = snapSettings;
        }

        public HorizontalGridLinesBuilder LinesInterval(List<Decimal> linesInterval)
        {
            this.snapSettings.HorizontalGridlines.LinesInterval = linesInterval;
            return this;
        }

        public HorizontalGridLinesBuilder SnapInterval(List<Decimal> snapInterval)
        {
            this.snapSettings.HorizontalGridlines.SnapInterval = snapInterval;
            return this;
        }

        public HorizontalGridLinesBuilder Strokes(Action<StrokesBuilder> strokes)
        {
            var builder = new StrokesBuilder(this.snapSettings.HorizontalGridlines);
            if (strokes != null)
                strokes.Invoke(builder);
            return this;
        }
    }
}
