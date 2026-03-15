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
    public class SnapSettingsBuilder
    {
        public DiagramProperties diagramModel;
        public SnapSettingsBuilder(DiagramProperties model)
        {
            this.diagramModel = model;
        }
        public SnapSettingsBuilder HorizontalGridLines(Action<HorizontalGridLinesBuilder> horizontalGridLines)
        {
            var builder = new HorizontalGridLinesBuilder(this.diagramModel.SnapSettings);
            if (horizontalGridLines != null)
                horizontalGridLines.Invoke(builder);
            return this;
        }
        public SnapSettingsBuilder VerticalGridLines(Action<VerticalGridLinesBuilder> verticalGridLines)
        {
            var builder = new VerticalGridLinesBuilder(this.diagramModel.SnapSettings);
            if (verticalGridLines != null)
                verticalGridLines.Invoke(builder);
            return this;
        }

        public SnapSettingsBuilder SnapConstraints(SnapConstraints snapConstraints)
        {
            this.diagramModel.SnapSettings.SnapConstraints = snapConstraints;
            return this;
        }

        public SnapSettingsBuilder SnapToObject(bool snapToObject)
        {
            this.diagramModel.SnapSettings.SnapToObject = snapToObject;
            return this;
        }

        public SnapSettingsBuilder SnapAngle(int snapAngle)
        {
            this.diagramModel.SnapSettings.SnapAngle = snapAngle;
            return this;
        }
    }
}
