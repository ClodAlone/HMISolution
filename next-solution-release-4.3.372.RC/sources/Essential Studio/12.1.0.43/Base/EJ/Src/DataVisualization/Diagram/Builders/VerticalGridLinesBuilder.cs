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
    public class VerticalGridLinesBuilder
    {
        public SnapSettings snapSettings;
        public VerticalGridLinesBuilder(SnapSettings snapSettings)
        {
            this.snapSettings = snapSettings;
        }

        public VerticalGridLinesBuilder LinesInterval(List<Decimal> linesInterval)
        {
            this.snapSettings.VerticalGridlines.LinesInterval = linesInterval;
            return this;
        }

        public VerticalGridLinesBuilder SnapInterval(List<Decimal> snapInterval)
        {
            this.snapSettings.VerticalGridlines.SnapInterval = snapInterval;
            return this;
        }

        public VerticalGridLinesBuilder Strokes(Action<StrokesBuilder> strokes)
        {
            var builder = new StrokesBuilder(this.snapSettings.VerticalGridlines);
            if (strokes != null)
                strokes.Invoke(builder);
            return this;
        }
    }
}
